using UnityEngine;
using UnityEngine.SceneManagement;

public class FaseManager : MonoBehaviour
{
    public static FaseManager Instance;

    [Header("Estados e Configurações")]
    public PrologoState estadoAtual;
    public int totalCientistas = 3;
    
    [Tooltip("Escreva aqui o nome EXATO da cena que deve carregar ao terminar.")]
    public string nomeProximaCena = "Fase2"; // <--- NOVO CAMPO

    [Header("Configuração de Mensagens (Sistema)")]
    [Tooltip("Mensagem quando o player tenta usar a máquina SEM falar com os cientistas.")]
    [TextArea(2, 5)]
    public string msgFaltamCientistas = "SISTEMA: Acesso Negado. Converse com os cientistas para obter as coordenadas.";

    [Tooltip("Mensagem quando o player resolveu o puzzle mas precisa falar com os cientistas DE NOVO.")]
    [TextArea(2, 5)]
    public string msgFaltamCientistasPosPuzzle = "SISTEMA: Calibração concluída. Fale com a equipe para confirmar a viagem.";

    [Header("Referências")]
    public NarratorController narrator;
    public GameObject puzzlePanel;
    public DialogueUI2D dialogueUI;

    // Contadores internos
    private int cientistasConversados = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Garante que o painel comece fechado
        if (puzzlePanel != null) puzzlePanel.SetActive(false);

        // Correção do Warning: Busca a UI se estiver vazia
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI2D>();

        // Configuração inicial do Estado
        estadoAtual = PrologoState.Inicio;

        if (narrator != null)
        {
            narrator.TocarIntro(() =>
            {
                estadoAtual = PrologoState.ConversarCientistas;
                Debug.Log("Intro finalizada. Player livre.");
            });
        }
        else
        {
            // Fallback caso não tenha narrador
            estadoAtual = PrologoState.ConversarCientistas;
        }
    }

    public void RegistrarConversaCientista()
    {
        if (estadoAtual == PrologoState.ConversarCientistas || estadoAtual == PrologoState.PuzzleResolvido)
        {
            cientistasConversados++;
            Debug.Log($"Progresso: {cientistasConversados}/{totalCientistas}");

            if (cientistasConversados >= totalCientistas)
            {
                AvancarProximaEtapa();
            }
        }
    }

    private void AvancarProximaEtapa()
    {
        cientistasConversados = 0; 

        if (estadoAtual == PrologoState.ConversarCientistas)
        {
            estadoAtual = PrologoState.PodeResolverPuzzle;
            Debug.Log("FaseManager: Máquina Liberada para conserto!");
        }
        else if (estadoAtual == PrologoState.PuzzleResolvido)
        {
            estadoAtual = PrologoState.ProntoParaViajar;
            Debug.Log("FaseManager: Pronto para viagem!");
        }
    }

    public void PuzzleFinalizado()
    {
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        
        estadoAtual = PrologoState.PuzzleResolvido;
        Debug.Log("FaseManager: Puzzle resolvido. Reiniciando NPCs.");

        // Correção do Warning: Busca lista de NPCs de forma otimizada
        var npcs = FindObjectsByType<NPCDialogue2D>(FindObjectsSortMode.None);
        foreach (var npc in npcs)
        {
            npc.ResetarConversaParaNovaEtapa();
        }
    }

    public void TentarUsarMaquinaTempo()
    {
        switch (estadoAtual)
        {
            case PrologoState.ConversarCientistas:
                MostrarMensagemSistema(msgFaltamCientistas);
                break;

            case PrologoState.PodeResolverPuzzle:
                if (puzzlePanel != null) puzzlePanel.SetActive(true);
                break;

            case PrologoState.PuzzleResolvido:
                MostrarMensagemSistema(msgFaltamCientistasPosPuzzle);
                break;

            case PrologoState.ProntoParaViajar:
                IniciarFinalDaFase();
                break;
        }
    }

    private void MostrarMensagemSistema(string mensagem)
    {
        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue("", mensagem, null);
        }
        else
        {
            Debug.Log($"Mensagem de Sistema: {mensagem}");
        }
    }

    private void IniciarFinalDaFase()
    {
        // AQUI ESTÁ A MUDANÇA: Usamos a variável nomeProximaCena
        if (narrator != null)
        {
            narrator.TocarFinal(() => SceneManager.LoadScene(nomeProximaCena));
        }
        else
        {
            SceneManager.LoadScene(nomeProximaCena);
        }
    }
}