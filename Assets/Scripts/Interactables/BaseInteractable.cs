using UnityEngine;
using System.Collections;

public abstract class BaseInteractable : MonoBehaviour
{
    public virtual bool OverrideItem(GameObject target) { return false; }
    public virtual bool CanUseAction1(GameObject target) { return false; }
    public virtual bool CanUseAction2(GameObject target) { return false; }
    public virtual string GetAction1Name(GameObject target) { return null; }
    public virtual string GetAction2Name(GameObject target) { return null; }
    public virtual void ExecAction1(GameObject target) { }
    public virtual void ExecAction2(GameObject target) { }

}
