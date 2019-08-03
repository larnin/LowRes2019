using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    string horizontalButton = "Horizontal";
    string jumpButton = "Jump";

    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] float m_jumpPower = 2;
    [SerializeField] float m_jumpHoldTime = 0.5f;
    [SerializeField] float m_jumpPressMaxDelay = 0.2f;
    [SerializeField] float m_groundDistance = 1;
    [SerializeField] float m_groundCheckRadius = 1;
    [SerializeField] LayerMask m_groundLayer;

    bool m_grounded = false;
    bool m_jumping = false;
    float m_jumpDuration = 0;

    float m_buttonMoveDir = 0;
    bool m_buttonJumpState = false;
    float m_buttonJumpPressTime = 0;

    Rigidbody2D m_rigidbody;

    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        m_buttonMoveDir = Input.GetAxisRaw(horizontalButton);
        m_buttonJumpState = Input.GetButton(jumpButton);
        if(Input.GetButtonDown(jumpButton))
            m_buttonJumpPressTime = 0;
    }

    void FixedUpdate()
    {
        UpdateGrounded();

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

        velocity.x = m_moveSpeed * m_buttonMoveDir;

        m_rigidbody.velocity = velocity;
    }

    void UpdateJump()
    {
        if(!m_jumping && m_grounded)
        {
            if(m_buttonJumpPressTime < m_jumpPressMaxDelay && m_buttonJumpState)
            {
                m_jumping = true;
                m_jumpDuration = 0;
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

    private void OnGUI()
    {
        GUI.Label(new Rect(5, 5, 200, 50), "Grounded " + m_grounded);
        GUI.Label(new Rect(5, 35, 200, 50), "Jumping " + m_jumping + " Hold " + m_jumpDuration);
    }
}
