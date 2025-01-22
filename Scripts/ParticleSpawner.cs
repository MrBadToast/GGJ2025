using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : StaticSerializedMonoBehaviour<ParticleSpawner>
{
    [SerializeField] private float spawnInterval = 0f;

    float spawnTimer = 0f;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
    }

    public void TrySpawn(GameObject particleObj, Vector3 postition)
    {
        if(spawnTimer > spawnInterval)
        {
            spawnTimer = 0f;
            Instantiate(particleObj, postition,Quaternion.identity);
        }
    }
}
