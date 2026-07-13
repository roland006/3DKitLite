using UnityEngine;
using Cinemachine;

public class RestoreCameraPriorities : MonoBehaviour
{
    public CinemachineFreeLook freeLook1; // перетащи сюда основную камеру от 3-го лица
    public CinemachineFreeLook freeLook2; // если есть вторая (например, для прицеливания)
    public int normalPriority = 10;

    public void Restore()
    {
        if (freeLook1 != null) freeLook1.Priority = normalPriority;
        if (freeLook2 != null) freeLook2.Priority = normalPriority;
    }
}