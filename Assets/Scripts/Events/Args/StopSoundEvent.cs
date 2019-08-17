using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StopSoundEvent : EventArgs
{
    public StopSoundEvent(AudioClip _clip)
    {
        clip = _clip;
    }

    public AudioClip clip;
}
