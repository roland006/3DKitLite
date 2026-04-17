using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 1f;
    public float rotateSpeed = 45f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {

        float offset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + Vector3.up * offset;

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }
}
