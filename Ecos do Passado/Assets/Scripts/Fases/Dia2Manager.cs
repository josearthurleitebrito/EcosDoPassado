using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Dia2Manager : MonoBehaviour
{
    public static Dia2Manager Instance;

    [Header("Estado Atual")]
    public Dia2State estadoAtual;

    [Header("Configurações do Desafio")]
    public float tempoMaximo = 60.0f;
    public int barrisParaEncontrar = 3;

    [Header("Mensagens de Missão (Editáveis)")]
    [TextArea(2, 3)]
    public string msgInicioDesafio = "ALERTA: O Viajante sabotou o campo! Remova a pólvora de 3 barris antes que exploda!";
    
    [TextArea(2, 3)]
    public string msgFimDesafio = "Ameaça neutralizada! Fale com Carolina, Mateus e Lucas imediatamente.";

    [Header("Referências de UI")]
    public DialogueUI2D dialogueUI;
    public GameObject painelMissao;
    public TMP_Text textoTimer;
    public TMP_Text textoContador;

    [Header("Referências de Cena")]
    public NarratorController narrator;

    // Variáveis Internas
    private float tempoAtual;
    private int barrisEncontrados = 0;
    private bool falouCarolina = false;
    private bool falouMateus = false;
    private bool falouLucas = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        tempoAtual = tempoMaximo;
        if(painelMissao != null) painelMissao.SetActive(false);
        AtualizarUI();

        estadoAtual = Dia2State.Intro;

        if (narrator != null)
        {
            narrator.TocarIntro(() => 
            {
                estadoAtual = Dia2State.FalarComViajante;
            });
        }
    }

    private void Update()
    {
        if (estadoAtual == Dia2State.ProcurandoBarris)
        {
            tempoAtual -= Time.deltaTime;
            
            if(textoTimer != null) textoTimer.text = Mathf.CeilToInt(tempoAtual).ToString() + "s";

            if (tempoAtual <= 0)
            {
                GameOver();
            }
        }
    }

    public void RegistrarConversaNPC(string nomeNPC)
    {
        if (estadoAtual == Dia2State.FalarComViajante && nomeNPC == "Viajante")
        {
            IniciarDesafio();
        }
        else if (estadoAtual == Dia2State.RelatarEquipe)
        {
            if (nomeNPC == "Carolina") falouCarolina = true;
            if (nomeNPC == "Mateus") falouMateus = true;
            if (nomeNPC == "Lucas") falouLucas = true;

            ChecarVitoriaFinal();
        }
    }

    private void IniciarDesafio()
    {
        estadoAtual = Dia2State.ProcurandoBarris;
        
        if(painelMissao != null) painelMissao.SetActive(true);
        
        // --- AQUI ESTÁ O DIÁLOGO DO NARRADOR/SISTEMA ---
        // Ele usa a variável que você escreveu no Inspector
        dialogueUI.ShowDialogue("Sistema", msgInicioDesafio, null);
    }

    public void ChecarBarril(BarrilInteract barril)
    {
        if (barril.temPolvora)
        {
            barrisEncontrados++;
            AtualizarUI();

            if (barrisEncontrados >= barrisParaEncontrar)
            {
                FaseConcluida_FalarComEquipe();
            }
            else
            {
                dialogueUI.ShowDialogue("Sistema", $"Pólvora removida! Faltam {barrisParaEncontrar - barrisEncontrados}.", null);
            }
        }
        else
        {
            dialogueUI.ShowDialogue("Sistema", "Barril vazio. O tempo está correndo!", null);
        }
    }

    private void FaseConcluida_FalarComEquipe()
    {
        estadoAtual = Dia2State.RelatarEquipe;
        
        // --- AQUI ESTÁ O DIÁLOGO DE FALAR COM A EQUIPE ---
        // Ele usa a variável que você escreveu no Inspector
        dialogueUI.ShowDialogue("Sistema", msgFimDesafio, null);
        
        if(textoContador != null) textoContador.text = "Avise a Equipe!";
        if(textoTimer != null) textoTimer.text = "--"; 
    }

    private void ChecarVitoriaFinal()
    {
        if (falouCarolina && falouMateus && falouLucas)
        {
            VitoriaReal();
        }
    }

    private void VitoriaReal()
    {
        estadoAtual = Dia2State.Vitoria;
        if(painelMissao != null) painelMissao.SetActive(false);

        narrator.TocarFinal(() => 
        {
            SceneManager.LoadScene("MenuPrincipal"); 
        });
    }

    private void AtualizarUI()
    {
        if(textoContador != null) 
            textoContador.text = $"{barrisEncontrados}/{barrisParaEncontrar} Barris";
    }

    private void GameOver()
    {
        estadoAtual = Dia2State.GameOver;
        dialogueUI.ShowDialogue("Sistema", "Tempo esgotado! Game Over.", null);
        Invoke("ReiniciarFase", 3f);
    }

    private void ReiniciarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}