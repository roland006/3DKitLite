using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Indiemount.Pooler;

namespace Indiemount.Spawner
{
    public class WaveSpawner : MonoBehaviour
    {

        [System.Serializable]
        public class Spawner
        {
            [Tooltip("Tag of the this Object. Tag must be present in Object Pooler")] public string tagOfObject;
            [Tooltip("Spawn Percentage of this Object with respect to other Objects on this List")] [Range(0,10)]public float spawnPercentage;

            public float normalizedPercentage;
        }

        [System.Serializable] [ExecuteInEditMode]
        public class WaveSetting
        {
            [Tooltip("Total Number of Waves")] public int totalWaveCount;
            [Tooltip("Number of Objects per Wave")] public int maxCountObjects;
            [Tooltip("Time Between Spawning two Objects (in sec)")] public float timeBetweenSpawn;
            [Tooltip("Time Between each Wave (in sec)")] public float timeBetweenWaves;

            [Tooltip("List for storing Spawner Elements")] public List<Spawner> spawn;
            public enum WaveType
            {
                limited = 0,
                endless = 1
            }
            [Tooltip("List for storing Spawner Elements")] public WaveType waveType;
        }

        #region Members
        ObjectPooler objPooler;
        List<Coroutine> waveCoroutines;
        float[] total;

        [HeaderAttribute("Wave Settings")]
        [Tooltip("List for storing Spawner Elements")] [HideInInspector] [SerializeField] public List<Spawner> spawn;
        [Tooltip("List for storing Spawner Elements")] [HideInInspector] [SerializeField] List<WaveSetting> waveSettings;
        #endregion
 
        #region Singleton
        public static WaveSpawner instance;
        public static WaveSpawner Instance { get => instance; }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            objPooler = ObjectPooler.Instance;
            total = new float[waveSettings.Count];
            waveCoroutines = new List<Coroutine>();
        }
        #endregion

        void Start()
        {
            Normalize();
            //StartSpawning();
        }

        public void StartSpawning()
        {
            for (int i = 0; i < waveSettings.Count; i++)
                waveCoroutines.Add(StartCoroutine(WaveSpawnerCo(i)));
        }
        
        public void StartSpawning(int waveSettingsIndex)
        {
            waveCoroutines.Add(StartCoroutine(WaveSpawnerCo(waveSettingsIndex)));
        }
        
        public void StopSpawning()
        {
            for (int i = 0; i < waveCoroutines.Count; i++)
                StopCoroutine(waveCoroutines[i]);
        }

        /// <summary>
        /// Function to find the Percentage of the each individual Pool in the Spawner
        /// </summary>
        void Normalize()
        {
            int i = 0;
            foreach (var item in waveSettings)
            {
                foreach (var value in waveSettings[i].spawn)
                    total[i] += value.spawnPercentage;
                i++;
            }

            i = 0;
            foreach (var item in waveSettings)
            {
                float tempSum = 0;
                foreach (var value in waveSettings[i].spawn)
                {
                    tempSum += value.spawnPercentage / total[i];
                    value.normalizedPercentage = tempSum;
                }
                i++;
            }
        }

        /// <summary>
        /// Coroutine Function for Spawing Waves of GameObject from the Object Pooler
        /// </summary>
        IEnumerator WaveSpawnerCo(int k)
        {
            int select = 0;
            int x = 0;
            for (int i = 0; i < waveSettings[k].totalWaveCount; i++)
            {
                bool condition = waveSettings[k].waveType == WaveSetting.WaveType.endless ? true : x < waveSettings[k].maxCountObjects;
                while (condition)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    int z = waveSettings[k].spawn.Count - 1;
                    foreach (var item in waveSettings[k].spawn)
                    {
                        if (waveSettings[k].spawn[z].normalizedPercentage > rand)
                            select = z;
                        z--;
                    }
                    var go = objPooler.SpawnFromPool(waveSettings[k].spawn[select].tagOfObject, Vector3.zero, Quaternion.identity);
                    yield return new WaitForSeconds(waveSettings[k].timeBetweenSpawn);
                    x++;
                }
                yield return new WaitForSeconds(waveSettings[k].timeBetweenWaves);
            }
        }
    }
}