using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Capsule : MonoBehaviour
{
    Rigidbody _rb;

    void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        transform.position = new Vector3(Random.Range(-40,-10), 5, Random.Range(0, -30));
        _rb.AddForce(Vector3.up * 500);
    }

    /// <summary>
    /// Write the Function to Perform When this GameObject is Deactivated from the Pool 
    /// Mostly used to reset values
    /// </summary>
    private void OnDisable()
    {
        //Your Code
    }
}
