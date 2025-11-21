using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private float deathYPosition = 10f;
    [SerializeField] private float fallDeathY = -10f;
    
    // HashSet para armazenar os blocos que já causaram dano (mais eficiente que List)
    private HashSet<GameObject> touchedHarmfulBlocks = new HashSet<GameObject>();
    
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
                // Verifica se este bloco específico já foi tocado antes
                if (!touchedHarmfulBlocks.Contains(collision.gameObject))
                {
                    Debug.Log("💥 Bloco PERIGOSO - Aplicando dano pela primeira vez!");
                    touchedHarmfulBlocks.Add(collision.gameObject); // Marca como tocado
                    TakeDamage();
                }
                else
                {
                    Debug.Log("🟡 Bloco PERIGOSO já tocado anteriormente - Dano evitado");
                }
            }
            else
            {
                Debug.Log("✅ Bloco SEGURO - Sem dano");
            }
        }
    }

    // Opcional: método para limpar blocos tocados se necessário (quando mudar de fase, etc.)
    public void ClearTouchedBlocks()
    {
        touchedHarmfulBlocks.Clear();
        Debug.Log("🧹 Lista de blocos tocados foi limpa");
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