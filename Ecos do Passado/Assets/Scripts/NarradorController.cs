using UnityEngine;
using System; // Necessário para usar Actions

public class NarratorController : MonoBehaviour
{
    [Header("Configuração das Falas")]
    public string nomeNarrador = "???"; // Pode ser vazio se não quiser nome
    
    [TextArea(2, 5)]
    public string[] falasIntro; // Falas antes de começar a andar

    [TextArea(2, 5)]
    public string[] falasFinal; // Falas antes de encerrar a fase

    [Header("Referências")]
    public DialogueUI2D dialogueUI;
    public PlayerController playerController;

    // Estado interno
    private string[] falasAtuais;
    private int index = 0;
    private bool isNarrating = false;
    private Action onDialogueFinished; // O que fazer quando acabar?

    void Start()
    {
        if (dialogueUI == null) dialogueUI = FindObjectOfType<DialogueUI2D>();
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // Se está narrando, espera Input para avançar (Mouse ou Espaço)
        if (isNarrating && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E)))
        {
            AvancarFala();
        }
    }

    // Chamado pelo FaseManager para iniciar a Intro
    public void TocarIntro(Action aoTerminar)
    {
        IniciarNarracao(falasIntro, aoTerminar);
    }

    // Chamado pelo FaseManager para iniciar o Final
    public void TocarFinal(Action aoTerminar)
    {
        IniciarNarracao(falasFinal, aoTerminar);
    }

    private void IniciarNarracao(string[] falas, Action aoTerminar)
    {
        if (falas == null || falas.Length == 0)
        {
            aoTerminar?.Invoke(); // Se não tiver fala, executa a ação imediatamente
            return;
        }

        falasAtuais = falas;
        onDialogueFinished = aoTerminar;
        index = 0;
        isNarrating = true;

        // Trava o Player
        if (playerController != null) 
        {
            playerController.enabled = false;
            // Zera a velocidade para ele não deslizar
            playerController.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero; 
        }

        // Mostra a primeira fala (Passamos 'null' no 3º parametro pois o Narrador não é um NPC físico)
        // OBS: Se seu DialogueUI exigir um NPC, precisaremos adaptar. 
        // Assumindo que ShowDialogue(string nome, string texto, NPCDialogue2D npc) aceita null.
        dialogueUI.ShowDialogue(nomeNarrador, falasAtuais[index], null); 
    }

    private void AvancarFala()
    {
        index++;
        if (index < falasAtuais.Length)
        {
            dialogueUI.UpdateText(falasAtuais[index]);
        }
        else
        {
            FinalizarNarracao();
        }
    }

    private void FinalizarNarracao()
    {
        isNarrating = false;
        dialogueUI.HideDialogue();

        // Destrava o Player (O FaseManager pode travar de novo se for transição de cena, mas por padrão liberamos)
        if (playerController != null) 
            playerController.enabled = true;

        // Executa o código que o FaseManager pediu (Ex: Mudar estado ou Carregar Cena)
        onDialogueFinished?.Invoke(); 
    }
}