using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemyPrefab;
   
    public void SpawnMob()
       {
           Instantiate(enemyPrefab, transform);
       }
}
