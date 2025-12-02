using UnityEngine;
using UnityEngine.SceneManagement;

public class BotoesFimDeJogo : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Nome exato da cena do Menu Principal")]
    public string nomeCenaMenu = "MenuPrincipal"; 

    // Botão: TENTAR DE NOVO (Da tela Game Over)
    public void TentarDeNovo()
    {
        // Reinicia a fase atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Botão: MENU (Da tela Win)
    public void IrParaMenu()
    {
        SceneManager.LoadScene(nomeCenaMenu);
    }

    // Botão: SAIR (De ambas as telas)
    public void SairDoJogo()
    {
        Debug.Log("O jogo fechou (Isso só funciona na Build final, não no Editor)");
        Application.Quit();
    }
}