using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InsanityOverlay : MonoBehaviour
{
    [SerializeField] float m_minInsanityShowOverlay = 0.5f;
    [SerializeField] float m_maxInsanityOpacityOverlay = 0.5f;
    Image m_image;
    SubscriberList m_subscriberList = new SubscriberList();

    private void Awake()
    {
        m_image = GetComponentInChildren<Image>();
        m_image.color = new Color(255, 255, 255, 0);

        m_subscriberList.Add(new Event<UpdateDarknessInsanityEvent>.Subscriber(OnSanityUpdate));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnSanityUpdate(UpdateDarknessInsanityEvent e)
    {
        float value = (e.value - m_minInsanityShowOverlay) / (1 - m_minInsanityShowOverlay);
        value = Mathf.Max(value, 0) * m_maxInsanityOpacityOverlay;
        
        m_image.color = new Color(1, 1, 1, value);
    }
}
