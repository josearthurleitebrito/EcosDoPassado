using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Necessário para o IEnumerator

public class FaseManager : MonoBehaviour
{
    public static FaseManager Instance;

    [Header("Estados e Configurações")]
    public PrologoState estadoAtual;
    public int totalCientistas = 3;
    [Tooltip("Escreva aqui o nome EXATO da cena que deve carregar ao terminar.")]
    public string nomeProximaCena = "Dia1"; // Atualize se necessário

    [Header("Intro da Fase (Capa)")]
    public GameObject painelCapa; // <--- ARRASTE A IMAGEM DO PROLOGO AQUI
    public float tempoCapa = 3f;

    [Header("Configuração de Mensagens (Sistema)")]
    [TextArea(2, 5)]
    public string msgFaltamCientistas = "SISTEMA: Acesso Negado. Converse com os cientistas.";
    [TextArea(2, 5)]
    public string msgFaltamCientistasPosPuzzle = "SISTEMA: Calibração concluída. Fale com a equipe.";

    [Header("Referências")]
    public NarratorController narrator;
    public GameObject puzzlePanel;
    public DialogueUI2D dialogueUI;

    private int cientistasConversados = 0;
    
    // Variável para controlar o timer da mensagem do sistema
    private Coroutine corrotinaMensagem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        // 1. Configuração Inicial (Esconde tudo que não deve aparecer)
        if (puzzlePanel != null) puzzlePanel.SetActive(false);
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI2D>();
        
        // 2. ATIVA A CAPA (Intro)
        if (painelCapa != null) 
        {
            painelCapa.SetActive(true);
            // TRAVA TUDO POR 3 SEGUNDOS
            yield return new WaitForSeconds(tempoCapa);
            // SOME A CAPA
            painelCapa.SetActive(false);
        }

        // 3. INICIA O JOGO REALMENTE
        IniciarLogicaDaFase();
    }

    private void IniciarLogicaDaFase()
    {
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

    // --- MUDANÇA AQUI: Lógica para mostrar e esconder automaticamente ---
    private void MostrarMensagemSistema(string mensagem)
    {
        if (dialogueUI != null)
        {
            // Se já tiver um timer rodando, cancela ele para reiniciar a contagem
            if (corrotinaMensagem != null) StopCoroutine(corrotinaMensagem);

            dialogueUI.ShowDialogue("", mensagem, null);
            
            // Inicia a contagem de 4 segundos
            corrotinaMensagem = StartCoroutine(FecharDialogoRoutine(4.0f));
        }
        else
        {
            Debug.Log($"Mensagem de Sistema: {mensagem}");
        }
    }

    private IEnumerator FecharDialogoRoutine(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        
        if (dialogueUI != null) dialogueUI.HideDialogue();
        
        corrotinaMensagem = null;
    }
    // --------------------------------------------------------------------

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