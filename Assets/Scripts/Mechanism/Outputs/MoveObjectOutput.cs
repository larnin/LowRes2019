using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MoveObjectOutput : BaseOutput
{
    [SerializeField] Vector2 m_enableOffset = Vector2.zero;
    [SerializeField] float m_moveDuration = 1;

    bool m_active = false;
    Vector3 m_offPos;
    Vector3 m_onPos;

    private void Start()
    {
        m_offPos = transform.position;
        m_onPos = transform.position + new Vector3(m_enableOffset.x, m_enableOffset.y, 0);
    }

    private void OnDrawGizmosSelected()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        Vector2 min = transform.position;
        Vector2 max = transform.position;

        foreach(var r in renderers)
        {
            var b = r.bounds;
            var bMin = b.min;
            var bMax = b.max;

            min.x = Mathf.Min(min.x, bMin.x);
            min.y = Mathf.Min(min.y, bMin.y);
            max.x = Mathf.Max(max.x, bMax.x);
            max.y = Mathf.Max(max.y, bMax.y);
        }
        min += m_enableOffset;
        max += m_enableOffset;

        Gizmos.color = new Color(0, 0, 255);
        Vector3 pos1 = new Vector3(min.x, min.y, transform.position.z);
        Vector3 pos2 = new Vector3(max.x, min.y, transform.position.z);
        Gizmos.DrawLine(pos1, pos2);
        pos1 = pos2;
        pos2.y = max.y;
        Gizmos.DrawLine(pos1, pos2);
        pos1 = pos2;
        pos2.x = min.x;
        Gizmos.DrawLine(pos1, pos2);
        pos1 = pos2;
        pos2.y = min.y;
        Gizmos.DrawLine(pos1, pos2);

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(m_enableOffset.x, m_enableOffset.y, 0));
    }

    public override void SetActiveStatus(bool active)
    {
        if (active == m_active)
            return;

        m_active = active;

        transform.DOMove(m_active ? m_onPos : m_offPos, m_moveDuration);
    }
}
