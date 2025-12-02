using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Configuração")]
    [Tooltip("Escreva aqui o nome EXATO da cena do seu Prólogo.")]
    public string nomeCenaInicial = "Prologo"; 

    // Função para o botão JOGAR
    public void Jogar()
    {
        // Carrega a cena do prólogo
        SceneManager.LoadScene(nomeCenaInicial);
    }

    // Função para o botão SAIR
    public void Sair()
    {
        Debug.Log("O jogo fechou (Funciona apenas na Build final).");
        Application.Quit();
    }
}