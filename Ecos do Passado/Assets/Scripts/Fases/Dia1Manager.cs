using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dia1Manager : MonoBehaviour
{
    public static Dia1Manager Instance;

    [Header("Estado Atual")]
    public Dia1State estadoAtual;

    [Header("Configuração de Navegação")]
    [Tooltip("Nome exato da cena que carrega ao vencer (ex: Dia2)")]
    public string nomeProximaCena = "Dia2"; 

    [Header("Referências")]
    public MensageiroRunner mensageiroScript;
    public NarratorController narrator;
    public GameObject painelEscolha; // O Painel com as 2 cartas para escolher

    // Controle interno das pistas
    private bool falouComTaberneiro = false;
    private bool falouComMarinheiro = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // 1. Garante que a UI de escolha começa fechada
        if(painelEscolha != null) painelEscolha.SetActive(false);

        // 2. Define estado inicial (Intro)
        // Você pode criar um estado 'Intro' no Enum ou apenas assumir que 'InvestigarTaverna' começa travado
        estadoAtual = Dia1State.InvestigarTaverna; 

        // 3. Toca a Intro (Player fica travado pelo script do Narrador)
        if (narrator != null)
        {
            narrator.TocarIntro(() => 
            {
                Debug.Log("Intro Finalizada. Objetivo: Investigar Taverna.");
                // Aqui o player é liberado para andar
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
                // Opcional: Tocar um som ou mostrar aviso na tela
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
        
        // Ativa o script de corrida do NPC
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
        if(painelEscolha != null) painelEscolha.SetActive(false);

        if (isCartaCorreta)
        {
            // VITÓRIA
            estadoAtual = Dia1State.Vitoria;
            
            // Toca a fala final do narrador e DEPOIS carrega a próxima fase
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
            // DERROTA
            estadoAtual = Dia1State.GameOver;
            Debug.Log("GAME OVER - Carta errada!");
            
            // Reinicia a cena atual
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}