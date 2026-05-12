using System.Collections.Generic;
using UnityEngine;

public class WaveSpawnerSave : ScriptableObject
{
    public class Spawner
    {
        [Tooltip("Tag of the this Object. Tag must be present in Object Pooler")] public string tagOfObject;
        [Tooltip("Spawn Percentage of this Object with respect to other Objects on this List")] [Range(0, 100)] public float spawnPercentage;

        [HideInInspector] public float normalizedPercentage;
        [HideInInspector] public bool setParent;
        [HideInInspector] public GameObject parent;
        [HideInInspector] public bool canExpand;
        [HideInInspector] public bool poolAtStart = true;
    }

    public class WaveSetting
    {
        [Tooltip("Total Number of Waves")] [HideInInspector] public int totalWaveCount;
        [Tooltip("Number of Objects per Wave")] [HideInInspector] public int maxCountObjects;
        [Tooltip("Time Between Instantiating two Objects (in sec)")] [HideInInspector] public float timeBetweenSpawn;
        [Tooltip("Time Between each Wave (in sec)")] [HideInInspector] public float timeBetweenWaves;

        [Tooltip("List for storing Spawner Elements")] [HideInInspector] [SerializeField] public List<Spawner> spawn;
        public enum WaveType
        {
            limited = 0,
            endless = 1
        }
        [HideInInspector] public WaveType waveType;

        void OnValidate()
        {
            timeBetweenWaves = Mathf.Clamp(timeBetweenWaves, maxCountObjects * timeBetweenSpawn, Mathf.Infinity);
        }
    }


    #region Members
    [HideInInspector] public int maxCountObject;
    [HideInInspector] public int totalWaveCount;
    [HideInInspector] public float timeBetweenWaves;
    [HideInInspector] public float timeBetweenSpawn;
    [TextAreaAttribute] [Tooltip("Add Description")] public string description;

    [HideInInspector] public List<string> tagOfObject;
    [HideInInspector] public List<float> spawnPercentage;

    public int XTest;

    #endregion

    #region Validate
    public int MaxCountObject { get => maxCountObject; set => maxCountObject = value; }
    public int TotalWaveCount { get => totalWaveCount; set => totalWaveCount = value; }
    public float TimeBetweenWaves { get => timeBetweenWaves; set => timeBetweenWaves = value; }
    public float TimeBetweenSpawn { get => timeBetweenSpawn; set => timeBetweenSpawn = value; }
    #endregion

    public void Display()
    {
        Debug.Log(MaxCountObject + " : " + TotalWaveCount + " : " + TimeBetweenSpawn + " : " + TimeBetweenWaves);
    }

}
