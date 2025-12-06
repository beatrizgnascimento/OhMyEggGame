using UnityEngine;

public class BlockController : MonoBehaviour
{
    public enum BlockType { Harmful, Safe }

    [SerializeField] float _speed;
    [SerializeField] BlockType blockType;

    public BlockType Type => blockType;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float currentSpeed = _speed;
        
        currentSpeed *= GameManager.Instance.GlobalSpeedMultiplier;
        
        rb.MovePosition(rb.position + Vector2.up * (currentSpeed * Time.fixedDeltaTime));
    }
}