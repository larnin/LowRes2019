using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StopActionTextEvent : EventArgs
{
    public StopActionTextEvent(GameObject _emiter)
    {
        emiter = _emiter;
    }

    public GameObject emiter;
}
