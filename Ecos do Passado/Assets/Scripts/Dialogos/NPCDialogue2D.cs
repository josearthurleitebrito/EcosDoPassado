using UnityEngine;

public class NPCDialogue2D : MonoBehaviour
{
    [Header("Configuração de Identificação")]
    public string npcName = "Cientista";
    
    [Tooltip("Se marcado, este NPC conta para o progresso do puzzle.")]
    public bool isRequiredForPuzzle = true; 

    [Header("Diálogos da História")]
    [TextArea(2, 4)]
    [Tooltip("O que ele diz quando você chega no laboratório.")]
    public string[] dialogoAntesDoPuzzle;

    [TextArea(2, 4)]
    [Tooltip("O que ele diz DEPOIS que você resolveu o puzzle.")]
    public string[] dialogoDepoisDoPuzzle;
    
    [Header("Diálogos Genéricos (Opcional)")]
    [TextArea(1, 2)]
    [Tooltip("Frase curta se o player falar com ele de novo sem o estado ter mudado.")]
    public string[] dialogoRepetido;

    // Variável interna para o array que será usado no momento
    private string[] sentencesParaUsar; 

    [Header("Referências")]
    public DialogueUI2D dialogueUI;
    public PlayerController playerController;

    private int index = 0;
    private bool isTalking = false;
    
    // Controle para saber se já contou para o progresso na etapa atual
    private bool jaConversouNessaEtapa = false;

    void Start()
    {
        // Auto-assign para evitar NullReferenceException
        if (dialogueUI == null)
            dialogueUI = FindObjectOfType<DialogueUI2D>();
        
        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        // Inscreva-se no evento de reset (se houver) ou deixe o Manager controlar
    }

    // Método chamado pelo seu script 'PlayerInteract' (Raycast ou Trigger)
    public void Interact()
    {
        if (isTalking)
        {
            // Se já está falando, o botão de interação serve para AVANÇAR o texto
            NextSentence();
        }
        else
        {
            // Se não está falando, define o texto correto e COMEÇA
            DefinirDialogoCorreto();
            StartDialogue();
        }
    }

    private void DefinirDialogoCorreto()
    {
        // Se não tiver gerenciador, usa o padrão (antes do puzzle)
        if (FaseManager.Instance == null)
        {
            sentencesParaUsar = dialogoAntesDoPuzzle;
            return;
        }

        PrologoState estado = FaseManager.Instance.estadoAtual;

        // Lógica de seleção de texto baseada no estado do FaseManager
        if (estado == PrologoState.Inicio || estado == PrologoState.ConversarCientistas)
        {
            // Se já falou, pode mandar um texto genérico ou repetir o mesmo
            sentencesParaUsar = jaConversouNessaEtapa && dialogoRepetido.Length > 0 ? 
                                dialogoRepetido : dialogoAntesDoPuzzle;
        }
        else if (estado == PrologoState.PodeResolverPuzzle)
        {
            sentencesParaUsar = new string[] { "O que você está esperando? Vá consertar o painel!" };
        }
        else if (estado == PrologoState.PuzzleResolvido || estado == PrologoState.ProntoParaViajar)
        {
             sentencesParaUsar = jaConversouNessaEtapa && dialogoRepetido.Length > 0 ? 
                                dialogoRepetido : dialogoDepoisDoPuzzle;
        }
        else
        {
            sentencesParaUsar = dialogoAntesDoPuzzle; // Fallback
        }
    }

    public void StartDialogue()
    {
        if (sentencesParaUsar == null || sentencesParaUsar.Length == 0) return;

        isTalking = true;
        index = 0;

        // Trava o player
        if (playerController != null)
        {
            playerController.enabled = false;
            // Se tiver animação de Idle, force aqui
            playerController.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        dialogueUI.ShowDialogue(npcName, sentencesParaUsar[index], this);
    }

    public void NextSentence()
    {
        index++;
        if (index < sentencesParaUsar.Length)
        {
            dialogueUI.UpdateText(sentencesParaUsar[index]);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        isTalking = false;

        if (dialogueUI != null)
            dialogueUI.HideDialogue();

        if (playerController != null)
            playerController.enabled = true;

        // --- LÓGICA DE PROGRESSO ---
        if (FaseManager.Instance != null && isRequiredForPuzzle)
        {
            // Verifica se estamos numa etapa que requer contagem
            bool etapaRequerConversa = (FaseManager.Instance.estadoAtual == PrologoState.ConversarCientistas || 
                                        FaseManager.Instance.estadoAtual == PrologoState.PuzzleResolvido);

            if (etapaRequerConversa && !jaConversouNessaEtapa)
            {
                FaseManager.Instance.RegistrarConversaCientista();
                jaConversouNessaEtapa = true; // Marca que já falou nesta rodada
            }
        }
    }

    // Chamado pelo FaseManager quando o puzzle é resolvido para permitir falar de novo
    public void ResetarConversaParaNovaEtapa()
    {
        jaConversouNessaEtapa = false;
    }
}