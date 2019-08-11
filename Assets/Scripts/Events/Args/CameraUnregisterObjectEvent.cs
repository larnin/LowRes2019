using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraUnregisterObjectEvent : EventArgs
{
    public CameraUnregisterObjectEvent(Transform _target)
    {
        target = _target;
    }

    public Transform target;
}
