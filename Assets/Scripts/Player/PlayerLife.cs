using UnityEngine;
using System.Collections;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] float m_darkDuration = 5;
    [SerializeField] float m_lightRecovery = 2.5f;
    [SerializeField] LayerMask m_deadlyLayer;

    float m_darkTime = 0;
    bool m_saturatedDarkness = false;

    private void Update()
    {
        if(LightSystem.instance.IsOnLight(transform))
            m_darkTime = Mathf.Max(m_darkTime - Time.deltaTime * m_lightRecovery, 0);
        else
        {
            m_darkTime += Time.deltaTime;
            if (m_darkTime > m_darkDuration && !m_saturatedDarkness)
                OnDarknessSaturation();
        }
    }

    void OnDarknessSaturation()
    {
        m_saturatedDarkness = true;
    }
}
