using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Escreva aqui o nome EXATO da cena do seu Prólogo.")]
    public string nomeCenaInicial = "Prólogo"; 

    // Função para o botão JOGAR
    public void Jogar()
    {
        // Carrega a cena do prólogo
        SceneManager.LoadScene(nomeCenaInicial);
    }

    // Função para o botão SAIR
    public void Sair()
    {
        Debug.Log("Saindo do jogo...");
        
        // Esse código faz funcionar tanto no Editor quanto no Jogo Final
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}