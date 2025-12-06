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

    private Queue<bool> blockTypeQueue = new Queue<bool>();
    private const int queueSize = 10;

    void Start()
    {
        CalculateSpawnBounds();
        PreFillQueue();
    }
    
    void Update()
    {
        CleanupDestroyedBlocks();
        timer += Time.deltaTime;
        
        float currentSpawnRate = spawnRate;
        
        currentSpawnRate /= GameManager.Instance.GlobalSpeedMultiplier;
        
        if (timer >= currentSpawnRate)
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
    }

    void PreFillQueue()
    {
        blockTypeQueue.Clear();
        for (int i = 0; i < queueSize; i++)
        {
            bool isSafe = Random.value <= safeBlockChance;
            blockTypeQueue.Enqueue(isSafe);
        }
    }

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
        RefillQueueIfNeeded();

        Vector2 spawnPos = FindValidSpawnPosition();

        if (spawnPos != Vector2.zero)
        {
            GameObject prefab = nextIsSafe ? safeBlockPrefab : harmfulBlockPrefab;

            if (prefab == null) return;
            GameObject newBlock = Instantiate(prefab, spawnPos, Quaternion.identity);
            activeBlocks.Add(newBlock);
        }
        else
        {
            blockTypeQueue.Enqueue(nextIsSafe);
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
