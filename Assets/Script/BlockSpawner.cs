using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] GameObject harmfulBlockPrefab;
    [SerializeField] GameObject safeBlockPrefab;
    [SerializeField] float spawnRate = 1.5f;
    [SerializeField] [Range(0, 1)] float safeBlockChance = 0.3f;
    [SerializeField] Transform background;
    
    // Controles de spawn
    [SerializeField] float maxSpawnWidth = 10f; // Largura máxima onde os blocos podem aparecer
    [SerializeField] float minHorizontalDistance = 1.5f; // Distância mínima horizontal entre blocos
    [SerializeField] float minVerticalDistance = 2f; // Distância mínima vertical entre blocos
    
    private float minX, maxX, spawnY;
    private float timer = 0f;
    private List<GameObject> activeBlocks = new List<GameObject>();

    void Start()
    {
        CalculateSpawnBounds();
        // SpawnRandomBlock(); // Primeiro bloco
    }

    void Update()
    {
        // Limpa blocos destruídos da lista
        CleanupDestroyedBlocks();
        
        timer += Time.deltaTime;
        
        if (timer >= spawnRate)
        {
            SpawnRandomBlock();
            timer = 0f;
        }
    }

    void CalculateSpawnBounds()
    {
        if (background == null)
        {
            // Usa a largura máxima definida, centralizada na tela
            minX = -maxSpawnWidth / 2;
            maxX = maxSpawnWidth / 2;
            spawnY = -5f;
            return;
        }

        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer != null)
        {
            Bounds bgBounds = bgRenderer.bounds;
            
            // Limita a largura de spawn ao valor máximo definido
            float availableWidth = Mathf.Min(bgBounds.size.x - 1f, maxSpawnWidth);
            minX = bgBounds.center.x - availableWidth / 2;
            maxX = bgBounds.center.x + availableWidth / 2;
            
            spawnY = bgBounds.min.y - 1f;
        }
        else
        {
            // Fallback com largura máxima
            minX = -maxSpawnWidth / 2;
            maxX = maxSpawnWidth / 2;
            spawnY = -5f;
        }
        
        Debug.Log($"Área de spawn: X({minX} to {maxX}), Y={spawnY}");
    }

    void SpawnRandomBlock()
    {
        Vector2 spawnPos = FindValidSpawnPosition();
        
        if (spawnPos != Vector2.zero)
        {
            GameObject blockToSpawn = Random.value <= safeBlockChance ? safeBlockPrefab : harmfulBlockPrefab;
            
            if (blockToSpawn != null)
            {
                GameObject newBlock = Instantiate(blockToSpawn, spawnPos, Quaternion.identity);
                activeBlocks.Add(newBlock);
                
                Debug.Log($"Spawned {blockToSpawn.name} at {spawnPos}");
            }
        }
        else
        {
            Debug.Log("Não foi possível encontrar posição válida para spawn");
        }
    }

    Vector2 FindValidSpawnPosition()
    {
        int maxAttempts = 20;
        
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 candidatePos = new Vector2(Random.Range(minX, maxX), spawnY);
            
            if (IsPositionValid(candidatePos))
            {
                return candidatePos;
            }
        }
        
        return Vector2.zero; // Não encontrou posição válida
    }

    bool IsPositionValid(Vector2 position)
    {
        foreach (GameObject block in activeBlocks)
        {
            if (block == null) continue;
            
            Vector2 blockPos = block.transform.position;
            
            // Calcula distâncias
            float horizontalDist = Mathf.Abs(position.x - blockPos.x);
            float verticalDist = Mathf.Abs(position.y - blockPos.y);
            
            // Verifica se está muito próximo horizontal E verticalmente
            if (horizontalDist < minHorizontalDistance && verticalDist < minVerticalDistance)
            {
                return false; // Muito próximo em ambos os eixos
            }
        }
        
        return true;
    }

    void CleanupDestroyedBlocks()
    {
        // Remove blocos que foram destruídos da lista
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            if (activeBlocks[i] == null)
            {
                activeBlocks.RemoveAt(i);
            }
        }
    }
    
    // Visualização no Editor para debug
    void OnDrawGizmosSelected()
    {
        // Desenha a área de spawn
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, spawnY - 0.5f, 0), new Vector3(maxX, spawnY - 0.5f, 0));
        Gizmos.DrawLine(new Vector3(minX, spawnY + 0.5f, 0), new Vector3(maxX, spawnY + 0.5f, 0));
        Gizmos.DrawLine(new Vector3(minX, spawnY - 0.5f, 0), new Vector3(minX, spawnY + 0.5f, 0));
        Gizmos.DrawLine(new Vector3(maxX, spawnY - 0.5f, 0), new Vector3(maxX, spawnY + 0.5f, 0));
        
        // Desenha a largura máxima
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-maxSpawnWidth/2, spawnY - 1f, 0), new Vector3(maxSpawnWidth/2, spawnY - 1f, 0));
    }
}