using TMPro;
using UnityEngine;

public class CoinCounter1 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private int totalCoins = 8;

    private int collectedCoins = 0;

    private void Start()
    {
        UpdateText();
    }

    public void AddCoin()
    {
        collectedCoins++;
        UpdateText();
    }

    private void UpdateText()
    {
        counterText.text =
            "Coins: " + collectedCoins + " / " + totalCoins;
    }
}