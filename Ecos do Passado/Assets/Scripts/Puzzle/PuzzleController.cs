using UnityEngine;
using UnityEngine.UI; // Caso precise manipular UI
using System.Collections;

public class PuzzleController : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo de espera entre terminar os fios e fechar o painel (para o player ver que conseguiu)")]
    public float delayParaFechar = 1.5f;
    
    [Header("Referências")]
    public WireManager wireManager; // Referência ao script que controla a lógica dos fios
    public GameObject feedbackVisualVitoria; // Ex: Uma luz verde ou texto "SUCESSO"

    [Header("Debug (Cheat)")]
    public bool permitirCheat = true;
    public KeyCode teclaCheat = KeyCode.P; // Aperte P para ganhar instantaneamente (bom para testes)

    private bool puzzleResolvido = false;

    private void Update()
    {
        // Cheat para pular o puzzle durante o desenvolvimento
        if (permitirCheat && Input.GetKeyDown(teclaCheat))
        {
            OnPuzzleConcluido();
        }

        // Lógica opcional: Se o WireManager não tiver eventos, podemos verificar ele aqui
        // if (!puzzleResolvido && wireManager != null && wireManager.CheckAllWiresConnected())
        // {
        //     OnPuzzleConcluido();
        // }
    }

    // Este método deve ser chamado pelo WireManager quando o último fio for conectado corretamente
    public void OnPuzzleConcluido()
    {
        if (puzzleResolvido) return; // Evita chamadas duplas
        puzzleResolvido = true;

        Debug.Log("Puzzle Controller: Vitória detectada!");

        // 1. Feedback Visual (Opcional)
        if (feedbackVisualVitoria != null)
            feedbackVisualVitoria.SetActive(true);

        // 2. Tocar som de sucesso (Opcional)
        // AudioManager.Instance.PlaySFX("PuzzleWin");

        // 3. Espera um pouco e avisa o Gerente
        StartCoroutine(FinalizarProcesso());
    }

    private IEnumerator FinalizarProcesso()
    {
        yield return new WaitForSeconds(delayParaFechar);

        // Avisa o "Cérebro" da fase que o puzzle acabou
        if (FaseManager.Instance != null)
        {
            FaseManager.Instance.PuzzleFinalizado(); 
        }
        else
        {
            Debug.LogError("ERRO: FaseManager não encontrado na cena!");
            // Fallback: Fecha o painel sozinho se não tiver manager
            gameObject.SetActive(false); 
        }
    }

    // Chamado sempre que o painel abre (OnEnable) para resetar o estado visual se necessário
    private void OnEnable()
    {
        puzzleResolvido = false;
        if (feedbackVisualVitoria != null) feedbackVisualVitoria.SetActive(false);
    }
}