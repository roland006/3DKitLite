using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.SetActive(false);
            Destroy(gameObject);
        }
    }
}