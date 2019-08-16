using UnityEngine;
using System.Collections;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] float m_darkDuration = 5;
    [SerializeField] float m_lightRecovery = 2.5f;
    [SerializeField] LayerMask m_deadlyLayer;
    [SerializeField] GameObject m_deathPrefab = null;
    [SerializeField] float m_deathTime = 1;
    [SerializeField] float m_shakePower = 1;

    float m_darkTime = 0;
    bool m_saturatedDarkness = false;

    private void Update()
    {
        if(LightSystem.instance.IsOnLight(transform))
            m_darkTime = Mathf.Max(m_darkTime - Time.deltaTime * m_lightRecovery, 0);
        else
        {
            m_darkTime += Time.deltaTime;
            if (m_darkTime > m_darkDuration && !m_saturatedDarkness)
                OnDarknessSaturation();
        }

        if (m_darkTime > 0)
            CameraShaker.instance.ShakeThisFrame(m_darkTime / m_darkDuration * m_shakePower);

        Event<UpdateDarknessInsanityEvent>.Broadcast(new UpdateDarknessInsanityEvent(m_darkTime / m_darkDuration));
    }

    void OnDarknessSaturation()
    {
        if (m_saturatedDarkness)
            return;

        m_saturatedDarkness = true;

        bool isFacingRight = GetComponent<PlayerControler>().IsFacingRight();

        var obj = Instantiate(m_deathPrefab);
        obj.transform.position = transform.position;
        if (!isFacingRight)
            obj.transform.localScale = new Vector3(-1, 1, 1);
        Destroy(obj, m_deathTime);
        Destroy(gameObject);

        Event<PlayerDeathEvent>.Broadcast(new PlayerDeathEvent());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var layer = collision.gameObject.layer;
        if (m_deadlyLayer.value == (m_deadlyLayer.value | (1 << layer)))
            OnDarknessSaturation();
    }
}
