using Gamekit2D;
using UnityEngine;


public class MirrorAttackPoint : MonoBehaviour
{
    [SerializeField] private Damager _damager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Damageable damageable))
        {
            damageable.TakeDamage(_damager);
        }
    }
}
