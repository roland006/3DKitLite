using UnityEngine;

public class CoinHUD : MonoBehaviour
{
    public static CoinHUD Instance;

    public GameObject iconImage;
    public Transform row;

    private void Awake()
    {
        Instance = this;
        if (iconImage != null)
        {
            iconImage.SetActive(false);
        }

    }

    public void Add(int amount)
    {
        if(iconImage == null || row == null) 
            return;

        for(int i = 0; i < amount; i++)
        {
            GameObject icon = Instantiate(iconImage, row);
            icon.SetActive(true);
        }
    }
}
