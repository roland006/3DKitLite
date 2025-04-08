using TMPro;
using UnityEngine;

public class CoinCount : MonoBehaviour
{
    [HideInInspector]
    public int coin = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coin = 0;
    }

    public void AddCoin()
    {
        coin = coin + 1;

        var Canvas = GameObject.Find("CoinCanvas");
        CoinUi UI = Canvas.GetComponent<CoinUi>();
        UI.RefreshText(coin);
    }
}
