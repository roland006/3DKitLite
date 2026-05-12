using UnityEngine;
using Indiemount.Spawner; //This namespace should be used to implement Object Pooler Class

public class GameManager : MonoBehaviour
{
    void Start()
    {
        //Starting a Wave using the element index
        WaveSpawner.Instance.StartSpawning(0);       
        WaveSpawner.Instance.StartSpawning(1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Stopping all Waves
            WaveSpawner.Instance.StopSpawning();
        }
    }
}