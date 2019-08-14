using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class Path : MonoBehaviour
{
    [SerializeField] bool m_displayTrack = true;

    List<Vector2> m_pos = new List<Vector2>();
    List<float> m_lenghts = new List<float>();
    float m_lenght = 0;

    void OnDrawGizmos()
    {
        if (!m_displayTrack)
            return;

        if (transform.childCount < 2)
            return;

        for(int i = 0; i < transform.childCount - 1; i++)
        {
            var pos1 = transform.GetChild(i).position;
            var pos2 = transform.GetChild(i + 1).position;

            Gizmos.color = new Color(0, 255, 255);
            Gizmos.DrawLine(pos1, pos2);
        }
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            m_pos.Add(transform.GetChild(i).position);
        for (int i = 0; i < m_pos.Count - 1; i++)
            m_lenghts.Add((m_pos[i] - m_pos[i + 1]).magnitude);
        m_lenght = 0;
        foreach (var l in m_lenghts)
            m_lenght += l;
    }

    public float GetLenght()
    {
        if (m_lenght == 0)
            Start();
        return m_lenght;
    }

    public Vector2 GetPosAt(float dist)
    {
        if (m_lenght == 0)
            Start();

        if (dist <= 0)
            return m_pos[0];
        if (dist >= m_lenght)
            return m_pos[m_pos.Count - 1];

        for(int i = 0; i < m_lenghts.Count; i++)
        {
            if(dist < m_lenghts[i])
            {
                dist /= m_lenghts[i];

                return m_pos[i] * (1 - dist) + m_pos[i + 1] * dist;
            }

            dist -= m_lenghts[i];
        }

        return m_pos[m_pos.Count - 1];
    }
}
