using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsWriter : MonoBehaviour
{
    [SerializeField] float m_showTime = 2;
    [SerializeField] float m_fadeTime = 1;

    Text m_action1Text;
    Text m_action2Text;

    float m_action1Timer = 0;
    float m_action2Timer = 0;

    Color m_action1BaseColor;
    Color m_action2BaseColor;

    GameObject m_emiter;

    SubscriberList m_subscriberList = new SubscriberList();

    private void Start()
    {
        m_action1Text = transform.Find("Action1").GetComponent<Text>();
        m_action1BaseColor = m_action1Text.color;

        m_action2Text = transform.Find("Action2").GetComponent<Text>();
        m_action2BaseColor = m_action2Text.color;
    }

    private void Awake()
    {
        m_subscriberList.Add(new Event<AddActionTextEvent>.Subscriber(OnTextAdd));
        m_subscriberList.Add(new Event<StopActionTextEvent>.Subscriber(OnTextRemove));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void Update()
    {
        m_action1Timer -= Time.deltaTime;
        m_action2Timer -= Time.deltaTime;

        UpdateColor(m_action1Text, m_action1Timer, m_action1BaseColor);
        UpdateColor(m_action2Text, m_action2Timer, m_action2BaseColor);
    }

    void UpdateColor(Text text, float time, Color color)
    {
        if (time < 0)
            color.a = 0;
        else if (time < m_fadeTime)
            color.a *= time / m_fadeTime;

        text.color = color;
    }

    void OnTextAdd(AddActionTextEvent e)
    {
        if(e.action1Text != null)
        {
            m_action1Text.text = e.action1Text;
            m_action1Timer = m_showTime + m_fadeTime;
        }

        if (e.action2Text != null)
        {
            m_action2Text.text = e.action2Text;
            m_action2Timer = m_showTime + m_fadeTime;
        }

        m_emiter = e.emiter;
    }

    void OnTextRemove(StopActionTextEvent e)
    {
        if(m_emiter == null || e.emiter == null || e.emiter == m_emiter)
        {
            m_action1Timer = 0;
            m_action2Timer = 0;
        }
    }
}
