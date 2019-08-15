using UnityEngine;
using System.Collections;

public class TheRock : MonoBehaviour
{
    [SerializeField] float m_speed = 1;
    [SerializeField] float m_checkDistance = 5;
    [SerializeField] float m_cleckGroundHeight = -5;
    [SerializeField] float m_checkRadius = 2;
    [SerializeField] bool m_startGoRight = true;
    [SerializeField] LayerMask m_checkLayer;

    Rigidbody2D m_rigidbody;
    Animator m_animator;
    Transform m_spriteTransform;

    bool m_goRight = false;
    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_spriteTransform = transform.Find("Sprite");
        m_goRight = m_startGoRight;
    }
    
    void Update()
    {
        bool isOnLight = LightSystem.instance.IsOnLight(transform);

        bool moveThisFrame = !isOnLight;

        if(!isOnLight)
        {
            Vector2 pos = transform.position;
            float h = m_checkDistance * (m_goRight ? 1 : -1);

            bool haveWall = Physics2D.OverlapCircle(pos + new Vector2(h, 0), m_checkRadius, m_checkLayer.value) != null;
            bool haveGround = Physics2D.OverlapCircle(pos + new Vector2(h, m_cleckGroundHeight), m_checkRadius, m_checkLayer.value) != null;

            if (haveWall || !haveGround)
            {
                m_goRight = !m_goRight;
                moveThisFrame = false;
            }
        }

        m_animator.SetBool("Moving", moveThisFrame);
        m_spriteTransform.localScale = new Vector3(m_goRight ? 1 : -1, 1, 1);

        float speed = moveThisFrame ? ((m_goRight ? 1 : -1) * m_speed) : 0;

        m_rigidbody.velocity = new Vector2(speed, 0);
    }
}
