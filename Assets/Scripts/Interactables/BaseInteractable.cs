using UnityEngine;
using System.Collections;

public abstract class BaseInteractable : MonoBehaviour
{
    public virtual bool OverrideItem() { return false; }
    public virtual bool CanUseAction1() { return false; }
    public virtual bool CanUseAction2() { return false; }
    public virtual string GetAction1Name() { return null; }
    public virtual string GetAction2Name() { return null; }
    public virtual void ExecAction1(GameObject target) { }
    public virtual void ExecAction2(GameObject target) { }

}
