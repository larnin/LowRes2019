using UnityEngine;
using System.Collections;

public abstract class BaseOutput : MonoBehaviour
{
    [SerializeField] bool m_inverseCondition = false;

    public bool IsConditionInversed()
    {
        return m_inverseCondition;
    }

    public abstract void SetActiveStatus(bool active);
}
