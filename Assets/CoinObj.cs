using UnityEngine;
using UnityEngine.Serialization;

public class CoinObj : MonoBehaviour
{
    public int rotateSpeed = 1;
    Vector3 startPos;
    Vector3 upPos;

    bool isUp;

    void Start()
    {        
       Debug.LogError("Метод старт");

        isUp = true;

        startPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);


    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("TriggerEntered");       
        Destroy(gameObject);
        
    }

    private void Update()
    {
        transform.Rotate(new Vector3(rotateSpeed, 0, 0) * Time.deltaTime);

        if (isUp == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPos, 1 * Time.deltaTime);
            Debug.LogError("Вверх");

            if (transform.position.y >= upPos.y)
            {
                isUp = false;
            }
        }
        else if(isUp == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, 1 * Time.deltaTime);
            Debug.LogError("Вниз");
            if (transform.position.y <= startPos.y)
            {
                isUp = true;
            }
        }
    }
}
