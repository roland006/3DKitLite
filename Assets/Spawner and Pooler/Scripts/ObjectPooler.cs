using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Indiemount.Pooler
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            [Tooltip("Tag for this Pool of GameObjects")] [HideInInspector] public string tag = "New Item";
            [Tooltip("Prefab to Spawn")] [HideInInspector] public GameObject prefab;
            [Tooltip("Total number of GameObjects in the Pool")] [HideInInspector] public int maxCount = 5;
            [Tooltip("Check if you want to Spawn GameObject as a Child of a another GameObject")] [HideInInspector] public bool setParent;
            [Tooltip("Set Parent GameObject")] [HideInInspector] public GameObject parent;
            [Tooltip("Check if you want to Expand the Pool of this GameObject at Runtime")] [HideInInspector] public bool canExpand;
            [Tooltip("Select the Pooling Method")] [HideInInspector] public poolUsing poolingMethod = poolUsing.withoutTags;
        
            public enum poolUsing
            {
                [Tooltip("Only the GameObjects active in the scene with the same Tag will be added to the pool")] tags = 0,
                [Tooltip("It Generates the pool at start")] withoutTags = 1 
            }
        }

        #region Public Members
        [HideInInspector] public List<Pool> objectPools;
        public Dictionary<string, List<GameObject>> poolDictionary;
        public Dictionary<string, Pool> poolDictionaryPools;
        #endregion

        #region Singleton
        public static ObjectPooler instance;
        public static ObjectPooler Instance { get => instance; }
        
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        void Start()
        {
            poolDictionary = new Dictionary<string, List<GameObject>>();
            poolDictionaryPools = new Dictionary<string, Pool>();
            int x = 0;
            foreach (Pool pool in objectPools)
            {
                List<GameObject> objects = new List<GameObject>();
                
                if ((int)pool.poolingMethod == 0)
                {
                    objects.AddRange(GameObject.FindGameObjectsWithTag(pool.tag));
                    print(objects.Count + " Number of GameObjects added to the Pool " + pool.tag);
                    if (pool.maxCount <= objects.Count)
                    {
                        pool.maxCount = objects.Count;
                    } 
                }

                for (int i = objects.Count; i < pool.maxCount; i++)
                {
                        GameObject obj = Instantiate(pool.prefab);
                        if (pool.setParent)
                            obj.transform.parent = pool.parent.transform;

                        obj.SetActive(false);
                        objects.Add(obj);
                }

                poolDictionary.Add(pool.tag, objects);
                poolDictionaryPools.Add(pool.tag, objectPools[x]);
                x++;
            }
        }

        /// <summary>
        /// Returns a GameObject from the Pool
        /// </summary>
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool With Tag: " + tag + " doesn't exsist.");
                return null;
            }

            int index = 0;
            foreach (GameObject go in poolDictionary[tag])
            {
                if (!go.activeSelf)
                {
                    GameObject objectToSpawn = poolDictionary[tag][index];
                    objectToSpawn.transform.position = position;
                    objectToSpawn.transform.rotation = rotation;
                    objectToSpawn.SetActive(true);

                    return objectToSpawn;
                }
                index++;
            }
            GameObject goExpanded = ExpandPool(tag, position, rotation);
            return goExpanded;
        }

        /// <summary>
        /// Returns a GameObject from the Pool if bool canExpand is True
        /// </summary>
        GameObject ExpandPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (poolDictionaryPools[tag].canExpand)
            {
                GameObject obj = (GameObject)Instantiate(poolDictionaryPools[tag].prefab);
                if (poolDictionaryPools[tag].setParent)
                    obj.transform.parent = poolDictionaryPools[tag].parent.transform; 

                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                poolDictionary[tag].Add(obj);
                return obj;
            }
            else
            {
                Debug.LogError("No Deactive object in the pool " + tag);
                return null;
            }
        }
    }
}