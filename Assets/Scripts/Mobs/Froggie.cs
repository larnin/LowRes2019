using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Froggie : MonoBehaviour
{
    [SerializeField] GameObject m_projectilePrefab = null;
    [SerializeField] float m_projectileLife = 1;
    [SerializeField] float m_projectileSpeed = 1;
    [SerializeField] float m_jumpWait = 2;
    [SerializeField] float m_jumpWaitOnLight = 0;
    [SerializeField] float m_jumpDistance = 8;
    [SerializeField] float m_maxDeltaHeight = 8;
    [SerializeField] bool m_startSeeRight = true;
    [SerializeField] LayerMask m_groundMask;
    [SerializeField] float m_groundOffset = 2;
    [SerializeField] float m_shootDistance = 8;
    [SerializeField] float m_shootDelay = 1;
    [SerializeField] BoxCollider2D m_allowedArea = null;

    float m_jumpDelay = 0;
    bool m_Jumping = false;
    bool m_seeRight = true;

    Animator m_animator;
    Bounds m_allowedZone;
    Transform m_spriteTransform;
    Rigidbody2D m_rigidbody;

    bool m_onLight = false;

    void Start()
    {
        m_spriteTransform = transform.Find("Sprite");
        m_seeRight = m_startSeeRight;
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();

        if (m_allowedArea != null)
            m_allowedZone = m_allowedArea.bounds;
    }
    
    void Update()
    {
        m_rigidbody.velocity = Vector2.zero;

        if (m_Jumping)
            return;

        m_onLight = LightSystem.instance.IsOnLight(transform);
        bool targetValid = false;
        Vector2 targetPos = Vector2.zero;

        Vector2 min = m_allowedZone.min;
        Vector2 max = m_allowedZone.max;

        if(m_onLight)
        {
            var target = LightSystem.instance.GetNearestLight(transform);
            targetPos = target.position;

            if (m_allowedArea == null || (targetPos.x > min.x && targetPos.y > min.y && targetPos.y < max.x && targetPos.y < max.y))
            {
                m_seeRight = targetPos.x > transform.position.x;
                targetValid = true;
            }
        }

        float maxDelay = m_onLight ? m_jumpWaitOnLight : m_jumpWait;

        m_jumpDelay += Time.deltaTime;
        if (m_jumpDelay < maxDelay)
            return;

        float dir = m_seeRight ? 1 : -1;

        bool haveWall = Physics2D.Raycast(transform.position, new Vector2(dir, 0), m_jumpDistance + 1, m_groundMask.value).collider != null;
        bool canGoUp = false;
        if(haveWall)
        {
            canGoUp = Physics2D.Raycast(transform.position, new Vector2(0, 1), m_maxDeltaHeight, m_groundMask.value).collider == null;

            if (canGoUp)
                canGoUp = Physics2D.Raycast(transform.position + new Vector3(0, m_maxDeltaHeight), new Vector2(dir, 0), m_jumpDistance + 1, m_groundMask.value).collider == null;
        }
        Vector2 startPos = transform.position + new Vector3(dir * m_jumpDistance, 0);
        if (haveWall)
            startPos += new Vector2(0, m_maxDeltaHeight);
        bool canGoDown = false;
        Vector2 point = Vector2.zero;
        if(!haveWall || canGoUp)
        {
            var c = Physics2D.Raycast(startPos, new Vector2(0, -1), m_maxDeltaHeight * 2, m_groundMask.value);

            canGoDown = c.collider != null;
            if(canGoDown)
            {
                point = c.point + new Vector2(0, m_groundOffset);
            }
        }

        m_spriteTransform.localScale = new Vector3(m_seeRight ? 1 : -1, 1, 1);

        if(targetValid && Mathf.Abs(targetPos.x - transform.position.x) < m_shootDistance)
        {
            Throw(targetPos);
            return;
        }

        if (m_allowedArea != null && (point.x < min.x || point.y < min.y || point.x > max.x || point.y > max.y))
        {
            m_seeRight = !m_seeRight;
            return;
        }

        if (canGoDown)
            Jump(point);
        else  m_seeRight = !m_seeRight;
    }

    void Jump(Vector2 target)
    {
        Vector2 pos = transform.position;

        bool jumping = Mathf.Abs(pos.y - target.y) > 1;

        if (jumping)
            m_animator.SetTrigger("Jump");
        else m_animator.SetTrigger("Move");

        m_Jumping = true;

        if (!jumping)
        {
            transform.DOMove(new Vector3(target.x, target.y, transform.position.z), 0.3f).OnComplete(() =>
            {
                m_Jumping = false;
                m_jumpDelay = (m_onLight ? m_jumpWaitOnLight : m_jumpWait) / 2;
            });
        }
        else
        {
            float h = Mathf.Max(pos.y, target.y) + 8;
            float duration = 0.5f;

            transform.DOMoveY(h, duration / 2).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                transform.DOMoveY(target.y, duration / 2).SetEase(Ease.InQuad);
            });

            transform.DOMoveX(target.x, duration).OnComplete(() =>
            {
                m_Jumping = false;
                m_jumpDelay = 0;
            });
        }
    }

    void Throw(Vector2 target)
    {
        m_animator.SetTrigger("Shoot");

        m_Jumping = true;

        DOVirtual.DelayedCall(m_shootDelay, () => { m_Jumping = false; });

        DOVirtual.DelayedCall(0.2f, () =>
        {
            if (this == null)
                return;

            if (m_projectilePrefab == null)
            {
                Debug.LogError("Can't shoot a null projectile");
                return;
            }

            var obj = Instantiate(m_projectilePrefab);
            obj.transform.position = transform.position + new Vector3(0, 0, -0.1f);
            var rigidbody = obj.GetComponent<Rigidbody2D>();
            var dir = target - new Vector2(transform.position.x, transform.position.y);
            rigidbody.velocity = dir.normalized * m_projectileSpeed;
            obj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            Destroy(obj, m_projectileLife);
        });
    }
}
