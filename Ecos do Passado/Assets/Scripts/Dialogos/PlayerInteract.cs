using UnityEngine;

public class PlayerInteract2D : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    private NPCDialogue2D currentNPC;

    void Update()
    {
        if (currentNPC != null && Input.GetKeyDown(interactKey))
        {
            currentNPC.StartDialogue();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
            currentNPC = other.GetComponent<NPCDialogue2D>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
            currentNPC = null;
    }
}
