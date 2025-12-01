using UnityEngine;

public class NPCDialogue2D : MonoBehaviour
{
    [Header("Informacoes do NPC")]
    public string npcName = "Lucas";
    [TextArea(2, 6)]
    public string[] sentences;

    public DialogueUI2D dialogueUI;
    public PlayerController playerController;

    private int index = 0;
    private bool isTalking = false;

    void Start()
    {
        if (dialogueUI == null)
            dialogueUI = FindObjectOfType<DialogueUI2D>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
    }

    public void StartDialogue()
    {
        if (isTalking || sentences.Length == 0) return;

        isTalking = true;
        index = 0;

        if (playerController != null)
            playerController.enabled = false;

        dialogueUI.ShowDialogue(npcName, sentences[index], this);
    }

    public void NextSentence()
    {
        index++;

        if (index < sentences.Length)
        {
            dialogueUI.UpdateText(sentences[index]);
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
    }
}
