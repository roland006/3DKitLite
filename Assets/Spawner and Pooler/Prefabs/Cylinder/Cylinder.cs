using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Cylinder : MonoBehaviour
{
    Rigidbody rb;

    /// <summary>
    /// Write the Function to Perform When this GameObject is Set to Active from the Pool
    /// </summary>
    void OnEnable()
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
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.MovePosition(transform.position + Vector3.right * Time.deltaTime * 10);
    }
}
