using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum InputType
{
    Press,
    PermanentSet,
    Togglable,
}

public class BaseInputs : MonoBehaviour
{
    [SerializeField] InputType m_inputType = InputType.Press;
    [SerializeField] List<BaseOutput> m_connectedOutputs = new List<BaseOutput>();

    bool m_pressState = false;

    private void Start()
    {
        SetPressState(false);
    }

    protected void StartPress()
    {
        switch(m_inputType)
        {
            case InputType.PermanentSet:
            case InputType.Press:
                if (!m_pressState)
                    SetPressState(true);
                break;
            case InputType.Togglable:
                SetPressState(!m_pressState);
                break;
        }

    }

    protected void EndPress()
    {
        switch (m_inputType)
        {
            case InputType.PermanentSet:
            case InputType.Togglable:
                //nothing to do here
                break;
            case InputType.Press:
                SetPressState(false);
                break;
        }
    }

    void SetPressState(bool press)
    {
        m_pressState = press;

        foreach (var c in m_connectedOutputs)
            c.SetActiveStatus(press);
    }
}
