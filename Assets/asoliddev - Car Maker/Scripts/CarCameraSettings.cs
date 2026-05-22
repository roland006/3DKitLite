using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by CarCamera to setup up different styled camera views.
/// </summary>
[System.Serializable]
public class CarCameraSettings
{
    /// <summary>
    /// How far the camera is behind the car.
    /// </summary>
    public float distance = 13.0f;

    /// <summary>
    /// How far the camera is above the car.
    /// </summary>
    public float height = 8.0f;

    /// <summary>
    /// Smoothing transition time of the Camera.
    /// </summary>
    public float smoothTime = 0.3F;


    /// <summary>
    /// A default settings of a camera.
    /// </summary>
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings0() {
        CarCameraSettings carCameraSettings = new CarCameraSettings();
        carCameraSettings.distance = 13.0f;
        carCameraSettings.height = 8.0f;
        carCameraSettings.smoothTime = 0.3f;

        return carCameraSettings;
    }

    /// <summary>
    /// A default settings of a camera.
    /// </summary>
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings1()
    {
        CarCameraSettings carCameraSettings = new CarCameraSettings();
        carCameraSettings.distance = 20.0f;
        carCameraSettings.height = 15.0f;
        carCameraSettings.smoothTime = 0.3f;

        return carCameraSettings;
    }

    /// <summary>
    /// A default settings of a camera.
    /// </summary>
    /// <returns></returns>
    public static CarCameraSettings GetDefaultSettings2()
    {
        CarCameraSettings carCameraSettings = new CarCameraSettings();
        carCameraSettings.distance = 6.0f;
        carCameraSettings.height = 3.0f;
        carCameraSettings.smoothTime = 0.3f;

        return carCameraSettings;
    }
}
