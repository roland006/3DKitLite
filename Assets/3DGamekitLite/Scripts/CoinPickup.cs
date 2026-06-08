using UnityEngine;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
   
    public TextMeshProUGUI uiText;
   

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log(other);
        if (other.CompareTag("Player"))
        {
            int current = int.Parse(uiText.text);
            uiText.text = (current + 1).ToString();

            Destroy(gameObject);
        }
    }

}
