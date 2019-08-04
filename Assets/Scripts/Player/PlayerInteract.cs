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

    [SerializeField] float m_interactionRadius;
    [SerializeField] LayerMask m_interactionLayer;
    [SerializeField] float m_waterDetectionRadius = 1;
    [SerializeField] LayerMask m_waterLayer;

    ItemType m_itemType = ItemType.empty;

    LightItem m_lightItem;

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

        m_itemType = type;

        if (m_itemType == ItemType.torch_off)
            m_lightItem.gameObject.SetActive(true);
    }

    void LockInteractable(BaseInteractable interactable)
    {
        m_lockedInteractable = interactable;
    }

    private void Start()
    {
        m_lightItem = GetComponentInChildren<LightItem>();
    }

    private void Update()
    {
        if (m_lockedInteractable == null)
            UpdateCurrentInteractable();

        BaseInteractable interactable = m_lockedInteractable == null ? m_currentInteractable : m_lockedInteractable;

        if (interactable.CanUseAction1() && Input.GetButtonDown(interaction1Button))
            interactable.ExecAction1(gameObject);
        if (interactable.CanUseAction2() && Input.GetButtonDown(interaction2Button))
            interactable.ExecAction2(gameObject);

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

            if (m_itemType == ItemType.empty || comp.OverrideItem())
            {
                Vector2 currentPos = nextInteractable.transform.position;
                Vector2 itemPos = c.transform.position;

                if ((itemPos - pos).sqrMagnitude < (currentPos - pos).sqrMagnitude)
                    nextInteractable = comp;
            }
        }

        if (nextInteractable != m_currentInteractable)
        {
            m_currentInteractable = nextInteractable;
            Event<AddActionTextEvent>.Broadcast(new AddActionTextEvent(m_currentInteractable.CanUseAction1() ? m_currentInteractable.GetAction1Name() : null
                                                                     , m_currentInteractable.CanUseAction2() ? m_currentInteractable.GetAction2Name() : null
                                                                     , gameObject));
        }

        Vector2 interactablePos = m_currentInteractable.transform.position;
        if ((interactablePos - pos).sqrMagnitude > m_interactionRadius * m_interactionRadius)
        {
            m_currentInteractable = null;
            Event<StopActionTextEvent>.Broadcast(new StopActionTextEvent(gameObject));
        }
    }
}
