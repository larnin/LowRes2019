using UnityEngine;
using System.Collections;

public class TorchItem : BaseInteractable
{
    [SerializeField] bool m_alight;
    [SerializeField] float m_waterDetectionRadius = 1;
    [SerializeField] LayerMask m_waterLayer;
    [SerializeField] float m_groundCheckDistance = 1;
    [SerializeField] LayerMask m_groundLayer;

    LightItem m_lightControler;
    Animator m_animator;

    void Start()
    {
        m_lightControler = GetComponentInChildren<LightItem>();
        if (m_lightControler == null)
            Debug.LogError("The torch need a LightItem");
        m_lightControler.gameObject.SetActive(m_alight);
        m_animator = GetComponent<Animator>();
    }

    public override bool CanUseAction1(GameObject target)
    {
        return true;
    }

    public override string GetAction1Name(GameObject target)
    {
        return "Take";
    }

    public override void ExecAction1(GameObject target)
    {
        var interact = target.GetComponent<PlayerInteract>();
        if (interact == null)
            return;

        if (interact.GetCurrentItem() != ItemType.empty)
            return;

        interact.SetCurrentItem(m_alight ? ItemType.torch_on : ItemType.torch_off);

        Destroy(gameObject);
    }

    private void Update()
    {
        if(m_alight)
        {
            var c = Physics2D.OverlapCircle(transform.position, m_waterDetectionRadius, m_waterLayer.value);
            if (c != null)
                SetAlight(false);
        }

        var ground = Physics2D.Raycast(transform.position, Vector2.down, m_groundCheckDistance, m_groundLayer.value);
        bool grounded = ground.collider != null;

        m_animator.SetBool("Alight", m_alight);
        m_animator.SetBool("Grounded", grounded);
    }

    public bool IsAlight()
    {
        return m_alight;
    }

    public void SetAlight(bool alight)
    {
        if (m_alight == alight)
            return;

        m_alight = alight;

        m_lightControler.gameObject.SetActive(alight);
    }
}
