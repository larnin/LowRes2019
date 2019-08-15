using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WaterFlowOutput : BaseOutput
{
    [SerializeField] float m_enableFrequency = 1;

    Animator m_animator;

    List<GameObject> m_waterChilds = new List<GameObject>();

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        for (int i = 0; i < transform.childCount; i++)
        {
            m_waterChilds.Add(transform.GetChild(i).gameObject);
        }
    }

    public override void SetActiveStatus(bool active)
    {
        for (int i = 0; i < m_waterChilds.Count; i++)
        {
            int j = i;
            DOVirtual.DelayedCall((i + 1) * 1 / m_enableFrequency, () => { m_waterChilds[j].SetActive(active); });
        }

        m_animator.SetBool("Flow", active);
    }
}
