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
    public float typingSpeed = 0.03f; 

    private NPCDialogue2D currentNPC;
    private bool isActive = false;
    private bool isTyping = false;
    private string currentSentence;
    private Coroutine typingCoroutine;

    // Propriedade pública para outros scripts saberem se ainda está escrevendo
    public bool IsTyping => isTyping; 

    void Awake()
    {
        // Garante que esconde o painel ANTES de qualquer um tentar abrir
        if(panel != null) panel.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        // Lógica de Input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopTypingInstantly();
            }
            else if (currentNPC != null)
            {
                // Se for NPC, ele controla o avanço.
                // Se for NARRADOR (currentNPC == null), quem controla o avanço é o script do NarradorController.
                currentNPC.NextSentence();
            }
        }
    }

    public void ShowDialogue(string name, string text, NPCDialogue2D npc)
    {
        currentNPC = npc; // Se for narrador, isso vira null. Tudo bem.
        
        // Proteção contra referências vazias
        if(nameText != null) nameText.text = name;
        if(dialogueText != null) dialogueText.text = ""; // Limpa antes de digitar
        
        if(panel != null) panel.SetActive(true);
        isActive = true;

        DisplaySentence(text);
    }

    public void UpdateText(string newSentence)
    {
        DisplaySentence(newSentence);
    }

    public void HideDialogue()
    {
        if(panel != null) panel.SetActive(false);
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
        if(dialogueText != null) dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if(dialogueText != null) dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void StopTypingInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        if(dialogueText != null) dialogueText.text = currentSentence;
        isTyping = false;
    }
}