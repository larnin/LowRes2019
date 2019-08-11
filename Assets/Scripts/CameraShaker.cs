using UnityEngine;
using System.Collections;
using NRand;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker instance { get; private set; }

    bool m_shake = false;
    float m_shakePower = 0;

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("More than one Camera Shaker instancied");
        instance = this;
    }

    void LateUpdate()
    {
        if (m_shake)
        {
            m_shake = false;

            transform.localPosition = new UniformVector2CircleDistribution(m_shakePower).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
        }
        else transform.localPosition = Vector3.zero;
    }

    public void ShakeThisFrame(float power)
    {
        m_shake = true;
        m_shakePower = power;
    }
}
