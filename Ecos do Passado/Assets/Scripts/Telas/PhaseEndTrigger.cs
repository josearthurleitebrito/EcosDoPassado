using UnityEngine;

public class PhaseEndTrigger : MonoBehaviour
{
    [Header("Configuração de Fase")]
    // IMPORTANTE: Selecione no Inspector qual é a fase ATUAL onde este objeto está
    public GameScene thisPhase; 
    
    // Opção 1: Gatilho por colisão (Player entra na saída)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerNextPhase();
        }
    }
    
    // Opção 2: Método público para chamar via código
    public void TriggerNextPhase()
    {
        if (GameSceneManager.Instance != null)
        {
            Debug.Log($"Encerrando fase: {thisPhase}");
            GameSceneManager.Instance.AdvanceToNextPhase(thisPhase);
        }
        else
        {
            Debug.LogError("GameSceneManager não encontrado na cena!");
        }
    }
}