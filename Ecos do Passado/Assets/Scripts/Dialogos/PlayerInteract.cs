using UnityEngine;

public class PlayerInteract2D : MonoBehaviour
{
    [Header("Configurações")]
    public KeyCode interactKey = KeyCode.E;
    
    // Opcional: Referência para um ícone visual de "Pressione E" (balãozinho)
    public GameObject interactIcon; 

    private NPCDialogue2D currentNPC;

    void Update()
    {
        // Verifica o Input e se tem um NPC na área
        if (Input.GetKeyDown(interactKey) && currentNPC != null)
        {
            // Chama o método inteligente que criamos no NPC
            currentNPC.Interact(); 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            currentNPC = other.GetComponent<NPCDialogue2D>();
            
            // Mostra ícone se existir
            if(interactIcon != null) interactIcon.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC") && other.GetComponent<NPCDialogue2D>() == currentNPC)
        {
            currentNPC = null;
            
            // Esconde ícone se existir
            if(interactIcon != null) interactIcon.SetActive(false);
        }
    }
}