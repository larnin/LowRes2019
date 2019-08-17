using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DoorOutput : BaseOutput
{
    [SerializeField] AudioClip m_soundOpen = null;
    [SerializeField] AudioClip m_soundClose = null;
    [SerializeField] AudioClip m_soundMoving = null;
    [SerializeField] float m_soundVolume = 1;
    [SerializeField] float m_soundDelay = 0.4f;

    Animator m_animator;
    BoxCollider2D m_collider;

    bool m_canPlaySound = false;

    void Awake()
    {
        m_collider = GetComponent<BoxCollider2D>();
        m_animator = GetComponent<Animator>();

        DOVirtual.DelayedCall(m_soundDelay, () => { m_canPlaySound = true; });
    }

    public override void SetActiveStatus(bool active)
    {
        m_animator.SetBool("Open", active);

        DOVirtual.DelayedCall(0.3f, () => 
        {
            if (this == null)
                return;
            m_collider.enabled = !active;
        });

        if (!m_canPlaySound)
            return;

        Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(m_soundMoving, m_soundVolume));
        DOVirtual.DelayedCall(m_soundDelay, () =>
        {
            if (this == null)
                return;

            Event<PlaySoundEvent>.Broadcast(new PlaySoundEvent(active ? m_soundOpen : m_soundClose, m_soundVolume));
        });
    }
    
}
