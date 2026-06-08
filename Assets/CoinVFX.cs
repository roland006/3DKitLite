using UnityEngine;

public class CoinVFX : MonoBehaviour
{
   
  
    public GameObject VFX_PickupSparkles;

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log(other);
        if (other.CompareTag("Player"))
        {
           
            Instantiate(VFX_PickupSparkles, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
    
}
