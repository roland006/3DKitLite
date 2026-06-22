using UnityEngine;

public class FinishGame : MonoBehaviour
{
    public GameObject blackScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            blackScreen.SetActive(true);
            Time.timeScale = 0f; 
        }
    }
}