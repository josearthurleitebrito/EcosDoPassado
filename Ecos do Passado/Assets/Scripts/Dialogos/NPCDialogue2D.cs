using UnityEngine;

public class NPCDialogue2D : MonoBehaviour
{
    [Header("Configuração de Identificação")]
    public string npcName = "Cientista";
    
    [Tooltip("Se marcado, este NPC conta para o progresso do puzzle (Usado no Prólogo).")]
    public bool isRequiredForPuzzle = true; 

    [Header("Diálogos Padrão")]
    [TextArea(2, 4)]
    [Tooltip("O que ele diz quando você chega (Padrão).")]
    public string[] dialogoAntesDoPuzzle;

    [Header("Prólogo (Fase 0)")]
    [TextArea(2, 4)]
    [Tooltip("O que ele diz DEPOIS que você resolveu o puzzle dos fios.")]
    public string[] dialogoDepoisDoPuzzle;
    
    [TextArea(1, 2)]
    public string[] dialogoRepetido;

    [Header("Fase 2 (Acampamento)")]
    [Tooltip("O que eles dizem DEPOIS que você tirou a pólvora.")]
    [TextArea(2, 4)]
    public string[] dialogoDia2Final; // <--- ESSE ERA O CAMPO QUE FALTAVA

    // Variável interna para o array que será usado no momento
    private string[] sentencesParaUsar; 

    [Header("Referências")]
    public DialogueUI2D dialogueUI;
    public PlayerController playerController;

    private int index = 0;
    private bool isTalking = false;
    private bool jaConversouNessaEtapa = false;

    void Start()
    {
        if (dialogueUI == null) dialogueUI = FindFirstObjectByType<DialogueUI2D>();
        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();
    }

    public void Interact()
    {
        if (isTalking)
        {
            NextSentence();
        }
        else
        {
            DefinirDialogoCorreto();
            StartDialogue();
        }
    }

    private void DefinirDialogoCorreto()
    {
        // --- LÓGICA DA FASE 2 (ACAMPAMENTO) ---
        if (Dia2Manager.Instance != null)
        {
            // Se já desarmou as bombas ou venceu
            if (Dia2Manager.Instance.estadoAtual == Dia2State.RelatarEquipe || 
                Dia2Manager.Instance.estadoAtual == Dia2State.Vitoria)
            {
                if (dialogoDia2Final != null && dialogoDia2Final.Length > 0)
                    sentencesParaUsar = dialogoDia2Final;
                else
                    sentencesParaUsar = dialogoAntesDoPuzzle;
            }
            else
            {
                sentencesParaUsar = dialogoAntesDoPuzzle;
            }
            return; // Sai do método, já decidiu
        }

        // --- LÓGICA DO PRÓLOGO (LABORATÓRIO) ---
        if (FaseManager.Instance != null)
        {
            PrologoState estado = FaseManager.Instance.estadoAtual;

            if (estado == PrologoState.PodeResolverPuzzle)
            {
                sentencesParaUsar = new string[] { "Vá consertar o painel primeiro!" };
            }
            else if (estado == PrologoState.PuzzleResolvido || estado == PrologoState.ProntoParaViajar)
            {
                 sentencesParaUsar = jaConversouNessaEtapa && dialogoRepetido.Length > 0 ? 
                                    dialogoRepetido : dialogoDepoisDoPuzzle;
            }
            else
            {
                // Inicio ou ConversarCientistas
                sentencesParaUsar = jaConversouNessaEtapa && dialogoRepetido.Length > 0 ? 
                                    dialogoRepetido : dialogoAntesDoPuzzle;
            }
            return; // Sai do método
        }

        // --- LÓGICA DO DIA 1 (PORTO) OU PADRÃO ---
        sentencesParaUsar = dialogoAntesDoPuzzle;
    }

    public void StartDialogue()
    {
        if (sentencesParaUsar == null || sentencesParaUsar.Length == 0) return;

        isTalking = true;
        index = 0;

        if (playerController != null)
        {
            playerController.enabled = false;
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
        if (dialogueUI != null) dialogueUI.HideDialogue();
        if (playerController != null) playerController.enabled = true;

        // --- AVISOS AOS GERENTES ---

        // Avisa Fase 2
        if (Dia2Manager.Instance != null)
        {
            Dia2Manager.Instance.RegistrarConversaNPC(npcName);
        }

        // Avisa Dia 1
        if (Dia1Manager.Instance != null)
        {
            Dia1Manager.Instance.RegistrarConversaNPC(npcName);
        }

        // Avisa Prólogo (Com lógica de contagem)
        if (FaseManager.Instance != null && isRequiredForPuzzle)
        {
            bool etapaRequerConversa = (FaseManager.Instance.estadoAtual == PrologoState.ConversarCientistas || 
                                        FaseManager.Instance.estadoAtual == PrologoState.PuzzleResolvido);

            if (etapaRequerConversa && !jaConversouNessaEtapa)
            {
                FaseManager.Instance.RegistrarConversaCientista();
                jaConversouNessaEtapa = true; 
            }
        }
    }

    public void ResetarConversaParaNovaEtapa()
    {
        jaConversouNessaEtapa = false;
    }
}