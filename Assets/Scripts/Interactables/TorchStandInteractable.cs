using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;

public class TorchStandInteractable : BaseInteractable
{
    [SerializeField] bool m_haveTorch = true;
    [ShowIf("m_haveTorch")]
    [SerializeField] bool m_alight = true;
    [SerializeField] bool m_canTake = true;
    
    LightItem m_lightControler;
    Animator m_animator;

    private void Start()
    {
        m_lightControler = GetComponentInChildren<LightItem>();
        if (m_lightControler == null)
            Debug.LogError("The torch stand need a LightItem");

        m_animator = GetComponent<Animator>();
        SetTorch(m_haveTorch, m_alight);
    }

    void SetTorch(bool haveTorch, bool alight = true)
    {
        m_haveTorch = haveTorch;
        m_alight = alight;
        
        m_lightControler.gameObject.SetActive(m_alight && m_haveTorch);

        m_animator.SetBool("Torch", haveTorch);
        m_animator.SetBool("Alight", alight);
    }

    void SetAlight(bool alight)
    {
        m_alight = alight;
        
        m_lightControler.gameObject.SetActive(m_alight && m_haveTorch);

        m_animator.SetBool("Alight", alight);
    }

    bool HaveTorch()
    {
        return m_haveTorch;
    }

    bool IsAlight()
    {
        return m_haveTorch && m_alight;
    }

    public override bool OverrideItem(GameObject target)
    {
        var interact = target.GetComponent<PlayerInteract>();
        if (interact == null)
            return false;

        if (m_haveTorch && !m_alight && interact.GetCurrentItem() == ItemType.torch_on)
            return true;

        if (!m_haveTorch && (interact.GetCurrentItem() == ItemType.torch_on || interact.GetCurrentItem() == ItemType.torch_off))
            return true;

        return false;
    }

    public override bool CanUseAction1(GameObject target)
    {
        var interact = target.GetComponent<PlayerInteract>();
        if (interact == null)
            return false;

        if (OverrideItem(target))
            return true;

        if (m_canTake && m_haveTorch && interact.GetCurrentItem() == ItemType.empty)
            return true;

        return false;
    }

    public override string GetAction1Name(GameObject target)
    {
        var interact = target.GetComponent<PlayerInteract>();
        if (interact == null)
            return null;

        if (m_haveTorch && !m_alight && interact.GetCurrentItem() == ItemType.torch_on)
            return "Light";

        if (!m_haveTorch && (interact.GetCurrentItem() == ItemType.torch_on || interact.GetCurrentItem() == ItemType.torch_off))
            return "Put";

        if (m_canTake && m_haveTorch && interact.GetCurrentItem() == ItemType.empty)
            return "Take";

            return null;
    }

    public override void ExecAction1(GameObject target)
    {
        var interact = target.GetComponent<PlayerInteract>();
        if (interact == null)
            return;

        if (m_haveTorch && !m_alight && interact.GetCurrentItem() == ItemType.torch_on)
        {
            SetAlight(true);
        }
        else if (!m_haveTorch && (interact.GetCurrentItem() == ItemType.torch_on || interact.GetCurrentItem() == ItemType.torch_off))
        {
            SetTorch(true, interact.GetCurrentItem() == ItemType.torch_on);
            interact.SetCurrentItem(ItemType.empty);
        }
        else if (m_canTake && m_haveTorch && interact.GetCurrentItem() == ItemType.empty)
        {
            interact.SetCurrentItem(m_alight ? ItemType.torch_on : ItemType.torch_off);
            SetTorch(false);
        }
    }
}
