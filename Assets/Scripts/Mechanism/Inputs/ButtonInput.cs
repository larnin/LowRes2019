using UnityEngine;
using System.Collections;

public class ButtonInput : BaseInputs
{
    float m_width = 3;
    bool m_pressed = false;

    Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetBool("Press", m_pressed);
    }

    private void Update()
    {
        bool overlap = Physics2D.OverlapBox(transform.position, new Vector2(m_width, m_width), 0) != null;

        if(overlap != m_pressed)
        {
            m_pressed = overlap;
            if (m_pressed)
                StartPress();
            else EndPress();

            m_animator.SetBool("Press", m_pressed);
        }
    }
}
