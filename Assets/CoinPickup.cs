using Gamekit3D;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;
    public Vector3 speed = new Vector3(0f, 90f, 0f);
    public float bobHeight = 0.15f;
    public float bobSpeed = 2f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.Rotate(speed * Time.deltaTime, Space.World);

        float delta = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.position = startPos + Vector3.up * delta;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        if(CoinHUD.Instance != null)
            CoinHUD.Instance.Add(value);

        Destroy(gameObject);
    }


}
