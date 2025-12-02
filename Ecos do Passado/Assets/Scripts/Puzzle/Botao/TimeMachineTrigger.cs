using UnityEngine;

public class TimeMachineTrigger : MonoBehaviour
{
    private bool playerNaArea = false;

    private void Update()
    {
        // Verifica se apertou a tecla de interação (ex: E ou Espaço)
        if (playerNaArea && Input.GetKeyDown(KeyCode.E)) 
        {
            FaseManager.Instance.TentarUsarMaquinaTempo();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNaArea = true;
            // Opcional: Mostrar ícone de "Pressione E"
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNaArea = false;
        }
    }
}