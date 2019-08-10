using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightSystem : MonoBehaviour
{
    class LightInfo
    {
        public LightInfo(float _radius, Transform _transform)
        {
            radius = _radius;
            transform = _transform;
        }

        public float radius;
        public Transform transform;
    }

    public static LightSystem instance { get; private set; }

    List<LightInfo> m_lights = new List<LightInfo>();

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("Multiple LightSystem instance");
        instance = this;
    }

    public void AddLight(Transform transform, float radius)
    {
        if (transform == null)
            return;

        var item = m_lights.Find(x => { return x.transform == transform; });

        if (item != null)
            item.radius = radius;
        else m_lights.Add(new LightInfo(radius, transform));
    }

    public void RemoveLight(Transform transform)
    {
        m_lights.RemoveAll(x => { return x.transform == transform; });
    }

    bool IsOnLight(Transform transform)
    {
        return IsOnLight(transform.position);
    }

    public bool IsOnLight(Vector2 pos)
    {
        foreach(var l in m_lights)
        {
            Vector2 lPos = l.transform.position;

            if ((lPos - pos).sqrMagnitude < l.radius)
                return true;
        }
        return false;
    }

    public List<Vector4> GetLightParams()
    {
        var lights = new List<Vector4>();

        foreach(var l in m_lights)
        {
            var pos = l.transform.position;
            lights.Add(new Vector4(pos.x, pos.y, pos.z, l.radius));
        }

        return lights;
    }
}
