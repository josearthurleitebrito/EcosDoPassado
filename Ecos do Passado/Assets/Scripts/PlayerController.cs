// PlayerController.cs - CÓDIGO ATUALIZADO
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- Configurações ---
    [Header("Configuração de Movimento")]
    public float moveSpeed = 5f;

    // --- Referências ---
    private Rigidbody2D rb;
    private Animator animator;

    // --- Variáveis de Estado ---
    private Vector2 moveInput;
    private bool isInteracting = false;
    private Collider2D currentInteractable = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null || animator == null)
        {
            Debug.LogError("Componentes Rigidbody2D ou Animator não encontrados no Player.");
        }
    }

    void Update()
    {
        // 1. Captura de Input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normaliza o input para que o movimento diagonal não seja mais rápido
        moveInput.Normalize();

        // 5. Interações
        if (Input.GetKeyDown(KeyCode.E) && !isInteracting)
        {
            TryInteract();
        }
    }

    void FixedUpdate()
    {
        // Executa a lógica de física
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Se estiver interagindo, o jogador não se move
        if (isInteracting)
        {
            rb.linearVelocity = Vector2.zero;
            // Define o estado de Movimento para IDLE (0)
            animator.SetInteger("Movimento", 0); 
            return;
        }

        // Aplica o movimento
        rb.linearVelocity = moveInput * moveSpeed;

        // 2. e 3. Blend e Lógica de LastMove
        bool isMoving = moveInput != Vector2.zero;
        
        // **NOVO:** Usa a variável Int "Movimento" (0 = Idle, 1 = Walk)
        animator.SetInteger("Movimento", isMoving ? 1 : 0); 

        if (isMoving)
        {
            // **NOVO:** Atualiza o Blend Tree de Walk (usando AxisX e AxisY)
            animator.SetFloat("AxisX", moveInput.x);
            animator.SetFloat("AxisY", moveInput.y);

            // 4. Atualiza o LastMove para o Idle
            animator.SetFloat("LastMoveX", moveInput.x);
            animator.SetFloat("LastMoveY", moveInput.y);
        }
    }

    // --- Lógica de Interação ---
    // (A lógica de interação (OnTriggerEnter/Exit e TryInteract) permanece inalterada,
    // pois não afeta as variáveis do Animator.)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    private void TryInteract()
    {
        if (currentInteractable != null)
        {
            IInteractable interactable = currentInteractable.GetComponent<IInteractable>();
            
            if (interactable != null)
            {
                isInteracting = true;
                rb.linearVelocity = Vector2.zero;
                // Garante que o Player pare e entre em Idle
                animator.SetInteger("Movimento", 0); 
                
                interactable.Interact(this);
            }
        }
    }

    public void FinishInteraction()
    {
        isInteracting = false;
    }
}

// Interface que define o que é um objeto interativo
public interface IInteractable
{
    void Interact(PlayerController player);
}