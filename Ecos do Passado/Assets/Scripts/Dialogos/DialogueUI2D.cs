using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueUI2D : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Configuracao do Efeito")]
    public float typingSpeed = 0.03f; // tempo entre cada letra

    private NPCDialogue2D currentNPC;
    private bool isActive = false;
    private bool isTyping = false;
    private string currentSentence;
    private Coroutine typingCoroutine;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        // Se apertar espaco, pula ou vai para pr�xima frase
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Termina de digitar instantaneamente
                StopTypingInstantly();
            }
            else if (currentNPC != null)
            {
                // Avan�a para a pr�xima frase
                currentNPC.NextSentence();
            }
        }
    }

    public void ShowDialogue(string name, string text, NPCDialogue2D npc)
    {
        currentNPC = npc;
        nameText.text = name;
        dialogueText.text = text;
        panel.SetActive(true);
        isActive = true;

        DisplaySentence(text);

        // Se você guarda a referência do NPC, verifique se é null
        this.currentNPC = npc;
    }



    public void UpdateText(string newSentence)
    {
        DisplaySentence(newSentence);
    }

    public void HideDialogue()
    {
        panel.SetActive(false);
        isActive = false;
        currentNPC = null;
    }

    private void DisplaySentence(string sentence)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentSentence = sentence;
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void StopTypingInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = currentSentence;
        isTyping = false;
    }
}
