using UnityEngine;

public class CoinPickup1 : MonoBehaviour
{
    [SerializeField] private CoinCounter1 coinCounter;
    [SerializeField] private ParticleSystem pickupEffect;
    [SerializeField] private AudioClip pickupSound;
    private void OnTriggerEnter(Collider enteredCollider)
    {
        Debug.Log(
            "Entered: " + enteredCollider.gameObject.name +
            ", layer: " + LayerMask.LayerToName(enteredCollider.gameObject.layer)
        );

        if (enteredCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Coin collected");
            coinCounter.AddCoin();
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, 0.7f);
            Destroy(gameObject);
        }
    }
}