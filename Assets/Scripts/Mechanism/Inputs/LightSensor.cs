using UnityEngine;
using System.Collections;

public class LightSensor : BaseInputs
{
    Animator m_animator;

    bool m_pressed = false;

    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.SetBool("Light", m_pressed);
    }

    void Update()
    {
        bool pressed = LightSystem.instance.IsOnLight(transform);

        if(pressed != m_pressed)
        {
            m_pressed = pressed;
            m_animator.SetBool("Light", m_pressed);
            if (m_pressed)
                StartPress();
            else EndPress();
        }
    }
}
