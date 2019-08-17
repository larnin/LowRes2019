using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundSystem : MonoBehaviour
{
    static SoundSystem instance;

    List<AudioSource> m_sources = new List<AudioSource>();

    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        m_subscriberList.Add(new Event<PlaySoundEvent>.Subscriber(OnPlaySound));
        m_subscriberList.Add(new Event<StopSoundEvent>.Subscriber(OnStopSound));
        m_subscriberList.Subscribe();

        for(int i = 0; i < transform.childCount; i++)
        {
            var s = transform.GetChild(i).GetComponent<AudioSource>();
            if (s != null)
                m_sources.Add(s);
        }
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnPlaySound(PlaySoundEvent e)
    {
        foreach(var s in m_sources)
        {
            if(!s.isPlaying)
            {
                s.clip = e.clip;
                s.loop = e.loop;
                s.volume = e.volume;
                s.Play();
                return;
            }
        }

        Debug.LogWarning("Cannot add the new sound " + e.ToString() + ": No free space");
    }

    void OnStopSound(StopSoundEvent e)
    {
        foreach(var s in m_sources)
        {
            if(s.clip == e.clip)
                s.Stop();
        }
    }
}
