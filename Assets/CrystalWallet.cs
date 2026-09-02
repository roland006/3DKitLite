using TMPro;
using UnityEngine;

public class CrystalWallet : MonoBehaviour
{
    [SerializeField] private TMP_Text crystalText;

    public int Crystals { get; private set; }

    private void Start()
    {
        UpdateUI();
    }

    public void AddCrystals(int amount)
    {
        Crystals += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (crystalText != null)
        {
            crystalText.text = Crystals.ToString("00");
        }
    }
}