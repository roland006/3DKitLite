using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI CurrentCoinCounterText;
    public TextMeshProUGUI MaxCoinCounterText;
    public int MaxCoinsOnLevel;
    public CoinCounter Counter;
    int LastCount;

    void Start()
    {
        MaxCoinCounterText.text = MaxCoinsOnLevel.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (LastCount < Counter.Coins)
        {
            LastCount = Counter.Coins;
            CurrentCoinCounterText.text=LastCount.ToString();
        }
    }
}
