using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] GameObject harmfulBlockPrefab;
    [SerializeField] GameObject safeBlockPrefab;
    [SerializeField] float spawnRate = 2f;
    [SerializeField] [Range(0, 1)] float safeBlockChance = 0.3f;
    [SerializeField] float minHorizontalDistance = 2f;
    [SerializeField] Transform background;
    [SerializeField] float spawnYOffset = -2f;
    [SerializeField] int maxBlocksInScene = 20;
    
    private float minX, maxX, spawnY;
    private List<GameObject> activeBlocks = new List<GameObject>();
    private float timeSinceLastSpawn = 0f;
    private Queue<Vector2> pendingSpawnPositions = new Queue<Vector2>();
    private bool spawnEnabled = true; // Controle global do spawn

    void Start()
    {
        if (background == null)
        {
            background = GameObject.Find("Background")?.transform;
            if (background == null)
            {
                Debug.LogError("Background não encontrado! Atribua manualmente no Inspector.");
                return;
            }
        }
        
        CalculateBackgroundBounds();
        Debug.Log($"BlockSpawner iniciado. Background bounds - X: {minX} to {maxX}, SpawnY: {spawnY}");
    }

    void Update()
    {
        if (background == null || !spawnEnabled) return;
        
        // Spawn de blocos
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= spawnRate)
        {
            TrySpawnBlock();
            timeSinceLastSpawn = 0f;
        }
        
        // Processar spawns pendentes
        ProcessPendingSpawns();
        
        // Limpar blocos destruídos da lista
        CleanupDestroyedBlocks();
        
        // DEBUG: Log do estado atual
        if (Time.frameCount % 60 == 0) // A cada ~1 segundo
        {
            Debug.Log($"Estado do Spawner - Blocos ativos: {activeBlocks.Count}, Pendentes: {pendingSpawnPositions.Count}, Spawn habilitado: {spawnEnabled}");
        }
    }

    void CalculateBackgroundBounds()
    {
        if (background == null) return;

        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer == null)
        {
            Debug.LogError("Background não tem SpriteRenderer!");
            return;
        }

        Bounds bgBounds = bgRenderer.bounds;
        
        float margin = 0.5f;
        minX = bgBounds.min.x + margin;
        maxX = bgBounds.max.x - margin;
        
        spawnY = bgBounds.min.y + spawnYOffset;
    }

    void TrySpawnBlock()
    {
        // Se já tem muitos blocos, não spawna mais
        if (activeBlocks.Count >= maxBlocksInScene)
        {
            Debug.Log($"Máximo de blocos atingido: {activeBlocks.Count}/{maxBlocksInScene}");
            return;
        }
        
        Vector2 spawnPos = FindValidSpawnPosition();
        
        if (spawnPos != Vector2.zero)
        {
            // Adiciona à fila de spawns pendentes para processar no próximo frame
            pendingSpawnPositions.Enqueue(spawnPos);
            Debug.Log($"Posição válida encontrada: {spawnPos}. Pendentes: {pendingSpawnPositions.Count}");
        }
        else
        {
            // Se não encontrou posição válida, tenta uma abordagem mais relaxada
            Vector2 fallbackPos = FindFallbackSpawnPosition();
            if (fallbackPos != Vector2.zero)
            {
                pendingSpawnPositions.Enqueue(fallbackPos);
                Debug.Log("Usando posição fallback para spawn");
            }
            else
            {
                // Mesmo se não encontrar posição, força um spawn em posição aleatória
                Vector2 forcedPos = new Vector2(Random.Range(minX, maxX), spawnY);
                pendingSpawnPositions.Enqueue(forcedPos);
                Debug.LogWarning("Forçando spawn em posição aleatória!");
            }
        }
    }

    void ProcessPendingSpawns()
    {
        while (pendingSpawnPositions.Count > 0)
        {
            Vector2 spawnPos = pendingSpawnPositions.Dequeue();
            SpawnBlockAtPosition(spawnPos);
        }
    }

    void SpawnBlockAtPosition(Vector2 spawnPos)
    {
        GameObject blockToSpawn = Random.value <= safeBlockChance ? safeBlockPrefab : harmfulBlockPrefab;
        
        if (blockToSpawn != null)
        {
            GameObject newBlock = Instantiate(blockToSpawn, spawnPos, Quaternion.identity);
            activeBlocks.Add(newBlock);
            
            Debug.Log($"Spawned {blockToSpawn.name} at {spawnPos}. Total de blocos ativos: {activeBlocks.Count}");
        }
    }

    Vector2 FindValidSpawnPosition()
    {
        // Se não há blocos ativos, qualquer posição é válida
        if (activeBlocks.Count == 0 && pendingSpawnPositions.Count == 0)
        {
            Vector2 randomPos = new Vector2(Random.Range(minX, maxX), spawnY);
            Debug.Log("Nenhum bloco ativo - spawnando em posição aleatória: " + randomPos);
            return randomPos;
        }
        
        int maxAttempts = 30;
        Vector2 spawnPos = Vector2.zero;
        bool validPositionFound = false;
        
        for (int i = 0; i < maxAttempts; i++)
        {
            spawnPos = new Vector2(Random.Range(minX, maxX), spawnY);
            
            if (IsPositionValid(spawnPos))
            {
                validPositionFound = true;
                break;
            }
        }
        
        return validPositionFound ? spawnPos : Vector2.zero;
    }
    
    Vector2 FindFallbackSpawnPosition()
    {
        // Abordagem mais relaxada: encontra a posição com MAIOR distância dos blocos existentes
        int samplePoints = 10;
        Vector2 bestPosition = Vector2.zero;
        float maxMinDistance = 0f;
        
        for (int i = 0; i < samplePoints; i++)
        {
            Vector2 testPos = new Vector2(Random.Range(minX, maxX), spawnY);
            float minDistance = CalculateMinDistanceToBlocks(testPos);
            
            if (minDistance > maxMinDistance)
            {
                maxMinDistance = minDistance;
                bestPosition = testPos;
            }
        }
        
        // Se encontrou uma posição com alguma distância, usa
        if (maxMinDistance > 0.5f) // Pelo menos 0.5 unidades de distância
        {
            return bestPosition;
        }
        
        return Vector2.zero;
    }
    
    float CalculateMinDistanceToBlocks(Vector2 position)
    {
        float minDistance = float.MaxValue;
        
        // Verifica blocos ativos
        foreach (GameObject block in activeBlocks)
        {
            if (block == null) continue;
            
            float horizontalDistance = Mathf.Abs(position.x - block.transform.position.x);
            if (horizontalDistance < minDistance)
            {
                minDistance = horizontalDistance;
            }
        }
        
        // Verifica posições pendentes
        foreach (Vector2 pendingPos in pendingSpawnPositions)
        {
            float horizontalDistance = Mathf.Abs(position.x - pendingPos.x);
            if (horizontalDistance < minDistance)
            {
                minDistance = horizontalDistance;
            }
        }
        
        return minDistance;
    }

    bool IsPositionValid(Vector2 position)
    {
        // Se não há blocos ativos nem pendentes, a posição é sempre válida
        if (activeBlocks.Count == 0 && pendingSpawnPositions.Count == 0)
            return true;
        
        // Verifica se está muito próximo de outros blocos (horizontalmente)
        foreach (GameObject block in activeBlocks)
        {
            if (block == null) continue;
            
            float horizontalDistance = Mathf.Abs(position.x - block.transform.position.x);
            if (horizontalDistance < minHorizontalDistance)
            {
                return false;
            }
        }
        
        // Verifica também posições pendentes
        foreach (Vector2 pendingPos in pendingSpawnPositions)
        {
            float horizontalDistance = Mathf.Abs(position.x - pendingPos.x);
            if (horizontalDistance < minHorizontalDistance)
            {
                return false;
            }
        }
        
        return true;
    }

    void CleanupDestroyedBlocks()
    {
        // Remove blocos nulos (que foram destruídos) da lista
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            if (activeBlocks[i] == null)
            {
                activeBlocks.RemoveAt(i);
            }
        }
    }
    
    // Método público para habilitar/desabilitar o spawn
    public void SetSpawnEnabled(bool enabled)
    {
        spawnEnabled = enabled;
        Debug.Log($"Spawn habilitado: {enabled}");
    }
    
    // Método para forçar spawn de um bloco (útil para debug)
    public void ForceSpawnBlock()
    {
        Vector2 spawnPos = new Vector2(Random.Range(minX, maxX), spawnY);
        SpawnBlockAtPosition(spawnPos);
    }
    
    // Visualização no Editor
    void OnDrawGizmosSelected()
    {
        if (background == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, spawnY, 0), new Vector3(maxX, spawnY, 0));
        
        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer != null)
        {
            Gizmos.color = Color.yellow;
            Bounds bgBounds = bgRenderer.bounds;
            Gizmos.DrawWireCube(bgBounds.center, bgBounds.size);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(minX, bgBounds.min.y, 0), new Vector3(minX, spawnY, 0));
            Gizmos.DrawLine(new Vector3(maxX, bgBounds.min.y, 0), new Vector3(maxX, spawnY, 0));
        }
        
        // Desenha a área de exclusão ao redor de cada bloco ativo
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                Vector3 blockPos = block.transform.position;
                Gizmos.DrawWireCube(
                    new Vector3(blockPos.x, spawnY, 0), 
                    new Vector3(minHorizontalDistance, 0.5f, 0)
                );
            }
        }
    }
}