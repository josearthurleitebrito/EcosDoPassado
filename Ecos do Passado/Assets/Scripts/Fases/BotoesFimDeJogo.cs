using UnityEngine;
using UnityEngine.SceneManagement;

public class BotoesFimDeJogo : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Nome exato da cena do Menu Principal")]
    public string nomeCenaMenu = "MainMenu"; 

    // Botão: TENTAR DE NOVO
    public void TentarDeNovo()
    {
        // Reinicia a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Botão: MENU
    public void IrParaMenu()
    {
        SceneManager.LoadScene(nomeCenaMenu);
    }

    // Botão: SAIR
    public void SairDoJogo()
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