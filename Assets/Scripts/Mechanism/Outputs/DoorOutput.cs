using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DoorOutput : BaseOutput
{
    Animator m_animator;
    BoxCollider2D m_collider;

    void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();
    }

    public override void SetActiveStatus(bool active)
    {
        m_animator.SetBool("Open", active);

        DOVirtual.DelayedCall(0.3f, () => { m_collider.enabled = !active; });
    }
    
}
