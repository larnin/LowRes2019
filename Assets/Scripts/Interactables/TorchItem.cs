using UnityEngine;
using System.Collections;

public class TorchItem : BaseInteractable
{
    [SerializeField] bool m_alight;

    LightItem m_lightControler;

    void Start()
    {
        m_lightControler = GetComponentInChildren<LightItem>();
        if (m_lightControler == null)
            Debug.LogError("The torch need a LightItem");
    }

    public override bool CanUseAction1()
    {
        return true;
    }

    public override string GetAction1Name()
    {
        return "Take";
    }

    public override void ExecAction1()
    {
        
    }
}
