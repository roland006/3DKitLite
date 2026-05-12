using UnityEngine;
using UnityEngine.UI;

public class ShapeSpawnerSave : ScriptableObject
{
    #region Members

    [HideInInspector] public GameObject spawnPrefab;
    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public Vector3 spacing;
    [HideInInspector] public int layoutInt;
    [HideInInspector] public int axisInt;
    [HideInInspector] public bool setParent;
    [HideInInspector] public GameObject parent;
    [TextAreaAttribute] [Tooltip("Add Description")] public string description;
    
    public enum Layout
    {
        rectangle = 0,
        triangle = 1,
        rightAngleTriangle = 2
    }

    public enum Axis
    {
        XY = 0,
        XZ = 1,
        XYZ = 2
    }

    [HideInInspector] public int rows = 1;
    [HideInInspector] public int columns = 1;
    [HideInInspector] public int height = 1;

    #endregion

    #region Validate
    public GameObject SpawnPrefab { get => spawnPrefab; set => spawnPrefab = value; }
    public Vector3 StartPosition { get => startPosition; set => startPosition = value; }
    public Vector3 Spacing { get => spacing; set => spacing = value; }
    public bool SetParent{ get => setParent; set => setParent = value; }
    public GameObject Parent{ get => parent; set => parent = value; }
    public int LayoutInt { get => layoutInt; set => layoutInt = Mathf.Clamp(value, 0, (int)Layout.rightAngleTriangle); }
    public int AxisInt { get => axisInt; set => axisInt = Mathf.Clamp(value, 0, (int)Axis.XYZ); }
    public int Rows { get => rows; set => rows = Mathf.Max(1, value); }
    public int Columns { get => columns; set => columns = Mathf.Max(1, value); }
    public int Height { get => height; set => height = Mathf.Max(1, value); }
    #endregion

    public void Display()
    {
        Debug.Log(spawnPrefab +":"+ startPosition + ":" + spacing + ":" + layoutInt + ":" + axisInt + ":" + rows + ":" + columns + ":" + height);
    }
}