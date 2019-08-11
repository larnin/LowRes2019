using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CheckpointInteractable : BaseInteractable
{
    [SerializeField] string m_id = "none";

    LightItem m_light;

    public string GetID() { return m_id; }

    private void Awake()
    {
        m_light = GetComponentInChildren<LightItem>();
        m_light.gameObject.SetActive(false);
    }

    public override bool CanUseAction1(GameObject target)
    {
        var interact = target.GetComponent<PlayerInteract>();
        if (interact == null)
            return false;

        if (interact.GetCurrentItem() != ItemType.torch_on)
            return false;

        return true;
    }

    public override string GetAction1Name(GameObject target)
    {
        return "Save";
    }

    public override void ExecAction1(GameObject target)
    {
        CheckpointManager.instance.SetCurrentCheckpoint(m_id);
    }

    public void EnablePoint(bool value)
    {
        m_light.gameObject.SetActive(value);
    }
}
