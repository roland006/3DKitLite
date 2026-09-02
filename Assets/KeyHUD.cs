using UnityEngine;

public class KeyHUD : MonoBehaviour
{
    public static KeyHUD Instance;

    public GameObject icon;

    public bool HasKey { get; private set; }

    private void Awake()
    {
        Instance = this;
        if (icon != null)
        {
            icon.SetActive(false);
        }
        HasKey = false;
    }

    public void Collect()
    {
        HasKey = true;
        if (icon != null)
        {
            icon.SetActive(true);
        }
    }

    public void Consume()
    {
        HasKey = false;
        if (icon != null)
        {
            icon.SetActive(false);
        }
    }
}