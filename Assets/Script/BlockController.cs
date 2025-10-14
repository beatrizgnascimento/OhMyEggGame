using UnityEngine;

public class BlockController : MonoBehaviour
{
    public enum BlockType { Harmful, Safe }

    [SerializeField] float _speed;
    [SerializeField] BlockType blockType;

    public BlockType Type => blockType;

    void Update()
    {
        transform.Translate(Vector2.up * (_speed * Time.deltaTime));
    }
}