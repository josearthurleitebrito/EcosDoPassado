using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MensageiroRunner : MonoBehaviour
{
    [Header("Configuração de Rota")]
    public Transform[] pontosDeTrajeto;
    public float velocidade = 5f;

    [Header("Configuração do Animator (Igual ao Player)")]
    public Animator animator;
    
    // Nomes exatos dos parâmetros baseados no seu PlayerController
    private string paramMovimento = "Movimento"; // Int (0 ou 1)
    private string paramAxisX = "AxisX";         // Float
    private string paramAxisY = "AxisY";         // Float
    private string paramLastX = "LastMoveX";     // Float (Para o Idle)
    private string paramLastY = "LastMoveY";     // Float (Para o Idle)

    private int indexAtual = 0;
    private bool correndo = false;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void IniciarCorrida()
    {
        correndo = true;
        gameObject.SetActive(true);
        indexAtual = 0;
    }

    void Update()
    {
        if (!correndo || pontosDeTrajeto.Length == 0) return;

        // Se chegou ao fim do array, não faz nada (a lógica de parada final já foi chamada)
        if (indexAtual >= pontosDeTrajeto.Length) return;

        Transform alvo = pontosDeTrajeto[indexAtual];

        // 1. Calcular a direção
        Vector3 diferenca = alvo.position - transform.position;
        Vector3 direcaoNormalizada = diferenca.normalized;

        // 2. Mover o NPC
        transform.position = Vector3.MoveTowards(transform.position, alvo.position, velocidade * Time.deltaTime);

        // 3. Atualizar Animação de Corrida
        AtualizarAnimator(direcaoNormalizada, true);

        // Verifica se chegou no ponto (com pequena margem de erro)
        if (diferenca.magnitude < 0.1f)
        {
            indexAtual++;

            // SE CHEGOU NO ÚLTIMO PONTO
            if (indexAtual >= pontosDeTrajeto.Length)
            {
                FinalizarTrajeto();
            }
        }
    }

    private void FinalizarTrajeto()
    {
        correndo = false;
        Debug.Log("Mensageiro chegou no porto.");

        // --- FORÇAR IDLE VIRADO PARA A ESQUERDA ---
        // X = -1 (Esquerda), Y = 0
        Vector2 direcaoFinal = new Vector2(-1f, 0f);
        
        AtualizarAnimator(direcaoFinal, false);
    }

    private void AtualizarAnimator(Vector2 direcao, bool estaAndando)
    {
        if (animator != null)
        {
            if (estaAndando)
            {
                // Define estado como ANDANDO (1)
                animator.SetInteger(paramMovimento, 1);

                // Atualiza o Blend Tree de movimento
                animator.SetFloat(paramAxisX, direcao.x);
                animator.SetFloat(paramAxisY, direcao.y);

                // IMPORTANTE: Atualiza o LastMove enquanto anda para transições suaves
                animator.SetFloat(paramLastX, direcao.x);
                animator.SetFloat(paramLastY, direcao.y);
            }
            else
            {
                // Define estado como IDLE (0)
                animator.SetInteger(paramMovimento, 0);

                // Zera os eixos de movimento (opcional, mas bom para limpeza)
                animator.SetFloat(paramAxisX, 0);
                animator.SetFloat(paramAxisY, 0);

                // FORÇA A DIREÇÃO DO OLHAR (Aqui garantimos que ele olhe para a esquerda no final)
                animator.SetFloat(paramLastX, direcao.x);
                animator.SetFloat(paramLastY, direcao.y);
            }
        }
    }
}