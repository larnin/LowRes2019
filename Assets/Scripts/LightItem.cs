using UnityEngine;
using System.Collections;

public class LightItem : MonoBehaviour
{
    [SerializeField] float m_radius = 1;

    bool m_added = false;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        var instance = LightSystem.instance;

        if (instance != null)
        {
            instance.AddLight(transform, m_radius);
            m_added = true;
        }
    }

    private void OnDisable()
    {
        var instance = LightSystem.instance;

        m_added = false;

        if (instance != null)
            instance.RemoveLight(transform);
    }

    private void OnDestroy()
    {
        var instance = LightSystem.instance;

        if (instance != null)
            instance.RemoveLight(transform);
    }

    private void Update()
    {
        if (!m_added)
            OnEnable();
    }
}
