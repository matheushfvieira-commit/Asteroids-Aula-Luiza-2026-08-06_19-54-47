using UnityEngine;

public class Tiro : MonoBehaviour
{
    [Header("Configurações do Tiro")]
    public float velocidade = 15f;
    public float tempoDeVida = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Se houver Rigidbody2D, aplica velocidade na direção apontada
        if (rb != null)
        {
            rb.linearVelocity = transform.up * velocidade;
        }

        // Destrói o projétil após o tempo de vida configurado para economizar memória
        Destroy(gameObject, tempoDeVida);
    }

    void Update()
    {
        // Caso não tenha Rigidbody2D, movimenta usando Transform
        if (rb == null)
        {
            transform.Translate(Vector3.up * velocidade * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHit(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    private void HandleHit(GameObject hitObject)
    {
        // Ignora a própria nave do jogador e outros tiros
        if (hitObject.GetComponent<Players>() != null || hitObject.GetComponent<Tiro>() != null)
        {
            return;
        }

        // Verifica se atingiu um meteoro/obstáculo
        obstacle meteoro = hitObject.GetComponent<obstacle>();
        bool isMeteoro = meteoro != null ||
                         hitObject.layer == LayerMask.NameToLayer("Meteoro") ||
                         hitObject.CompareTag("Meteoro") ||
                         hitObject.name.ToLower().Contains("meteoro");

        if (isMeteoro)
        {
            Destroy(hitObject);
            Destroy(gameObject);
            return;
        }

        // Se atingir paredes, chão ou bordas
        if (hitObject.name.ToLower().Contains("parede") ||
            hitObject.name.ToLower().Contains("chao") ||
            hitObject.name.ToLower().Contains("borda"))
        {
            Destroy(gameObject);
        }
    }
}
