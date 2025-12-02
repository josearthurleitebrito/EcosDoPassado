using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Necessário para Coroutines

public class Dia2Manager : MonoBehaviour
{
    public static Dia2Manager Instance;

    [Header("Estado Atual")]
    public Dia2State estadoAtual;

    [Header("Configurações do Desafio")]
    public float tempoMaximo = 60.0f;
    public int barrisParaEncontrar = 3;

    [Header("Mensagens")]
    [TextArea(2, 3)]
    public string msgInicioDesafio = "ALERTA: Remova a pólvora de 3 barris antes que exploda!";
    [TextArea(2, 3)]
    public string msgFimDesafio = "Ameaça neutralizada! Fale com Carolina, Mateus e Lucas.";

    [Header("Referências de UI")]
    public DialogueUI2D dialogueUI;
    public GameObject painelMissao;
    public TMP_Text textoTimer;
    public TMP_Text textoContador;

    [Header("Telas Finais (Arraste os Painéis)")]
    public GameObject telaWin;      // <--- NOVO
    public GameObject telaGameOver; // <--- NOVO

    [Header("Referências de Cena")]
    public NarratorController narrator;

    // Variáveis Internas
    private float tempoAtual;
    private int barrisEncontrados = 0;
    private bool falouCarolina = false;
    private bool falouMateus = false;
    private bool falouLucas = false;
    private Coroutine corrotinaMensagem; // Para controlar o tempo da mensagem

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        tempoAtual = tempoMaximo;
        
        // Garante que tudo começa desligado
        if(painelMissao != null) painelMissao.SetActive(false);
        if(telaWin != null) telaWin.SetActive(false);
        if(telaGameOver != null) telaGameOver.SetActive(false);
        
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
        
        // Mensagem de SISTEMA: Dura 5 segundos
        MostrarMensagemTemporaria("Sistema", msgInicioDesafio, 5.0f);
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
                // Mensagem de BARRIL: Dura 1 segundo
                MostrarMensagemTemporaria("Sistema", $"Pólvora removida! Faltam {barrisParaEncontrar - barrisEncontrados}.", 1.0f);
            }
        }
        else
        {
            // Mensagem de BARRIL: Dura 1 segundo
            MostrarMensagemTemporaria("Sistema", "Barril vazio.", 1.0f);
        }
    }

    private void MostrarMensagemTemporaria(string titulo, string msg, float duracao)
    {
        // Se já tiver uma mensagem contando tempo, cancela ela para não bugar
        if (corrotinaMensagem != null) StopCoroutine(corrotinaMensagem);
        
        dialogueUI.ShowDialogue(titulo, msg, null);
        corrotinaMensagem = StartCoroutine(FecharDialogoRoutine(duracao));
    }

    private IEnumerator FecharDialogoRoutine(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        dialogueUI.HideDialogue();
        corrotinaMensagem = null;
    }

    private void FaseConcluida_FalarComEquipe()
    {
        estadoAtual = Dia2State.RelatarEquipe;
        
        // Mensagem de SISTEMA: Dura 5 segundos
        MostrarMensagemTemporaria("Sistema", msgFimDesafio, 5.0f);
        
        // Remove texto do contador como pedido
        if(painelMissao != null) painelMissao.SetActive(false);
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

        Debug.Log("JOGO FINALIZADO!");

        narrator.TocarFinal(() => 
        {
            // Ativa o Painel de Win
            if (telaWin != null) telaWin.SetActive(true);
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
        
        dialogueUI.HideDialogue(); // Fecha qualquer texto
        if(painelMissao != null) painelMissao.SetActive(false);

        // Ativa o Painel de Game Over
        if (telaGameOver != null) telaGameOver.SetActive(true);
    }
}