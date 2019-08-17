using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlaySoundEvent : EventArgs
{
    public PlaySoundEvent(AudioClip _clip, float _volume, bool _loop = false)
    {
        clip = _clip;
        volume = _volume;
        loop = _loop;
    }

    public AudioClip clip;
    public float volume;
    public bool loop;
}
