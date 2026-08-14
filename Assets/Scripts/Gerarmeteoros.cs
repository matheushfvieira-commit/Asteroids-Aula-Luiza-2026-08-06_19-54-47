using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] SpawnItens;
    public float SpawnTime;
    public float SpawnDelay;

    void Start()
    {
        InvokeRepeating("SpawnRandom", SpawnTime, SpawnDelay);
    }

    void SpawnRandom()
    {
        int random = Random.Range(0, SpawnItens.Length);
        Instantiate(SpawnItens[random], transform.position, transform.rotation);
    }
}
