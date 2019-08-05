using UnityEngine;
using System.Collections;

public class TorchItem : BaseInteractable
{
    [SerializeField] bool m_alight;
    [SerializeField] float m_waterDetectionRadius = 1;
    [SerializeField] LayerMask m_waterLayer;

    LightItem m_lightControler;

    void Start()
    {
        m_lightControler = GetComponentInChildren<LightItem>();
        if (m_lightControler == null)
            Debug.LogError("The torch need a LightItem");
        m_lightControler.gameObject.SetActive(m_alight);
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
    }

    bool IsAlight()
    {
        return m_alight;
    }

    void SetAlight(bool alight)
    {
        if (m_alight == alight)
            return;

        m_alight = alight;

        m_lightControler.gameObject.SetActive(alight);
    }
}
