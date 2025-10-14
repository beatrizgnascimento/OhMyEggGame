using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int playerHealth = 3;

    void OnCollisionEnter2D(Collision2D collision)
    {
        BlockController block = collision.gameObject.GetComponent<BlockController>();
        
        if(block != null)
        {
            if(block.Type == BlockController.BlockType.Harmful)
            {
                TakeDamage();
            }
            // Blocos seguros não causam dano
        }
    }

    void TakeDamage()
    {
        playerHealth--;
        Debug.Log($"Vida restante: {playerHealth}");
        
        if(playerHealth <= 0)
        {
            // Implementar game over
            Debug.Log("Game Over!");
        }
    }
}