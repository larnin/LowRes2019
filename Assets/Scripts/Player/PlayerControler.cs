using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
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

    bool m_facingRight = true;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_playerItem = GetComponent<PlayerInteract>();

        m_defaultGravityScale = m_rigidbody.gravityScale;
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
    }

    void UpdateSpeed()
    {
        var velocity = m_rigidbody.velocity;

        if (m_onLadder)
            velocity.x = m_moveSpeedLadder * m_buttonMoveDir;
        else velocity.x = m_moveSpeed * m_buttonMoveDir;

        m_rigidbody.velocity = velocity;

        if (Mathf.Abs(velocity.x) > 0.1f)
            m_facingRight = velocity.x > 0;
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
}
