using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddActionTextEvent : EventArgs
{
    public AddActionTextEvent(string _action1Text, string _action2Text, GameObject _emiter)
    {
        action1Text = _action1Text;
        action2Text = _action2Text;
    }

    public string action1Text;
    public string action2Text;
    public GameObject emiter;
}
