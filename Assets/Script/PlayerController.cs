using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private float deathYPosition = 10f;
    [SerializeField] private float fallDeathY = -10f;
    
    void Update()
    {
        CheckDeathConditions();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"🎯 Colidiu com: {collision.gameObject.name}");
    
        BlockController block = collision.gameObject.GetComponent<BlockController>();
    
        if(block != null)
        {
            Debug.Log($"📦 Bloco detectado. Tipo: {block.Type}");
        
            if(block.Type == BlockController.BlockType.Harmful)
            {
                Debug.Log("💥 Bloco PERIGOSO - Aplicando dano!");
                TakeDamage();
            }
            else
            {
                Debug.Log("✅ Bloco SEGURO - Sem dano");
            }
        }
    }

    void TakeDamage()
    {
        Debug.Log("🔴 DANO TOMADO!");
    
        if (GameManager.Instance != null)
        {
            Debug.Log("✅ GameManager encontrado, aplicando dano");
            GameManager.Instance.TakeDamage();
        }
        else
        {
            Debug.LogError("❌ GameManager Instance é NULL!");
        }
    }
    
    void CheckDeathConditions()
    {
        // Morre se atingir o topo da tela
        if (transform.position.y >= deathYPosition)
        {
            Debug.Log($"☠️ MORTO - Atingiu o topo! Y: {transform.position.y} > {deathYPosition}");
            GameOverByTop();
        }
        // Morre se cair muito
        else if (transform.position.y <= fallDeathY)
        {
            Debug.Log($"☠️ MORTO - Caiu demais! Y: {transform.position.y} < {fallDeathY}");
            GameOverByFall();
        }
    }
    
    void GameOverByTop()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
        {
            Debug.Log("💥 Aplicando dano duplo por morte no topo");
            GameManager.Instance.TakeDamage();
            GameManager.Instance.TakeDamage();
        }
    }
    
    void GameOverByFall()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver())
        {
            Debug.Log("💥 Aplicando dano duplo por queda");
            GameManager.Instance.TakeDamage();
            GameManager.Instance.TakeDamage();
        }
    }
}