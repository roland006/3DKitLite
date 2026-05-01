using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gamekit3D.Message;
using UnityEngine.Serialization;
using System.Runtime.CompilerServices;

public class CoinComponent : MonoBehaviour
{
    public CharacterController Ellen;
    public CounterCoins Counter;
    public GameObject CoinVFX;


    public float moveSpeed = 1;
    public float heightAplitude;

    public Vector3 rotationSpeed = new Vector3(0, 50, 0);
    Vector3 startPos;
    Vector3 upPos;
    bool isUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        upPos = new Vector3(transform.position.x, transform.position.y + heightAplitude, transform.position.z);
    }

    // Update is called once per frame

    void Update()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other == Ellen)
        {
            Counter.Coins++; //

            Debug.LogError(Counter.Coins);
            Instantiate(CoinVFX, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}

