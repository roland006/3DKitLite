using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    /// <summary>
    /// Write the Function to Perform When this GameObject is Set to Active from the Pool
    /// </summary>
    private void OnEnable()
    {
        OnObjectSpawn();
    }

    /// <summary>
    /// Write the Function to Perform When this GameObject is Deactivated from the Pool 
    /// Mostly used to reset values
    /// </summary>
    private void OnDisable()
    {
        //Your Code
    }

    public void OnObjectSpawn()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 500);
        rb.AddForce(Vector3.left * 100);
        rb.AddForce(Vector3.right * 100);
    }
}