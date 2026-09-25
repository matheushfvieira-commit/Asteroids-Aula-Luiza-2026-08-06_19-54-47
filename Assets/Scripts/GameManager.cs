using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum EstadoJogo
    {
        Menu,
        Jogando,
        Vitoria,
        Derrota
    }

    [Header("Configurações da Missão")]
    [Tooltip("Tempo em segundos que o jogador precisa sobreviver para chegar a AstroNoids")]
    public float tempoParaVitoria = 60f;

    [Header("Estado Atual")]
    public EstadoJogo estadoAtual = EstadoJogo.Menu;

    private float tempoRestante;

    // Garante que o GameManager seja instanciado automaticamente mesmo se não estiver na cena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInicializar()
    {
        if (Instance == null)
        {
            GameManager existente = Object.FindFirstObjectByType<GameManager>();
            if (existente == null)
            {
                GameObject obj = new GameObject("GameManager");
                obj.AddComponent<GameManager>();
            }
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        tempoRestante = tempoParaVitoria;
    }

    void Update()
    {
        switch (estadoAtual)
        {
            case EstadoJogo.Menu:
                // Permite iniciar apertando Espaço ou Enter
                if (VerificarInputConfirmacao())
                {
                    IniciarJogo();
                }
                break;

            case EstadoJogo.Jogando:
                tempoRestante -= Time.deltaTime;
                if (tempoRestante <= 0f)
                {
                    tempoRestante = 0f;
                    VencerJogo();
                }
                break;

            case EstadoJogo.Vitoria:
                // Permite reiniciar apertando Espaço, Enter ou R
                if (VerificarInputConfirmacao() || VerificarTeclaR())
                {
                    ReiniciarJogo();
                }
                break;

            case EstadoJogo.Derrota:
                // O script Players cuida da contagem de derrota e recarregamento da cena
                break;
        }
    }

    private bool VerificarInputConfirmacao()
    {
        // Verifica se o teclado do novo Input System está conectado
        if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                return true;
            }
        }

        // Fallback para caso esteja usando o Input clássico
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
    }

    private bool VerificarTeclaR()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            return true;
        }
        return Input.GetKeyDown(KeyCode.R);
    }

    public void IniciarJogo()
    {
        estadoAtual = EstadoJogo.Jogando;
        tempoRestante = tempoParaVitoria;
    }

    public void VencerJogo()
    {
        estadoAtual = EstadoJogo.Vitoria;

        // Limpa todos os meteoros existentes na tela para que o jogador comemore em segurança
        obstacle[] meteorosRestantes = Object.FindObjectsByType<obstacle>(FindObjectsSortMode.None);
        foreach (var meteoro in meteorosRestantes)
        {
            if (meteoro != null)
            {
                Destroy(meteoro.gameObject);
            }
        }
    }

    public void Derrota()
    {
        estadoAtual = EstadoJogo.Derrota;
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool EstaJogando()
    {
        return estadoAtual == EstadoJogo.Jogando;
    }

    public bool JaGanhou()
    {
        return estadoAtual == EstadoJogo.Vitoria;
    }

    void OnGUI()
    {
        switch (estadoAtual)
        {
            case EstadoJogo.Menu:
                DesenharMenuInicial();
                break;

            case EstadoJogo.Jogando:
                DesenharHUDJogando();
                break;

            case EstadoJogo.Vitoria:
                DesenharTelaVitoria();
                break;

            case EstadoJogo.Derrota:
                // O script Players já desenha a tela de derrota perfeitamente!
                break;
        }
    }

    private void DesenharMenuInicial()
    {
        // Fundo escuro semi-transparente
        GUI.color = new Color(0f, 0f, 0.05f, 0.85f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float centroX = Screen.width / 2f;
        float centroY = Screen.height / 2f;

        // Estilo do Título Principal
        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.label);
        estiloTitulo.fontSize = Mathf.Clamp(Screen.width / 16, 36, 68);
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.MiddleCenter;
        estiloTitulo.normal.textColor = new Color(0.2f, 0.85f, 1f); // Ciano espacial

        // Estilo do Subtítulo / Destino
        GUIStyle estiloSubtitulo = new GUIStyle(GUI.skin.label);
        estiloSubtitulo.fontSize = Mathf.Clamp(Screen.width / 32, 18, 28);
        estiloSubtitulo.fontStyle = FontStyle.Bold;
        estiloSubtitulo.alignment = TextAnchor.MiddleCenter;
        estiloSubtitulo.normal.textColor = new Color(1f, 0.85f, 0.3f); // Dourado

        // Estilo da Missão e Instruções
        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = Mathf.Clamp(Screen.width / 45, 14, 20);
        estiloTexto.alignment = TextAnchor.MiddleCenter;
        estiloTexto.normal.textColor = Color.white;

        // Estilo dos Controles
        GUIStyle estiloControles = new GUIStyle(GUI.skin.box);
        estiloControles.fontSize = Mathf.Clamp(Screen.width / 50, 13, 17);
        estiloControles.alignment = TextAnchor.MiddleCenter;
        estiloControles.normal.textColor = new Color(0.85f, 0.95f, 1f);

        // Estilo do Botão Começar
        GUIStyle estiloBotao = new GUIStyle(GUI.skin.button);
        estiloBotao.fontSize = Mathf.Clamp(Screen.width / 35, 18, 28);
        estiloBotao.fontStyle = FontStyle.Bold;
        estiloBotao.alignment = TextAnchor.MiddleCenter;

        // Renderização dos elementos
        GUI.Label(new Rect(centroX - 350, centroY - 210, 700, 70), "ASTEROIDS", estiloTitulo);
        GUI.Label(new Rect(centroX - 350, centroY - 145, 700, 40), "🚀 Missão: Viagem a AstroNoids 🚀", estiloSubtitulo);

        string textoMissao = "Piloto, atravesse a tempestade de asteroides e guie sua nave até o planeta AstroNoids!\n" +
                             $"Sobreviva por {Mathf.CeilToInt(tempoParaVitoria)} segundos sem colidir para alcançar o destino.";
        GUI.Label(new Rect(centroX - 400, centroY - 95, 800, 50), textoMissao, estiloTexto);

        string instrucoesControles = "CONTROLES:\n" +
                                     "🖱️ Mouse Esquerdo: Direcionar e Acelerar a nave\n" +
                                     "⌨️ Barra de Espaço ou Botão Direito: Disparar tiros";
        GUI.Box(new Rect(centroX - 250, centroY - 35, 500, 80), instrucoesControles, estiloControles);

        // Botão para Começar
        if (GUI.Button(new Rect(centroX - 160, centroY + 65, 320, 55), "INICIAR MISSÃO", estiloBotao))
        {
            IniciarJogo();
        }

        GUIStyle estiloDica = new GUIStyle(GUI.skin.label);
        estiloDica.fontSize = 13;
        estiloDica.alignment = TextAnchor.MiddleCenter;
        estiloDica.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        GUI.Label(new Rect(centroX - 200, centroY + 125, 400, 25), "(Você também pode apertar Barra de Espaço ou Enter)", estiloDica);
    }

    private void DesenharHUDJogando()
    {
        // Faixa escura no topo da tela
        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.DrawTexture(new Rect(Screen.width / 2f - 240, 10, 480, 50), Texture2D.whiteTexture);
        GUI.color = Color.white;

        GUIStyle estiloHUD = new GUIStyle(GUI.skin.label);
        estiloHUD.fontSize = Mathf.Clamp(Screen.width / 45, 14, 20);
        estiloHUD.fontStyle = FontStyle.Bold;
        estiloHUD.alignment = TextAnchor.MiddleCenter;
        estiloHUD.normal.textColor = Color.white;

        int segundos = Mathf.CeilToInt(tempoRestante);
        GUI.Label(new Rect(Screen.width / 2f - 240, 12, 480, 26), $"🪐 Destino: AstroNoids  |  ⏳ Tempo: {segundos}s", estiloHUD);

        // Barra de progresso da viagem
        float larguraBarraTotal = 420f;
        float progresso = 1f - (tempoRestante / tempoParaVitoria);
        progresso = Mathf.Clamp01(progresso);

        float barX = Screen.width / 2f - (larguraBarraTotal / 2f);
        float barY = 40f;

        // Fundo da barra
        GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        GUI.DrawTexture(new Rect(barX, barY, larguraBarraTotal, 12), Texture2D.whiteTexture);

        // Preenchimento do progresso (Verde/Ciano)
        GUI.color = Color.Lerp(new Color(1f, 0.6f, 0.1f), new Color(0.1f, 1f, 0.5f), progresso);
        GUI.DrawTexture(new Rect(barX, barY, larguraBarraTotal * progresso, 12), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DesenharTelaVitoria()
    {
        // Fundo escuro com leve brilho azul-esverdeado de vitória
        GUI.color = new Color(0.02f, 0.1f, 0.08f, 0.88f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float centroX = Screen.width / 2f;
        float centroY = Screen.height / 2f;

        // Estilo do Título "MISSÃO CUMPRIDA!"
        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.label);
        estiloTitulo.fontSize = Mathf.Clamp(Screen.width / 16, 36, 64);
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.MiddleCenter;
        estiloTitulo.normal.textColor = new Color(0.2f, 1f, 0.4f); // Verde comemorativo

        // Estilo do Subtítulo "Você chegou a AstroNoids"
        GUIStyle estiloPlaneta = new GUIStyle(GUI.skin.label);
        estiloPlaneta.fontSize = Mathf.Clamp(Screen.width / 26, 22, 36);
        estiloPlaneta.fontStyle = FontStyle.Bold;
        estiloPlaneta.alignment = TextAnchor.MiddleCenter;
        estiloPlaneta.normal.textColor = new Color(1f, 0.85f, 0.2f); // Ouro brilhante

        // Estilo do Texto de Parabéns
        GUIStyle estiloMensagem = new GUIStyle(GUI.skin.label);
        estiloMensagem.fontSize = Mathf.Clamp(Screen.width / 42, 15, 22);
        estiloMensagem.alignment = TextAnchor.MiddleCenter;
        estiloMensagem.normal.textColor = Color.white;

        // Estilo do Botão
        GUIStyle estiloBotao = new GUIStyle(GUI.skin.button);
        estiloBotao.fontSize = Mathf.Clamp(Screen.width / 35, 18, 28);
        estiloBotao.fontStyle = FontStyle.Bold;
        estiloBotao.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(centroX - 400, centroY - 170, 800, 65), "🎉 MISSÃO CUMPRIDA! 🎉", estiloTitulo);
        GUI.Label(new Rect(centroX - 400, centroY - 100, 800, 50), "🪐 VOCÊ CHEGOU A ASTRONOIDS! 🪐", estiloPlaneta);

        string textoVitoria = "Parabéns, Comandante! Você sobreviveu à chuva de asteroides\n" +
                              "e pousou sua nave em segurança na superfície de AstroNoids.";
        GUI.Label(new Rect(centroX - 400, centroY - 40, 800, 55), textoVitoria, estiloMensagem);

        if (GUI.Button(new Rect(centroX - 160, centroY + 45, 320, 55), "JOGAR NOVAMENTE", estiloBotao))
        {
            ReiniciarJogo();
        }

        GUIStyle estiloDica = new GUIStyle(GUI.skin.label);
        estiloDica.fontSize = 13;
        estiloDica.alignment = TextAnchor.MiddleCenter;
        estiloDica.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
        GUI.Label(new Rect(centroX - 200, centroY + 110, 400, 25), "(Aperte Espaço, Enter ou R para reiniciar)", estiloDica);
    }
}
