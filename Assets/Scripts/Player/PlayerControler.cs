using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public static PlayerControler instance { get; private set; }

    string horizontalButton = "Horizontal";
    string verticalButton = "Vertical";
    string jumpButton = "Jump";

    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] float m_moveSpeedLadder = 1;
    [SerializeField] float m_jumpPower = 2;
    [SerializeField] float m_jumpHoldTime = 0.5f;
    [SerializeField] float m_jumpPressMaxDelay = 0.2f;
    [SerializeField] float m_groundDistance = 1;
    [SerializeField] float m_groundCheckRadius = 1;
    [SerializeField] float m_ladderSpeed = 1;
    [SerializeField] float m_ladderCheckRadius = 1;
    [SerializeField] float m_deadzone = 0.1f;
    [SerializeField] LayerMask m_groundLayer;
    [SerializeField] LayerMask m_ladderLayer;

    bool m_grounded = false;
    bool m_jumping = false;
    float m_jumpDuration = 0;

    float m_buttonMoveDir = 0;
    float m_buttonVerticalDir = 0;
    bool m_buttonJumpState = false;
    float m_buttonJumpPressTime = 0;

    float m_defaultGravityScale = 0;

    bool m_onLadder = false;
    bool m_canGrabLadder = false;

    Rigidbody2D m_rigidbody;
    PlayerInteract m_playerItem;
    Animator m_animator;

    Transform m_spriteTransform;
    float m_spriteOriginalXPos;
    SpriteRenderer m_spriteRenderer;

    bool m_facingRight = true;
    bool m_idle;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("More than one player instancied");
        instance = this;
    }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_playerItem = GetComponent<PlayerInteract>();
        m_animator = GetComponent<Animator>();

        m_spriteTransform = transform.Find("Sprite");
        m_spriteOriginalXPos = m_spriteTransform.localPosition.x;
        m_spriteRenderer = m_spriteTransform.GetComponent<SpriteRenderer>();

        m_defaultGravityScale = m_rigidbody.gravityScale;

        Event<CameraRegisterObjectEvent>.Broadcast(new CameraRegisterObjectEvent(transform, 1));
        Event<CameraInstantMoveEvent>.Broadcast(new CameraInstantMoveEvent(transform.position));
    }

    void Update()
    {
        m_buttonMoveDir = Input.GetAxisRaw(horizontalButton);
        if (Mathf.Abs(m_buttonMoveDir) < m_deadzone)
            m_buttonMoveDir = 0;

        m_buttonVerticalDir = Input.GetAxisRaw(verticalButton);
        if (Mathf.Abs(m_buttonVerticalDir) < m_deadzone)
            m_buttonVerticalDir = 0;

        m_buttonJumpState = Input.GetButton(jumpButton);

        if(Input.GetButtonDown(jumpButton))
            m_buttonJumpPressTime = 0;

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        UpdateGrounded();

        UpdateLadder();

        UpdateSpeed();

        UpdateJump();
    }

    void UpdateGrounded()
    {
        var hit = Physics2D.CircleCast(transform.position, m_groundCheckRadius, Vector2.down, m_groundDistance, m_groundLayer.value);

        m_grounded = hit.collider != null;

        if (hit.collider == null)
            transform.parent = null;
        else transform.parent = hit.collider.transform;
    }

    void UpdateSpeed()
    {
        var velocity = m_rigidbody.velocity;

        if (m_onLadder)
            velocity.x = m_moveSpeedLadder * m_buttonMoveDir;
        else velocity.x = m_moveSpeed * m_buttonMoveDir;

        m_rigidbody.velocity = velocity;

        if (Mathf.Abs(velocity.x) > 0.1f)
        {
            m_facingRight = velocity.x > 0;
            m_spriteRenderer.flipX = !m_facingRight;
            m_spriteTransform.localPosition = new Vector3(m_spriteOriginalXPos * (m_facingRight ? 1.01f : -1), m_spriteTransform.localPosition.y, m_spriteTransform.localPosition.z);
            m_idle = false;
        }
        else m_idle = true;
    }

    void UpdateLadder()
    {
        var hit = Physics2D.OverlapCircle(transform.position, m_ladderCheckRadius, m_ladderLayer.value);

        m_canGrabLadder = hit != null;

        if (Mathf.Abs(m_buttonVerticalDir) > m_deadzone && m_canGrabLadder && CanUseLadders())
            EnterLadder();

        if (m_onLadder && !m_canGrabLadder)
            ExitLadder();

        if(m_onLadder)
        {
            var velocity = m_rigidbody.velocity;
            velocity.y = m_ladderSpeed * m_buttonVerticalDir;
            m_rigidbody.velocity = velocity;
        }
    }

    void UpdateJump()
    {
        if(!m_jumping && (m_grounded || m_onLadder))
        {
            if(m_buttonJumpPressTime < m_jumpPressMaxDelay && m_buttonJumpState)
            {
                m_jumping = true;
                m_jumpDuration = 0;
                ExitLadder();
            }
        }

        if (m_jumping && !m_buttonJumpState)
            m_jumping = false;

        var velocity = m_rigidbody.velocity;
        if (m_jumping)
            velocity.y = m_jumpPower;
        m_rigidbody.velocity = velocity;

        m_jumpDuration += Time.deltaTime;
        if (m_jumpDuration > m_jumpHoldTime)
            m_jumping = false;
        m_buttonJumpPressTime += Time.deltaTime;
    }

    bool CanUseLadders()
    {
        return m_playerItem.GetCurrentItem() == ItemType.empty;
    }
    
    void EnterLadder()
    {
        m_rigidbody.gravityScale = 0;

        var velocity = m_rigidbody.velocity;
        velocity.y = 0;
        m_rigidbody.velocity = velocity;

        m_onLadder = true;
        m_jumping = false;
    }

    void ExitLadder()
    {
        m_rigidbody.gravityScale = m_defaultGravityScale;

        m_onLadder = false;
    }

    public bool IsFacingRight()
    {
        return m_facingRight;
    }

    void UpdateAnimator()
    {
        string idleName = "Idle";
        string walkName = "Walking";
        string jumpingName = "Jumping";
        string fallingName = "Falling";
        string pushStateName = "PushState"; //0 = none, 1 = grab, 2 = push, 3 = pull
        string ladderStateName = "LadderState"; //0 = none, 1 = idle, 2 = down, 3 = up
        string throwTorchName = "ThrowTorch";
        string torchStateName = "TorchState"; //0 = empty, 1 = off, 2 = on

        var velocity = m_rigidbody.velocity;
        var torch = m_playerItem.GetCurrentItem();

        m_animator.SetInteger(torchStateName, torch == ItemType.torch_off ? 1 : (torch == ItemType.torch_on ? 2 : 0));

        if(m_onLadder)
        {
            m_animator.SetBool(idleName, false);
            m_animator.SetBool(walkName, false);
            m_animator.SetBool(jumpingName, false);
            m_animator.SetBool(fallingName, false);
            m_animator.SetInteger(pushStateName, 0);

            m_animator.SetInteger(ladderStateName, (Mathf.Abs(velocity.y) > 0.1f || Mathf.Abs(velocity.x) > 0.1f) ? (velocity.y >= 0 ? 3 : 2) : 1);
        }
        else
        {
            m_animator.SetBool(idleName, m_grounded && m_idle);
            m_animator.SetBool(walkName, m_grounded && !m_idle);
            m_animator.SetBool(jumpingName, !m_grounded && velocity.y > 0);
            m_animator.SetBool(fallingName, !m_grounded && velocity.y < 0);

            m_animator.SetInteger(pushStateName, 0);
            m_animator.SetInteger(ladderStateName, 0);
        }
    }
}
