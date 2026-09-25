using UnityEngine;

public class obstacle : MonoBehaviour
{
    private Rigidbody2D rb;
    private float rotationVelocity;

    [Header("Destruição pelas Paredes")]
    public float tempoAposPassarParede = 3f;
    public float limiteX = 9.5f;
    public float limiteY = 5.5f;

    private bool agendouDestruicao = false;
    private bool entrouNaAreaJogo = false;

    void Start()
    {
        float tamanhometeoro = Random.Range(0.5f, 2f);
        transform.localScale = new Vector3(tamanhometeoro, tamanhometeoro, 1);
        rotationVelocity = Random.Range(-50f, 50f);
        rb = GetComponent<Rigidbody2D>();
        Vector2 direcaoAleatoria = Random.insideUnitCircle;
        rb.AddForce(direcaoAleatoria * 5f, ForceMode2D.Impulse);

        float torqueAleatorio = Random.Range(-10f, 10f);
        rb.AddTorque(torqueAleatorio);

        // Se já nasceu dentro da área da tela, marca como dentro
        if (Mathf.Abs(transform.position.x) < limiteX && Mathf.Abs(transform.position.y) < limiteY)
        {
            entrouNaAreaJogo = true;
        }

        // Tempo de vida máximo de segurança para evitar vazamento de memória
        Destroy(gameObject, 25f);
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationVelocity * Time.deltaTime);

        // Verifica se o meteoro passou pelos limites das paredes
        if (!agendouDestruicao)
        {
            if (!entrouNaAreaJogo)
            {
                // Espera o meteoro entrar na área de jogo caso tenha spawnado fora
                if (Mathf.Abs(transform.position.x) < limiteX && Mathf.Abs(transform.position.y) < limiteY)
                {
                    entrouNaAreaJogo = true;
                }
            }
            else
            {
                // Quando já esteve na tela e agora ultrapassa a linha das paredes
                if (Mathf.Abs(transform.position.x) >= limiteX || Mathf.Abs(transform.position.y) >= limiteY)
                {
                    IniciarAutodestruicao();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VerificarColisaoComParede(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        VerificarColisaoComParede(collision.gameObject);
    }

    private void VerificarColisaoComParede(GameObject obj)
    {
        if (agendouDestruicao) return;

        string nome = obj.name.ToLower();
        bool isParede = nome.Contains("parede") ||
                        nome.Contains("chao") ||
                        nome.Contains("borda") ||
                        obj.layer == LayerMask.NameToLayer("Parede") ||
                        obj.CompareTag("Parede");

        if (isParede)
        {
            IniciarAutodestruicao();
        }
    }

    private void IniciarAutodestruicao()
    {
        if (!agendouDestruicao)
        {
            agendouDestruicao = true;
            Destroy(gameObject, tempoAposPassarParede);
        }
    }
}
