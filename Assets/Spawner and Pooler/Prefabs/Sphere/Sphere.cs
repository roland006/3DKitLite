using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Sphere : MonoBehaviour
{
    float timer = 4;
    bool shoot = false;

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
        timer = 4;
        shoot = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        var dir = transform.position - Camera.main.transform.position;
        rb.AddForce(Camera.main.transform.forward * 2000, ForceMode.Force);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            //GameObject is Deactivated but still remains in the pool
            gameObject.SetActive(false);
        }
    }
}