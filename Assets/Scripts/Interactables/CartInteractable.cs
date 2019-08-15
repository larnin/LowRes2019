using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;

public class CartInteractable : BaseInteractable
{
    const float deltaPosY = 4;

    [SerializeField] Path m_path = null;
    [SerializeField] float m_speed = 1;
    [SerializeField] bool m_reverse = false;
    [SerializeField] bool m_onlyOneRide = false;

    bool m_haveRided = false;
    bool m_rideDirection = false;
    bool m_riding = false;
    float m_rideDistance = 0;

    PlayerControler m_currentControler = null;
    Animator m_currentAnimator = null;
    float m_currentPosZ = 0;

    public override bool OverrideItem(GameObject target)
    {
        return true;
    }

    public override bool CanUseAction1(GameObject target)
    {
        return !m_onlyOneRide || !m_haveRided;
    }

    public override string GetAction1Name(GameObject target)
    {
        return "Ride";
    }

    public override void ExecAction1(GameObject target)
    {
        var controler = target.GetComponent<PlayerControler>();
        if (controler == null)
            return;

        m_haveRided = true;
        m_rideDistance = 0;

        controler.SetControlesDisabled(true);

        m_currentControler = controler;
        m_currentAnimator = target.GetComponent<Animator>();
        m_currentPosZ = target.transform.position.z;

        target.transform.parent = transform;

        Jump(transform.position + new Vector3(0, 0, 1), () => { m_riding = true; });
    }
    
    void Start()
    {
        if(m_path == null)
        {
            Debug.LogError("CartInteractable need a path");
            enabled = false;
            return;
        }

        if (m_reverse)
            m_rideDirection = true;

        var pos = m_path.GetPosAt(!m_rideDirection ? 0 : m_path.GetLenght());

        transform.position = new Vector3(pos.x, pos.y + deltaPosY, transform.position.z);
    }
    
    void Update()
    {
        float deltaJump = 8;

        if(m_riding)
        {
            float lenght = m_path.GetLenght();

            float nextDistance = m_rideDistance + Time.deltaTime * m_speed;

            var pos = m_path.GetPosAt(m_rideDirection ? lenght - m_rideDistance : m_rideDistance) + new Vector2(0, deltaPosY);
            var nextPos = m_path.GetPosAt(m_rideDirection ? lenght - nextDistance : nextDistance) + new Vector2(0, deltaPosY);

            var dPos = nextPos - pos;
            if (Mathf.Abs(dPos.y) < 0.01f)
                dPos.y = 0;

            if(nextDistance >= lenght)
            {
                dPos.y = 0;
                m_riding = false;
                m_rideDirection = !m_rideDirection;
                Jump(new Vector3(nextPos.x + (dPos.x > 0 ? deltaJump : -deltaJump), nextPos.y, m_currentPosZ), OnExitCart);
            }

            if (dPos.y == 0)
                transform.rotation = Quaternion.Euler(Vector3.zero);
            else if (dPos.y * (m_rideDirection ? -1 : 1) < 0)
                transform.rotation = Quaternion.Euler(0, 0, -45f);
            else transform.rotation = Quaternion.Euler(0, 0, 45f);

            transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
            m_rideDistance = nextDistance;
        }
    }

    void OnExitCart()
    {
        m_currentControler.SetControlesDisabled(false);
        m_currentControler.transform.parent = null;

        m_currentControler = null;
        m_currentAnimator = null;
    }

    void Jump(Vector3 target, Action callbackDone)
    {
        string idleName = "Idle";
        string walkName = "Walking";
        string jumpingName = "Jumping";
        string fallingName = "Falling";
        string pushStateName = "PushState"; //0 = none, 1 = grab, 2 = push, 3 = pull
        string ladderStateName = "LadderState"; //0 = none, 1 = idle, 2 = down, 3 = up

        float time = 0.5f;

        m_currentAnimator.SetBool(idleName, false);
        m_currentAnimator.SetBool(walkName, false);
        m_currentAnimator.SetBool(jumpingName, true);
        m_currentAnimator.SetBool(fallingName, false);
        m_currentAnimator.SetInteger(pushStateName, 0);
        m_currentAnimator.SetInteger(ladderStateName, 0);

        var transform = m_currentControler.transform;

        transform.DOMoveX(target.x, time).OnComplete(() => { DOVirtual.DelayedCall(0.1f, () => { callbackDone(); }); });

        float h = (m_currentControler.transform.position.y + target.y) / 2 + 8;

        transform.DOMoveY(h, time / 2).SetEase(Ease.OutQuad).OnComplete(()=> 
        {
            m_currentAnimator.SetBool(jumpingName, false);
            m_currentAnimator.SetBool(fallingName, true);

            transform.DOMoveY(target.y, time / 2).SetEase(Ease.InQuad).OnComplete(()=>
            {
                m_currentAnimator.SetBool(idleName, true);
                m_currentAnimator.SetBool(fallingName, false);
            });
        });

        transform.DOMoveZ(target.z, time);
    }
}
