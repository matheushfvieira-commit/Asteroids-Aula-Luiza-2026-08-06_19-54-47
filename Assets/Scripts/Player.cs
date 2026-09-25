using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class Players : MonoBehaviour
{
    public Animator animator;

    public float Thurst = 5f;
    public Rigidbody2D rb;
    public float maxSpeed = 10f;
    public LineRenderer lineRenderer;

    [Header("Sistema de Tiro")]
    public GameObject tiroPrefab;
    public Transform pontoDeDisparo;
    public float fireRate = 0.25f;
    public float tiroVelocidade = 15f;
    private float nextFireTime = 0f;

    [Header("Sistema de Vida e Morte")]
    public int vidas = 1;
    public float tempoParaReiniciar = 5f;
    private bool estaMorto = false;
    private float tempoRestante = 5f;

    void Start()
    {
        animator  = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void HandleMovment()
    {
        if (estaMorto) return;
        if (GameManager.Instance != null && !GameManager.Instance.EstaJogando()) return;

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            Vector2 direction = (mouseWorldPos - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            rb.angularVelocity = 0f;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            rb.AddForce(transform.up * Thurst);
            rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);

            if (animator != null)
                animator.SetBool("isMoving", true);
        }
        else if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }  
    }

    private void HandleShooting()
    {
        if (estaMorto) return;
        if (GameManager.Instance != null && !GameManager.Instance.EstaJogando()) return;

        bool shootRequested = false;

        // Disparo via Barra de Espaço
        if (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.spaceKey.isPressed))
        {
            shootRequested = true;
        }

        // Disparo via Botão Direito do Mouse
        if (Mouse.current != null && (Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.rightButton.isPressed))
        {
            shootRequested = true;
        }

        if (shootRequested && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Atirar();
        }
    }

    private void Atirar()
    {
        // Se houver um ponto de disparo configurado, usa ele; senão, calcula na ponta da frente da nave
        Vector3 spawnPos = pontoDeDisparo != null ? pontoDeDisparo.position : transform.position + transform.up * 0.85f;
        Quaternion spawnRot = transform.rotation;

        if (tiroPrefab != null)
        {
            GameObject tiro = Instantiate(tiroPrefab, spawnPos, spawnRot);
            Tiro tiroComp = tiro.GetComponent<Tiro>();
            if (tiroComp != null)
            {
                tiroComp.velocidade = tiroVelocidade;
            }
        }
        else
        {
            CriarTiroDinamico(spawnPos, spawnRot);
        }
    }

    private void CriarTiroDinamico(Vector3 posicao, Quaternion rotacao)
    {
        GameObject tiroObj = new GameObject("Tiro");
        tiroObj.transform.position = posicao;
        tiroObj.transform.rotation = rotacao;

        SpriteRenderer sr = tiroObj.AddComponent<SpriteRenderer>();
        Texture2D tex = new Texture2D(4, 12);
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                tex.SetPixel(x, y, new Color(1f, 0.9f, 0.2f, 1f));
            }
        }
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);

        CapsuleCollider2D col = tiroObj.AddComponent<CapsuleCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.2f, 0.6f);

        Rigidbody2D rbTiro = tiroObj.AddComponent<Rigidbody2D>();
        rbTiro.gravityScale = 0f;

        Tiro scriptTiro = tiroObj.AddComponent<Tiro>();
        scriptTiro.velocidade = tiroVelocidade;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        VerificarColisaoMeteoro(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VerificarColisaoMeteoro(collision.gameObject);
    }

    private void VerificarColisaoMeteoro(GameObject outro)
    {
        if (estaMorto) return;
        if (GameManager.Instance != null && GameManager.Instance.JaGanhou()) return;

        bool isMeteoro = outro.GetComponent<obstacle>() != null ||
                         outro.layer == LayerMask.NameToLayer("Meteoro") ||
                         outro.CompareTag("Meteoro") ||
                         outro.name.ToLower().Contains("meteoro");

        if (isMeteoro)
        {
            // Destrói o meteoro ao encostar na nave
            Destroy(outro);
            Morrer();
        }
    }

    public void Morrer()
    {
        if (estaMorto) return;
        if (GameManager.Instance != null && GameManager.Instance.JaGanhou()) return;

        vidas--;
        if (vidas <= 0)
        {
            estaMorto = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Derrota();
            }
            tempoRestante = tempoParaReiniciar;

            // Desativa visual da nave
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;

            // Desativa os colisores da nave
            Collider2D[] colisores = GetComponents<Collider2D>();
            foreach (var col in colisores)
            {
                col.enabled = false;
            }

            // Para qualquer movimento físico
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }

            // Inicia contagem regressiva para recomeçar o jogo
            StartCoroutine(ContagemRegressivaMorte());
        }
    }

    private IEnumerator ContagemRegressivaMorte()
    {
        while (tempoRestante > 0f)
        {
            yield return null;
            tempoRestante -= Time.deltaTime;
        }

        tempoRestante = 0f;
        // Recarrega a cena automaticamente
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnGUI()
    {
        if (!estaMorto) return;

        // Fundo escuro semi-transparente cobrindo a tela
        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Estilo do título "VOCÊ MORREU!"
        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.label);
        estiloTitulo.fontSize = Mathf.Clamp(Screen.width / 18, 32, 64);
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.MiddleCenter;
        estiloTitulo.normal.textColor = new Color(1f, 0.2f, 0.2f);

        // Estilo do texto com timer regressivo
        GUIStyle estiloTimer = new GUIStyle(GUI.skin.label);
        estiloTimer.fontSize = Mathf.Clamp(Screen.width / 32, 18, 30);
        estiloTimer.alignment = TextAnchor.MiddleCenter;
        estiloTimer.normal.textColor = Color.white;

        float centroX = Screen.width / 2f;
        float centroY = Screen.height / 2f;

        GUI.Label(new Rect(centroX - 350, centroY - 60, 700, 70), "VOCÊ MORREU!", estiloTitulo);
        int segundos = Mathf.CeilToInt(tempoRestante);
        GUI.Label(new Rect(centroX - 350, centroY + 15, 700, 45), $"Reiniciando em {segundos} segundos...", estiloTimer);
    }

    void Update()
    {
        if (estaMorto) return;
        if (GameManager.Instance != null && !GameManager.Instance.EstaJogando()) return;

        HandleMovment();
        HandleShooting();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 20f);
        Vector3 endPoint = hit.collider != null ? hit.point : transform.position + transform.up * 20f;
    }
}