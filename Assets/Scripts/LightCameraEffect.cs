using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]

public class LightCameraEffect : MonoBehaviour
{
    string lightCountName = "_LightCount";
    string lightsName = "_Lights";
    string maxDistanceName = "_MaxLightDistance";
    string ambiantName = "_Ambiant";
    int maxLightNb = 20;

    [SerializeField] float m_lightMaxDistance = 20;
    [SerializeField] float m_ambiantLightValue = 0.1f;

    public Material material;

    Camera m_camera;

    void Start()
    {
        if (material == null || material.shader == null || !material.shader.isSupported)
        {
            enabled = false;
            return;
        }

        m_camera = GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    private void Update()
    {
        if(LightSystem.instance == null)
        {
            material.SetInt(lightCountName, 0);
            return;
        }

        Vector2 pos = transform.position;
        var size = m_camera.orthographicSize;

        var lights = LightSystem.instance.GetLightParams();

        for (int i = 0; i < lights.Count; i++)
        {
            var l = lights[i];
            l.x -= pos.x - size;
            l.y -= pos.y - size;

            lights[i] = l;
        }

        if(lights.Count == 0)
        {
            material.SetInt(lightCountName, 0);
        }
        else if(lights.Count <= maxLightNb)
        {
            material.SetVectorArray(lightsName, lights.ToArray());
            material.SetInt(lightCountName, lights.Count);
        }
        else
        {
            lights.Sort((x, y) => 
            {
                var d1 = (pos - new Vector2(x.x, x.y)).sqrMagnitude;
                var d2 = (pos - new Vector2(y.x, y.y)).sqrMagnitude;
                return d1.CompareTo(d2);
            });

            var lightsArray = new Vector4[maxLightNb];
            for (int i = 0; i < maxLightNb; i++)
                lightsArray[i] = lights[i];
            material.SetVectorArray(lightsName, lightsArray);
            material.SetInt(lightCountName, maxLightNb);
        }

        material.SetFloat(maxDistanceName, m_lightMaxDistance);
        material.SetFloat(ambiantName, m_ambiantLightValue);
    }
}
