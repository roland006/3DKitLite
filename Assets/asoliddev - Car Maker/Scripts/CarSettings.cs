using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basic car settings for CarControler
/// </summary>
[System.Serializable]
public class CarSettings
{
    /// <summary>
    /// mass of the car rigidbody
    /// </summary>
    public float mass = 1500;

    /// <summary>
    /// drag of the car rigidbody
    /// </summary>
    public float drag = 0.05f;

    /// <summary>
    /// center of the mass of the car rigidbody
    /// </summary>
    public Vector3 centerOfMass = new Vector3(0, -1.0f, 0);

    /// <summary>
    /// motorTorque of the car (speed and acceleration)
    /// </summary>
    public float motorTorque = 1200;

    /// <summary>
    /// steering angle of the car
    /// </summary>
    public float steeringAngle = 50;
}
