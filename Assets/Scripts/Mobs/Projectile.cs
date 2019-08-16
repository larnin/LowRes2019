using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
