using UnityEngine;


public class UpgradeActivatorView : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Teleport teleport))
        {
            teleport.ActivateUpgrade();
            Destroy(gameObject);
        }
    }
}
