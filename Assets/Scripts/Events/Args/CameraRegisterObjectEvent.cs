using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraRegisterObjectEvent : EventArgs
{
    public CameraRegisterObjectEvent(Transform _target, float _weight, bool _solo = false)
    {
        target = _target;
        weight = _weight;
        solo = _solo;
        permanent = true;
    }

    public CameraRegisterObjectEvent(Transform _target, float _weight, float _duration, bool _solo = false)
    {
        target = _target;
        weight = _weight;
        duration = _duration;
        permanent = false;
        solo = _solo;
    }

    public Transform target;
    public float weight;
    public float duration;
    public bool permanent;
    public bool solo;
}
