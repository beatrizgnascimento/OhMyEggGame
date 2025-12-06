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
        float currentSpeed = _speed;
        
        currentSpeed *= GameManager.Instance.GlobalSpeedMultiplier;

        _offset.y -= currentSpeed * Time.deltaTime;
        _material.mainTextureOffset = _offset;    }
}
