using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] GameObject harmfulBlockPrefab;
    [SerializeField] GameObject safeBlockPrefab;
    [SerializeField] float spawnRate = 1.5f;
    [SerializeField] [Range(0, 1)] float safeBlockChance = 0.3f;
    [SerializeField] Transform background;
    
    [SerializeField] float maxSpawnWidth = 10f;
    [SerializeField] float minHorizontalDistance = 1.5f;
    [SerializeField] float minVerticalDistance = 2f;
    
    private float minX, maxX, spawnY;
    private float timer = 0f;
    private List<GameObject> activeBlocks = new List<GameObject>();

    // 🟢 Nova fila de tipos de blocos já sorteados (true = safe, false = harmful)
    private Queue<bool> blockTypeQueue = new Queue<bool>();
    private const int queueSize = 10; // tamanho da fila de pré-sorteios

    void Start()
    {
        CalculateSpawnBounds();
        PreFillQueue();
    }

    void Update()
    {
        CleanupDestroyedBlocks();
        timer += Time.deltaTime;
        
        if (timer >= spawnRate)
        {
            SpawnFromQueue();
            timer = 0f;
        }
    }

    void CalculateSpawnBounds()
    {
        if (background == null)
        {
            minX = -maxSpawnWidth / 2;
            maxX = maxSpawnWidth / 2;
            spawnY = -5f;
            return;
        }

        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer != null)
        {
            Bounds bgBounds = bgRenderer.bounds;
            float availableWidth = Mathf.Min(bgBounds.size.x - 1f, maxSpawnWidth);
            minX = bgBounds.center.x - availableWidth / 2;
            maxX = bgBounds.center.x + availableWidth / 2;
            spawnY = bgBounds.min.y - 1f;
        }
        else
        {
            minX = -maxSpawnWidth / 2;
            maxX = maxSpawnWidth / 2;
            spawnY = -5f;
        }

        Debug.Log($"Área de spawn: X({minX} to {maxX}), Y={spawnY}");
    }

    // 🟢 Preenche a fila com resultados baseados em safeBlockChance
    void PreFillQueue()
    {
        blockTypeQueue.Clear();
        for (int i = 0; i < queueSize; i++)
        {
            bool isSafe = Random.value <= safeBlockChance;
            blockTypeQueue.Enqueue(isSafe);
        }
    }

    // 🟢 Garante que a fila nunca esvazie
    void RefillQueueIfNeeded()
    {
        while (blockTypeQueue.Count < queueSize)
        {
            bool isSafe = Random.value <= safeBlockChance;
            blockTypeQueue.Enqueue(isSafe);
        }
    }

    void SpawnFromQueue()
    {
        if (blockTypeQueue.Count == 0)
            RefillQueueIfNeeded();

        bool nextIsSafe = blockTypeQueue.Dequeue();
        RefillQueueIfNeeded(); // mantém a fila cheia

        Vector2 spawnPos = FindValidSpawnPosition();

        if (spawnPos != Vector2.zero)
        {
            GameObject prefab = nextIsSafe ? safeBlockPrefab : harmfulBlockPrefab;

            if (prefab != null)
            {
                GameObject newBlock = Instantiate(prefab, spawnPos, Quaternion.identity);
                activeBlocks.Add(newBlock);

                Debug.Log($"Spawned {(nextIsSafe ? "SAFE" : "HARMFUL")} block at {spawnPos}");
            }
        }
        else
        {
            // ❗ Importante: Se não conseguiu spawnar, devolve o tipo para a fila
            blockTypeQueue.Enqueue(nextIsSafe);
            Debug.Log("Não foi possível encontrar posição válida — tipo devolvido à fila");
        }
    }

    Vector2 FindValidSpawnPosition()
    {
        int maxAttempts = 20;
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 candidatePos = new Vector2(Random.Range(minX, maxX), spawnY);
            if (IsPositionValid(candidatePos))
                return candidatePos;
        }
        return Vector2.zero;
    }

    bool IsPositionValid(Vector2 position)
    {
        foreach (GameObject block in activeBlocks)
        {
            if (block == null) continue;
            Vector2 blockPos = block.transform.position;
            
            float horizontalDist = Mathf.Abs(position.x - blockPos.x);
            float verticalDist = Mathf.Abs(position.y - blockPos.y);
            
            if (horizontalDist < minHorizontalDistance && verticalDist < minVerticalDistance)
                return false;
        }
        return true;
    }

    void CleanupDestroyedBlocks()
    {
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            if (activeBlocks[i] == null)
                activeBlocks.RemoveAt(i);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minX, spawnY - 0.5f, 0), new Vector3(maxX, spawnY - 0.5f, 0));
        Gizmos.DrawLine(new Vector3(minX, spawnY + 0.5f, 0), new Vector3(maxX, spawnY + 0.5f, 0));
        Gizmos.DrawLine(new Vector3(minX, spawnY - 0.5f, 0), new Vector3(minX, spawnY + 0.5f, 0));
        Gizmos.DrawLine(new Vector3(maxX, spawnY - 0.5f, 0), new Vector3(maxX, spawnY + 0.5f, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-maxSpawnWidth/2, spawnY - 1f, 0), new Vector3(maxSpawnWidth/2, spawnY - 1f, 0));
    }
}
