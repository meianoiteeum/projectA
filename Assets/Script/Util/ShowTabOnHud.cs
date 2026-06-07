using UnityEngine;

public class ShowTabOnHud : MonoBehaviour
{
    [SerializeField] Transform triggerPos;
    [SerializeField] float triggerRadius;
    [SerializeField] Animator animator;
    
    private bool isPlayerInside; // Verifica se o jogador está na área
    private bool alreadyPressedTab; // Evita que a segunda animação toque mais de uma vez 

    private void Update()
    {
        // Se o jogador estiver dentro do trigger, não tiver apertado tab ainda e apertar a tecla Tab:
        if (isPlayerInside && !alreadyPressedTab && Input.GetKeyDown(KeyCode.Tab))
        {
            animator.Play("ShowQandE");
            alreadyPressedTab = true; // Registra que o jogador já apertou
        }
        if (alreadyPressedTab && Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            animator.Play("HideAll");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            animator.Play("ShowTab");
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            alreadyPressedTab = false; // Reseta caso o jogador saia e entre de novo
        }
    }

    void OnDrawGizmos()
    {
        if (triggerPos != null)
        {
            Gizmos.DrawWireSphere(triggerPos.position, triggerRadius);
        }
    }
}