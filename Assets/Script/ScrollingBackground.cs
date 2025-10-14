using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    private Material _material;
    
    [SerializeField] private float _speed;

    private Vector2 _offset;
    
    void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    void Update()
    {
        _offset.y -= _speed * Time.deltaTime;
        _material.mainTextureOffset = _offset;
    }
}
