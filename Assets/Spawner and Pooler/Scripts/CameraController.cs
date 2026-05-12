using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float turnSpeed = 2;

    float x = 0.0f;
    float y = 0.0f;

    private void Update()
    {
        MouseAim();    
    }

    void MouseAim()
    {
        y += turnSpeed * Input.GetAxis("Mouse X");
        x += turnSpeed * Input.GetAxis("Mouse Y");

        y = Mathf.Clamp(y, -45,45);
        x = Mathf.Clamp(x, -30, 50);
        
        transform.eulerAngles = new Vector3(x, y, 0);
    }
}
