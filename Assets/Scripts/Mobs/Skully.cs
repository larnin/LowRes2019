using UnityEngine;
using System.Collections;

public class Skully : MonoBehaviour
{
    [SerializeField] float m_seeDistance = 80;
    [SerializeField] BoxCollider2D m_allowedArea = null;
    [SerializeField] float m_moveSpeed = 5;
    [SerializeField] LayerMask m_wallLayer;

    Animator m_animator;
    Rigidbody2D m_rigidbody;
    Bounds m_allowedZone;
    Transform m_spriteTransform;

    void Start()
    {
        m_spriteTransform = transform.Find("Sprite");
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        if(m_allowedArea != null)
            m_allowedZone = m_allowedArea.bounds;
    }

    private void FixedUpdate()
    {
        bool seePlayer = false;

        Vector2 playerPos = Vector2.zero;
        Vector2 pos = transform.position;
        float dist = 0;

        if (PlayerControler.instance != null)
        {
            playerPos = PlayerControler.instance.transform.position;
            dist = (playerPos - pos).magnitude;

            if (dist <= m_seeDistance && Physics2D.Raycast(transform.position, (playerPos - pos) / dist, dist, m_wallLayer.value).collider == null)
                seePlayer = true;
        }

        if(seePlayer && m_allowedArea != null)
        {
            Vector2 min = m_allowedZone.min; 
            Vector2 max = m_allowedZone.max;

            if (playerPos.x < min.x || playerPos.y < min.y || playerPos.x > max.x || playerPos.y > max.y)
                seePlayer = false;
        }

        bool isOnLight = LightSystem.instance.IsOnLight(transform);

        m_animator.SetBool("Moving", seePlayer && !isOnLight);
        m_animator.SetBool("Scared", isOnLight);

        if (seePlayer && !isOnLight)
        {
            m_rigidbody.velocity = (playerPos - pos) / dist * m_moveSpeed;

            if (Mathf.Abs(m_rigidbody.velocity.x) > 0.1f)
                m_spriteTransform.localScale = new Vector3(m_rigidbody.velocity.x > 0 ? 1 : -1, 1, 1);
        }
        else m_rigidbody.velocity = Vector2.zero;
    }
}
