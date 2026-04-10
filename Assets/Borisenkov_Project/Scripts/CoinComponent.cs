using UnityEngine;

public class CoinComponent : MonoBehaviour
{
    public CharacterController Ellen;
    public CoinCounter Counter;

    public float heightAmplitude = 1;
    public float moveSpeed = 1;
    bool isUp;

    Vector3 startPos;
    Vector3 upPos;

    public Vector3 rotationSpeed = new Vector3(0, 50, 0);
    

    private void Start()
    {
        startPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y + moveSpeed, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == Ellen)
        {
            Counter.Coins++;
            Debug.LogError(Counter.Coins);
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);

        if (isUp == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPos, moveSpeed * Time.deltaTime);

            if (transform.position.y >= upPos.y)
            {
                isUp = false;
            }
        }
        else if (isUp == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);

            if (transform.position.y <= startPos.y)
            {
                isUp = true;
            }
        }
    }

}
