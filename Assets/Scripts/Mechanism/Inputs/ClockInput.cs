using UnityEngine;
using System.Collections;

public class ClockInput : BaseInputs
{
    [SerializeField] float m_hightTime = 1;
    [SerializeField] float m_lowTime = 1;

    float m_timer = 0;
    
    void Update()
    {
        float ratio = m_lowTime / (m_lowTime + m_hightTime);
        float oldTime = m_timer / (m_lowTime + m_hightTime);

        m_timer += Time.deltaTime;

        float newTime = m_timer / (m_lowTime + m_hightTime);

        if(newTime > 1)
        {
            m_timer -= m_lowTime + m_hightTime;
            EndPress();
        }
        else if(newTime > ratio && oldTime <= ratio)
        {
            StartPress();
        }
    }
}
