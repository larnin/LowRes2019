using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    empty,
    torch_on,
    torch_off,
}

public class PlayerInteract : MonoBehaviour
{
    string interaction1Button = "Action1";
    string interaction2Button = "Action2";

    [SerializeField] float m_interactionRadius = 1;
    [SerializeField] LayerMask m_interactionLayer;
    [SerializeField] float m_waterDetectionRadius = 1;
    [SerializeField] LayerMask m_waterLayer;
    [SerializeField] GameObject m_torchOnPrefab = null;
    [SerializeField] GameObject m_torchOffPrefab = null;
    [SerializeField] float m_throwPower = 1;
    [SerializeField] float m_throwAngle = 10;
    [SerializeField] Vector2 m_dropOffset = new Vector2(0, 2);

    ItemType m_itemType = ItemType.empty;

    LightItem m_lightItem;
    PlayerControler m_controler;

    BaseInteractable m_currentInteractable;
    BaseInteractable m_lockedInteractable;

    public ItemType GetCurrentItem()
    {
        return m_itemType;
    }

    public void SetCurrentItem(ItemType type)
    {
        if (m_itemType == type)
            return;

        if (m_itemType == ItemType.torch_on)
            m_lightItem.gameObject.SetActive(false);

        Debug.Log("Set Item from " + m_itemType + " to " + type);

        m_itemType = type;

        if (m_itemType == ItemType.torch_on)
            m_lightItem.gameObject.SetActive(true);
    }

    void LockInteractable(BaseInteractable interactable)
    {
        m_lockedInteractable = interactable;
    }

    private void Start()
    {
        m_lightItem = GetComponentInChildren<LightItem>();
        if (m_itemType != ItemType.torch_on)
            m_lightItem.gameObject.SetActive(false);
        m_controler = GetComponent<PlayerControler>();
    }

    private void Update()
    {
        if (m_lockedInteractable == null)
            UpdateCurrentInteractable();

        BaseInteractable interactable = m_lockedInteractable == null ? m_currentInteractable : m_lockedInteractable;

        if (interactable != null)
        {
            if (interactable.CanUseAction1(gameObject) && Input.GetButtonDown(interaction1Button))
                interactable.ExecAction1(gameObject);
            if (interactable.CanUseAction2(gameObject) && Input.GetButtonDown(interaction2Button))
                interactable.ExecAction2(gameObject);
        }
        else
        {
            if(Input.GetButtonDown(interaction1Button))
                ExecAction1();
            if (Input.GetButtonDown(interaction2Button))
                ExecAction2();
        }

        if (m_itemType == ItemType.torch_on )
        {
            var c = Physics2D.OverlapCircle(transform.position, m_waterDetectionRadius, m_waterLayer.value);
            if (c != null)
                SetCurrentItem(ItemType.torch_off);
        }
    }

    void UpdateCurrentInteractable()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, m_interactionRadius, m_interactionLayer.value);

        Vector2 pos = transform.position;
        BaseInteractable nextInteractable = m_currentInteractable;

        foreach (var c in colliders)
        {
            var comp = c.GetComponent<BaseInteractable>();
            if (comp == null)
                continue;

            if (m_itemType == ItemType.empty || comp.OverrideItem(gameObject))
            {
                bool canAdd = true;
                if (nextInteractable != null)
                {
                    Vector2 currentPos = nextInteractable.transform.position;
                    Vector2 itemPos = c.transform.position;
                    if ((itemPos - pos).sqrMagnitude >= (currentPos - pos).sqrMagnitude)
                        canAdd = false;
                }

                if (canAdd)
                    nextInteractable = comp;
            }
        }

        if (nextInteractable != m_currentInteractable)
        {
            m_currentInteractable = nextInteractable;
            Event<AddActionTextEvent>.Broadcast(new AddActionTextEvent(m_currentInteractable.CanUseAction1(gameObject) ? m_currentInteractable.GetAction1Name(gameObject) : null
                                                                     , m_currentInteractable.CanUseAction2(gameObject) ? m_currentInteractable.GetAction2Name(gameObject) : null
                                                                     , gameObject));
        }

        if (m_currentInteractable != null)
        {
            Vector2 interactablePos = m_currentInteractable.transform.position;
            if ((interactablePos - pos).sqrMagnitude > m_interactionRadius * m_interactionRadius)
            {
                m_currentInteractable = null;
                Event<StopActionTextEvent>.Broadcast(new StopActionTextEvent(gameObject));

                if(m_lockedInteractable == null)
                    Event<AddActionTextEvent>.Broadcast(new AddActionTextEvent(GetAction1Name(), GetAction2Name(), gameObject));
            }
        }
    }

    void ExecAction1()
    {
        if (m_itemType == ItemType.torch_off || m_itemType == ItemType.torch_on)
        {
            var obj = Instantiate(m_itemType == ItemType.torch_on ? m_torchOnPrefab : m_torchOffPrefab);

            var offset = m_dropOffset;
            var facing = m_controler.IsFacingRight();
            if (!facing)
                offset.x *= -1;

            obj.transform.position = transform.position + new Vector3(m_dropOffset.x, m_dropOffset.y, 0);

            SetCurrentItem(ItemType.empty);
        }
    }

    void ExecAction2()
    {
        if (m_itemType == ItemType.torch_off || m_itemType == ItemType.torch_on)
        {
            var obj = Instantiate(m_itemType == ItemType.torch_on ? m_torchOnPrefab : m_torchOffPrefab);

            var offset = m_dropOffset;
            var facing = m_controler.IsFacingRight();
            if (!facing)
                offset.x *= -1;

            obj.transform.position = transform.position + new Vector3(m_dropOffset.x, m_dropOffset.y, 0);

            var rigidbody = obj.GetComponent<Rigidbody2D>();
            if(rigidbody != null)
            {
                var dir = new Vector2(Mathf.Cos(m_throwAngle), Mathf.Sin(m_throwAngle)) * m_throwPower;
                if (!facing)
                    dir.x *= -1;
                rigidbody.velocity = dir;
            }

            SetCurrentItem(ItemType.empty);
        }
    }

    string GetAction1Name()
    {
        if (m_itemType == ItemType.torch_off || m_itemType == ItemType.torch_on)
            return "Drop";

        return null;
    }

    string GetAction2Name()
    {
        if (m_itemType == ItemType.torch_off || m_itemType == ItemType.torch_on)
            return "Throw";
        return null;
    }
}
