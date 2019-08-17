using UnityEngine;
using System.Collections;

public class Pushable : MonoBehaviour
{
    Rigidbody2D m_rigidbody;
    
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        var velocity = m_rigidbody.velocity;
        velocity.x = 0;
        m_rigidbody.velocity = velocity;
    }
}
