using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterSurface : MonoBehaviour
{
    [SerializeField] float m_resolution = 0.25f;
    [SerializeField] float m_width = 1;
    [SerializeField] float m_height = 1;
    [SerializeField] LayerMask m_interactionMask;
    [SerializeField] float m_splashSize = 1;
    [SerializeField] float m_splashPower = 1;
    [SerializeField] float m_spreadDistance = 1;
    [SerializeField] float m_spreadPower = 1;
    [SerializeField] float m_frictions = 1;

    class VertexInfo
    {
        public float height;
        public float velocity;
        public int verticeIndex;
    }

    class CollisionsInfo
    {
        public Transform transform;
        public Vector2 oldPosition;
    }

    List<VertexInfo> m_vertexs = new List<VertexInfo>();
    List<CollisionsInfo> m_collisions = new List<CollisionsInfo>();
    Mesh m_mesh;

    MeshFilter m_meshFilter;

    Vector3[] m_vertices;

    private void Start()
    {
        int nbVertex = Mathf.CeilToInt(m_width / m_resolution) + 1;
        m_vertexs.Clear();
        for (int i = 0; i < nbVertex; i++)
            m_vertexs.Add(new VertexInfo());

        m_meshFilter = GetComponent<MeshFilter>();

        GenerateMesh();
    }

    void Update()
    {
        UpdateInteractions();
        FlowWater();

        UpdateMesh();
    }

    void GenerateMesh()
    {
        int nb = m_vertexs.Count;

        m_vertices = new Vector3[nb * 2];
        Vector2[] uv = new Vector2[nb * 2];

        float delta = GetVerticeDelta();

        for (int i = 0; i < nb; i++)
        {
            m_vertices[i] = new Vector3(i * delta - m_width / 2, m_height / 2, 0); 
            uv[i] = new Vector2(i * delta / m_width, 1);
            m_vertexs[i].verticeIndex = i;
        }
        for (int i = 0; i < nb; i++)
        {
            m_vertices[i + nb] = new Vector3(i * delta - m_width / 2, -m_height / 2, 0);
            uv[i + nb] = new Vector2(i * delta / m_width, 0);
        }

        int[] triangles = new int[(m_vertexs.Count - 1) * 6];

        for (int i = 0; i < m_vertexs.Count - 1; i++)
        {
            triangles[i * 6] = nb + i;
            triangles[i * 6 + 1] = i;
            triangles[i * 6 + 2] = i + 1;
            triangles[i * 6 + 3] = nb + i;
            triangles[i * 6 + 4] = i + 1;
            triangles[i * 6 + 5] = nb + i + 1;
        }
        
        m_mesh = new Mesh();
        m_mesh.vertices = m_vertices;
        m_mesh.triangles = triangles;
        m_mesh.uv = uv;

        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();

        m_meshFilter.mesh = m_mesh;
    }

    void UpdateMesh()
    {
        int nb = m_vertexs.Count;

        for (int i = 0; i < nb; i++)
            m_vertices[m_vertexs[i].verticeIndex].y = m_height / 2 + m_vertexs[i].height;

        m_mesh.vertices = m_vertices;
    }

    void UpdateInteractions()
    {
        var objects = Physics2D.OverlapBoxAll(transform.position, new Vector2(m_width, m_height), 0, m_interactionMask.value);

        foreach(var o in objects)
        {
            var it = m_collisions.Find(x => { return x.transform == o.transform; });

            if(it == null)
            {
                var c = new CollisionsInfo();
                c.transform = o.transform;
                c.oldPosition = o.transform.position;
                m_collisions.Add(c);
                continue;
            }

            Vector2 pos = it.transform.position;

            var dir = pos - it.oldPosition;
            var dist = dir.magnitude;
            if (dist < 0.01f)
                continue;

            it.oldPosition = pos;

            float posX = pos.x - transform.position.x + m_width / 2;

            ApplyInteraction(posX, dir, dist);
        }

        m_collisions.RemoveAll(x =>
        {
            foreach (var c in objects)
                if (c.transform == x.transform)
                    return false;
            return true;
        });
    }

    void ApplyInteraction(float pos, Vector2 dir, float dist)
    {
        dir.Normalize();

        float delta = GetVerticeDelta();

        int min = Mathf.FloorToInt((pos - m_splashSize) / delta);
        min = Mathf.Clamp(min, 0, m_vertexs.Count);
        int max = Mathf.CeilToInt((pos + m_splashSize) / delta) + 1;
        max = Mathf.Clamp(max, 0, m_vertexs.Count);

        for (int i = min; i < max; i++)
        {
            float x = i * delta;

            m_vertexs[i].height += GetWaveValue(dir, x - pos) * dist * m_splashPower * Time.deltaTime;
        }
    }

    void FlowWater()
    {
        float delta = GetVerticeDelta();

        for(int i = 0; i < m_vertexs.Count; i++)
        {
            float sum = GetSpreadSumValue(i * delta);

            m_vertexs[i].velocity += -sum - m_vertexs[i].height * Time.deltaTime * m_spreadPower;
        }

        for (int i = 0; i < m_vertexs.Count; i++)
        {
            m_vertexs[i].height += m_vertexs[i].velocity * Time.deltaTime;
            m_vertexs[i].height *= 1 - m_frictions * Time.deltaTime;
        }
    }

    float GetVerticeDelta()
    {
        return m_width / (m_vertexs.Count - 1);
    }

    float GetWaveValue(Vector2 splashDir, float distance)
    {
        distance /= m_splashSize;
        if (Mathf.Abs(distance) > 1)
            return 0;

        distance *= Mathf.PI;
        
        float xValue = Mathf.Sin(distance);
        float yValue = Mathf.Cos(distance * 4 / 3);

        return splashDir.x * xValue + splashDir.y * yValue;
    }

    float GetSpreadValue(float distance)
    {
        distance = Mathf.Abs(distance);
        if (distance > m_spreadDistance)
            return 0;

        return (1 - (distance / m_spreadDistance));
    }

    float GetSpreadSumValue(float pos)
    {
        float sum = 0;
        float heightSum = 0;
        float delta = GetVerticeDelta();

        int min = Mathf.FloorToInt((pos - m_spreadDistance) / delta);
        min = Mathf.Clamp(min, 0, m_vertexs.Count);
        int max = Mathf.CeilToInt((pos + m_spreadDistance) / delta) + 1;
        max = Mathf.Clamp(max, 0, m_vertexs.Count);

        for (int i = min; i < max; i++)
        {
            float h = GetSpreadValue(i * delta - pos);
            heightSum += h;
            sum += h * m_vertexs[i].height;
        }
        sum /= heightSum;

        return sum;
    }
}
