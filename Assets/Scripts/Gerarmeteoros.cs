using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] SpawnItens;
    public float SpawnTime;
    public float SpawnDelay;

    [Header("Area de spawn (Linha Horiozontal")]
    public float minX = -8f;
    public float maxX = 8f;
    public float spawnY = 6f;

    void Start()
    {
        InvokeRepeating("SpawnRandom", SpawnTime, SpawnDelay);
    }

    void SpawnRandom()
    {
        // Só gera meteoros se o jogo tiver sido iniciado e ainda estiver em andamento
        if (GameManager.Instance != null && !GameManager.Instance.EstaJogando())
        {
            return;
        }

        int random = Random.Range(0, SpawnItens.Length);
        Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), spawnY, 0f);
        GameObject novoItem = Instantiate(SpawnItens[random], spawnPosition, Quaternion.identity);
        Rigidbody2D rbItem = novoItem.GetComponent<Rigidbody2D>();
        if(rbItem != null)
        {
            float randomXForce = Random.Range(-1f, 1f);
            Vector2 direcaoQueda = new Vector2(randomXForce, -5f).normalized;

            rbItem.linearVelocity = direcaoQueda * Random.Range(3f, 6f);
        }
    }
}
