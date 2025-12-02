using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Importante

public class Dia1Manager : MonoBehaviour
{
    public static Dia1Manager Instance;

    [Header("Estado Atual")]
    public Dia1State estadoAtual;

    [Header("Configuração de Navegação")]
    public string nomeProximaCena = "Dia2"; 

    [Header("Intro da Fase (Capa)")]
    public GameObject painelCapa; // <--- ARRASTE A IMAGEM DO DIA 1 AQUI
    public float tempoCapa = 3f;

    [Header("Referências")]
    public MensageiroRunner mensageiroScript;
    public NarratorController narrator;
    public GameObject painelEscolha; 
    public GameObject telaGameOver;

    private bool falouComTaberneiro = false;
    private bool falouComMarinheiro = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private IEnumerator Start()
    {
        // 1. Configurações de UI (Esconde tudo)
        if(painelEscolha != null) painelEscolha.SetActive(false);
        if(telaGameOver != null) telaGameOver.SetActive(false);

        // 2. EXIBE A CAPA
        if (painelCapa != null)
        {
            painelCapa.SetActive(true);
            yield return new WaitForSeconds(tempoCapa);
            painelCapa.SetActive(false);
        }

        // 3. INICIA O JOGO
        IniciarLogicaDaFase();
    }

    private void IniciarLogicaDaFase()
    {
        estadoAtual = Dia1State.InvestigarTaverna; 

        if (narrator != null)
        {
            narrator.TocarIntro(() => 
            {
                Debug.Log("Intro Finalizada. Objetivo: Investigar Taverna.");
            });
        }
    }

    // Chamado pelo script NPCDialogue2D ao final de cada conversa
    public void RegistrarConversaNPC(string nomeNPC)
    {
        // ETAPA 1: TAVERNA
        if (estadoAtual == Dia1State.InvestigarTaverna)
        {
            if (nomeNPC == "Taberneiro") falouComTaberneiro = true;
            if (nomeNPC == "Marinheiro") falouComMarinheiro = true;

            // Se falou com os dois, avança
            if (falouComTaberneiro && falouComMarinheiro)
            {
                estadoAtual = Dia1State.FalarComEscravo;
                Debug.Log("Objetivo Atualizado: Vá ao Armazém falar com o Escravo!");
            }
        }
        // ETAPA 2: ARMAZÉM
        else if (estadoAtual == Dia1State.FalarComEscravo)
        {
            if (nomeNPC == "Escravo")
            {
                TriggerMensageiro();
            }
        }
        // ETAPA 3: PORTO (Depois da corrida)
        else if (estadoAtual == Dia1State.MensageiroFugindo)
        {
            if (nomeNPC == "Mensageiro")
            {
                AbrirEscolhaFinal();
            }
        }
    }

    private void TriggerMensageiro()
    {
        Debug.Log("Escravo falou! Mensageiro começou a correr!");
        estadoAtual = Dia1State.MensageiroFugindo;
        if(mensageiroScript != null) mensageiroScript.IniciarCorrida();
    }

    private void AbrirEscolhaFinal()
    {
        estadoAtual = Dia1State.EscolhaFinal;
        if(painelEscolha != null) painelEscolha.SetActive(true);
    }

    // Método chamado pelos Botões da UI (Carta Certa / Carta Errada)
    public void EscolherCarta(bool isCartaCorreta)
    {
        // Fecha o painel de escolha
        if(painelEscolha != null) painelEscolha.SetActive(false);

        if (isCartaCorreta)
        {
            // --- VITÓRIA ---
            estadoAtual = Dia1State.Vitoria;
            
            // Toca a fala final e carrega o Dia 2
            if (narrator != null)
            {
                narrator.TocarFinal(() => 
                {
                    SceneManager.LoadScene(nomeProximaCena);
                });
            }
            else
            {
                SceneManager.LoadScene(nomeProximaCena);
            }
        }
        else
        {
            // --- DERROTA (GAME OVER) ---
            estadoAtual = Dia1State.GameOver;
            Debug.Log("GAME OVER - Carta errada!");
            
            // Em vez de reiniciar direto, abrimos o painel
            if (telaGameOver != null)
            {
                telaGameOver.SetActive(true);
            }
            else
            {
                // Fallback caso esqueça de arrastar o painel
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}