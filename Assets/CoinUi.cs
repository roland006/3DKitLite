using UnityEngine;
using TMPro;

public class CoinUi : MonoBehaviour
{   
    public TMP_Text text;

    void Start()
    {
        text.SetText("Coins = 0");
    }

    public void RefreshText(int CoinCount)
    {
        text.SetText("Coins = " + CoinCount);
    }

}
