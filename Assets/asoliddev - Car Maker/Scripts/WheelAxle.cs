using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings of visuals and physics of a left and right wheel.
/// </summary>
[System.Serializable]
public class WheelAxle
{
    /// <summary>
    /// Left WheelCollider
    /// </summary>
    public WheelCollider wheelColliderLeft;

    /// <summary>
    /// Right WheelCollider
    /// </summary>
    public WheelCollider wheelColliderRight;

    /// <summary>
    /// Left Wheel Mesh
    /// </summary>
    public GameObject wheelMeshLeft;

    /// <summary>
    /// Right Wheel Mesh
    /// </summary>
    public GameObject wheelMeshRight;

    /// <summary>
    /// Is motor torque applyed to this axle
    /// </summary>
    public bool motor;

    /// <summary>
    /// Is this is a stearing axle
    /// </summary>
    public bool steering;
}

