using TMPro;
using UnityEngine;

public class CoinUi : MonoBehaviour
{
    public TextMeshProUGUI CurrentCoinCounterText;
    public TextMeshProUGUI MaxCoinCounterText;
    public int MaxCoinsOnLevel;
    public CounterCoins Counter;
    int LastCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaxCoinCounterText.text = MaxCoinsOnLevel.ToString(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (LastCount != Counter.Coins)
        { 
        LastCount = Counter.Coins;
            CurrentCoinCounterText.text = LastCount.ToString();
        }
    }
}