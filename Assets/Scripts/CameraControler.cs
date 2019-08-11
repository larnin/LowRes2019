using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraControler : MonoBehaviour
{
    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] float m_moveSpeedPow = 1;
    [SerializeField] float m_maxSpeed = 100;
    [SerializeField] float m_minSpeed = 2;

    class TargetItem
    {
        public Transform target;
        public float weight;
        public float duration;
        public bool permanent;
        public bool solo;
    }

    SubscriberList m_subscriberList = new SubscriberList();

    List<TargetItem> m_targets = new List<TargetItem>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<CameraRegisterObjectEvent>.Subscriber(OnRegisterObject));
        m_subscriberList.Add(new Event<CameraUnregisterObjectEvent>.Subscriber(OnUnregisterObject));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void LateUpdate()
    {
        if (m_targets.Count == 0)
            return;

        bool solo = false;

        Vector2 target = Vector2.zero;
        float targetWeight = 0;

        foreach(var t in m_targets)
        {
            if(t.solo && !solo)
            {
                target = Vector2.zero;
                targetWeight = 0;
                solo = true;
            }

            if(solo == t.solo)
            {
                target += new Vector2(t.target.position.x, t.target.position.y) * t.weight;
                targetWeight += t.weight;
            }

            if (!t.permanent)
                t.duration -= Time.deltaTime;
        }

        if (targetWeight == 0)
            return;

        m_targets.RemoveAll(x => { return !x.permanent && x.duration <= 0; });

        target /= targetWeight;

        Vector2 pos = transform.position;

        var dir = target - pos;
        float distance = dir.magnitude;
        if (distance > 0.1f)
        {
            float dTarget = Mathf.Pow(distance * m_moveSpeed, m_moveSpeedPow);
            dTarget = Mathf.Clamp(dTarget, m_minSpeed, m_maxSpeed) * Time.deltaTime;
            if(dTarget < distance)
                dir *= dTarget / distance;
        }

        transform.position = transform.position + new Vector3(dir.x, dir.y, 0);
    }

    void OnRegisterObject(CameraRegisterObjectEvent e)
    {
        var it = m_targets.Find(x => { return x.target == e.target; });

        if(it != null)
        {
            it.duration = e.duration;
            it.weight = e.weight;
            it.solo = e.solo;
            it.permanent = e.permanent;
        }
        else
        {
            var obj = new TargetItem();
            obj.target = e.target;
            obj.duration = e.duration;
            obj.weight = e.weight;
            obj.solo = e.solo;
            obj.permanent = e.permanent;

            m_targets.Add(obj);
        }
    }

    void OnUnregisterObject(CameraUnregisterObjectEvent e)
    {
        m_targets.RemoveAll(x => { return x.target == e.target; });
    }

    void OnInstantMove(CameraInstantMoveEvent e)
    {
        transform.position = new Vector3(e.target.x, e.target.y, transform.position.z);
    }
}
