using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NRand;

public class LightParam
{
    public LightParam(Vector4 _pos, Color _color)
    {
        pos = _pos;
        color = _color;
    }

    public Vector4 pos;
    public Color color;
}

public class LightSystem : MonoBehaviour
{
    [SerializeField] float m_radiusSpeed = 1;
    [SerializeField] float m_radiusOffset = 1;
    [SerializeField] float m_posSpeed = 1;
    [SerializeField] float m_posOffset = 1;
    [SerializeField] float m_lightingTime = 0.4f;

    class LightInfo
    {
        public LightInfo(float _radius, Transform _transform, Color _color)
        {
            radius = _radius;
            transform = _transform;
            color = _color;
        }

        public float radius;
        public Transform transform;
        public bool lightingOn = true;
        public float time = 0;
        public float radiusOffset = 0;
        public float radiusOffsetTarget = 0;
        public Vector2 posOffset = Vector2.zero;
        public Vector2 offsetTarget = Vector2.zero;
        public Color color;
    }

    class LightOffInfo
    {
        public LightOffInfo(float _radius, Vector2 _pos, Color _color)
        {
            baseRadius = _radius;
            pos = _pos;
            color = _color;
        }

        public float baseRadius;
        public Vector2 pos;
        public float time = 0;
        public Color color;
    }

    public static LightSystem instance { get; private set; }

    List<LightInfo> m_lights = new List<LightInfo>();
    List<LightOffInfo> m_lightsOff = new List<LightOffInfo>();


    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Multiple LightSystem instance");
        instance = this;
    }

    public void AddLight(Transform transform, float radius, Color color)
    {
        if (transform == null)
            return;

        var item = m_lights.Find(x => { return x.transform == transform; });

        if (item != null)
        {
            item.radius = radius;
            item.color = color;
        }
        else m_lights.Add(new LightInfo(radius, transform, color));
    }

    public void RemoveLight(Transform transform)
    {
        for(int i = 0; i < m_lights.Count; i++)
        {
            if(m_lights[i].transform == transform)
            {
                m_lightsOff.Add(new LightOffInfo(m_lights[i].radius, m_lights[i].transform.position, m_lights[i].color));
                m_lights.RemoveAt(i);
                break;
            }
        }
    }

    public bool IsOnLight(Transform transform)
    {
        return IsOnLight(transform.position);
    }

    public bool IsOnLight(Vector2 pos)
    {
        foreach(var l in m_lights)
        {
            Vector2 lPos = l.transform.position;

            if ((lPos - pos).sqrMagnitude < l.radius * l.radius)
                return true;
        }
        return false;
    }

    public List<LightParam> GetLightParams()
    {
        var lights = new List<LightParam>();

        foreach(var l in m_lights)
        {
            var pos = l.transform.position + new Vector3(l.posOffset.x, l.posOffset.y, 0);
            lights.Add(new LightParam(new Vector4(pos.x, pos.y, pos.z, l.radius + l.radiusOffset), l.color));
        }

        foreach(var l in m_lightsOff)
        {
            float radius = (1 - (l.time / m_lightingTime)) * l.baseRadius;
            lights.Add(new LightParam(new Vector4(l.pos.x, l.pos.y, 0, radius), l.color));
        }

        return lights;
    }

    private void Update()
    {
        UpdateLights();
        UpdateLightsOff();

        DebugLight();
    }

    void UpdateLights()
    {
        foreach(var l in m_lights)
        {
            if(l.lightingOn)
            {
                l.time += Time.deltaTime;
                if(l.time >= m_lightingTime)
                {
                    l.lightingOn = false;
                    l.radiusOffset = 0;
                    continue;
                }
                l.radiusOffset = -(1 - (l.time / m_lightingTime)) * l.radius;
            }
            else
            {
                {
                    float d = (l.posOffset - l.offsetTarget).magnitude;
                    float delta = Time.deltaTime * m_posSpeed;
                    if (delta >= d)
                    {
                        l.posOffset = l.offsetTarget;
                        l.offsetTarget = new UniformVector2CircleDistribution(m_posOffset).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
                    }
                    else l.posOffset += (l.offsetTarget - l.posOffset) / d * delta;
                }

                {
                    float d = l.radiusOffset - l.radiusOffsetTarget;
                    float delta = Time.deltaTime * m_radiusSpeed;

                    if (delta >= Mathf.Abs(d))
                    {
                        l.radiusOffset = l.radiusOffsetTarget;
                        l.radiusOffsetTarget = new UniformFloatDistribution(0, m_radiusOffset).Next(new StaticRandomGenerator<DefaultRandomGenerator>());
                    }
                    else l.radiusOffset += d > 0 ? -delta : delta;
                }
                
            }
        }
    }

    void UpdateLightsOff()
    {
        foreach(var l in m_lightsOff)
            l.time += Time.deltaTime;

        m_lightsOff.RemoveAll(x => { return x.time >= m_lightingTime; });
    }

    void DebugLight()
    {
        foreach(var l in m_lights)
        {
            int segments = 10;
            float step = 2 * Mathf.PI / segments;
            for(int i = 0; i < segments; i++)
            {
                Vector2 pos1 = new Vector2(Mathf.Cos(step * i), Mathf.Sin(step * i)) * l.radius;
                Vector2 pos2 = new Vector2(Mathf.Cos(step * (i + 1)), Mathf.Sin(step * (i + 1))) * l.radius;

                Debug.DrawLine(new Vector3(pos1.x, pos1.y, 0) + l.transform.position, new Vector3(pos2.x, pos2.y, 0) + l.transform.position, l.color);
            }
        }
    }
}
