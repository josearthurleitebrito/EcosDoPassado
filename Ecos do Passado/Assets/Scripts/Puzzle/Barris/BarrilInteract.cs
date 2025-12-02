using UnityEngine;

public class BarrilInteract : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Marque isso se este for um dos barris 'premiados' com pólvora.")]
    public bool temPolvora = false;

    private bool jaVerificado = false;
    private bool playerPerto = false;

    private void Update()
    {
        // Só permite interagir se o jogo estiver na etapa de procurar e o player estiver perto
        if (playerPerto && !jaVerificado && Input.GetKeyDown(KeyCode.E))
        {
            if (Dia2Manager.Instance.estadoAtual == Dia2State.ProcurandoBarris)
            {
                Verificar();
            }
        }
    }

    private void Verificar()
    {
        jaVerificado = true; // Impede clicar duas vezes no mesmo barril
        
        // Avisa o gerente
        Dia2Manager.Instance.ChecarBarril(this);

        // Feedback Visual (Opcional: Desativar colisão ou mudar cor)
        // GetComponent<SpriteRenderer>().color = Color.gray;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerPerto = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerPerto = false;
    }
}