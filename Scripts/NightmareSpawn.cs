using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareSpawn : StaticSerializedMonoBehaviour<NightmareSpawn>
{
    [SerializeField] private GameObject nightmarePrefab;
    [SerializeField] private Rect Area_L;
    [SerializeField] private Rect Area_R;

    [SerializeField,MinMaxSlider(0f,20f,showFields:true)] private Vector2 spawnTimeRange;

    private List<GameObject> spawned;

    protected override void Awake()
    {
        base.Awake();
        spawned = new List<GameObject>();
    }

    public void StartSpawn()
    {
        StartCoroutine(Cor_SpawnCycle());
    }

    public void AbortSpawn()
    {
        StopAllCoroutines();
    }

    public void ClearSpawned()
    {
        foreach (GameObject sp in spawned)
        {
            Destroy(sp);
        }

        spawned.Clear();
    }

    private IEnumerator Cor_SpawnCycle()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(spawnTimeRange.x, spawnTimeRange.y));
            SpawnSingle();
        }
    }

    [Button]
    public GameObject SpawnSingle()
    {
        bool spawnAtLeft = Random.value > 0.5f;
        GameObject spawn;
        Vector3 spawnPos;

        if(spawnAtLeft)
        {
            spawnPos = new Vector3(Random.Range(Area_L.xMin, Area_L.xMax), Random.Range(Area_L.yMin, Area_L.yMax),0f);
        }
        else
        {
            spawnPos = new Vector3(Random.Range(Area_R.xMin, Area_R.xMax), Random.Range(Area_R.yMin, Area_R.yMax), 0f);
        }

        spawn = Instantiate(nightmarePrefab, transform.position + spawnPos, Quaternion.identity);
        spawned.Add(spawn);

        return spawn;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 vec = new Vector2(transform.position.x, transform.position.y);
        Gizmos.DrawWireCube(vec + Area_L.position, Area_L.size);
        Gizmos.DrawWireCube(vec + Area_R.position, Area_R.size);
    }
}
