using Gamekit3D;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        if(KeyHUD.Instance == null)
            return;

        if (KeyHUD.Instance.HasKey)
            return;

        KeyHUD.Instance.Collect();
        Destroy(gameObject);    
    }
}
