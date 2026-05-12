using UnityEngine;
using Indiemount.Pooler; //This namespace should be used to implement Object Pooler Class

//This script is attached to a Empty gameobject which is also the spawn point for the bullets 
public class BulletSpawner : MonoBehaviour
{
    Transform spawnPoint;

    private void Start()
    {
        spawnPoint = gameObject.transform.GetChild(1).transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // When Left Mouse Button is Pressed
        {
            //Accessing a GameObject From the Pool "Sphere"
            ObjectPooler.Instance.SpawnFromPool("Sphere", spawnPoint.position, Quaternion.identity);  
        }
    }
}