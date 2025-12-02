using UnityEngine;
using UnityEngine.UI;

public class BotaoTestePuzzle : MonoBehaviour
{
    [Header("Referência")]
    public PuzzleController puzzleController; // O "juiz" que criamos antes

    // Esse método vai no evento OnClick do botão
    public void AoClicar()
    {
        Debug.Log("Teste: Botão clicado, puzzle resolvido!");
        
        // Avisa o controller que acabou
        if(puzzleController != null)
        {
            puzzleController.OnPuzzleConcluido();
        }
    }
}