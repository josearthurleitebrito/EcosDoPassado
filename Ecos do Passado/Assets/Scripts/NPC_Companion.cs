// NPC_Companion.cs - CÓDIGO ATUALIZADO
using UnityEngine;

public class NPC_Companion : MonoBehaviour
{
    // --- Enum de Comportamento ---
    public enum CompanionMode { FOLLOW, IDLE_STATIONARY }

    // --- Configurações ---
    [Header("Configuração de Comportamento")]
    public CompanionMode currentMode = CompanionMode.FOLLOW;
    public Transform target; // O alvo a seguir (Player ou outro Cientista)
    public float followDistance = 1.5f;
    public float followSpeed = 4f;

    // --- Referências ---
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null || animator == null)
        {
            Debug.LogError("Componentes Rigidbody2D ou Animator não encontrados no Companheiro.");
        }
        
        if (target == null && currentMode == CompanionMode.FOLLOW)
        {
            Debug.LogError("Target não configurado para o modo FOLLOW!");
        }
    }

    void FixedUpdate()
    {
        if (currentMode == CompanionMode.FOLLOW)
        {
            HandleFollow();
        }
        else if (currentMode == CompanionMode.IDLE_STATIONARY)
        {
            HandleIdleStationary();
        }
    }

    private void HandleFollow()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > followDistance)
        {
            // 3. Aplica o movimento
            Vector2 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * followSpeed;
            
            // **NOVO:** Atualiza Animação para WALK (1)
            animator.SetInteger("Movimento", 1); 
            
            // **NOVO:** Atualiza as variáveis de direção de movimento
            animator.SetFloat("AxisX", direction.x);
            animator.SetFloat("AxisY", direction.y);
            
            // Atualiza LastMove (para quando ele parar)
            animator.SetFloat("LastMoveX", direction.x);
            animator.SetFloat("LastMoveY", direction.y);
        }
        else
        {
            // Pára, mantendo a direção do Idle
            rb.linearVelocity = Vector2.zero;
            // **NOVO:** Atualiza Animação para IDLE (0)
            animator.SetInteger("Movimento", 0); 
        }
    }

    private void HandleIdleStationary()
    {
        // 4. Lógica de Idle Parado: Garante que não está se movendo
        rb.linearVelocity = Vector2.zero;
        // **NOVO:** Atualiza Animação para IDLE (0)
        animator.SetInteger("Movimento", 0); 

        // Lógica opcional para fazer o NPC olhar para o Player
        /*
        if (target != null)
        {
            Vector2 lookDirection = (target.position - transform.position).normalized;
            animator.SetFloat("LastMoveX", lookDirection.x);
            animator.SetFloat("LastMoveY", lookDirection.y);
        }
        */
    }

    // --- Interações com Fala ---
    public void StartDialogue(string[] dialogueLines)
    {
        // Lógica de diálogo...
        Debug.Log($"Cientista {gameObject.name} iniciando diálogo com {dialogueLines.Length} linhas.");
    }

    public void ChangeMode(CompanionMode newMode)
    {
        currentMode = newMode;
    }
}