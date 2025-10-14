using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    private BoxCollider2D _feet;
    
    private Animator _animator;
    
    [SerializeField] float _speed;
    [SerializeField] private float _jumpForce;

    private Vector2 _input;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _feet = GetComponentInChildren<BoxCollider2D>();
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
        
        var isOnAir = !_feet.IsTouchingLayers(LayerMask.GetMask("Ground"));
        _animator.SetBool("isOnAir", isOnAir);

        if (isOnAir)
        {
            _rigidbody.gravityScale = 10;
            _animator.SetBool("isWalking", false);
        }
        else
        {
            _rigidbody.gravityScale = 30;
        }
    }

    void OnMove(InputValue inputValue)
    {
        _input = inputValue.Get<Vector2>();
        _animator.SetBool("isWalking", _input.x != 0 || _input.y != 0);
    }

    void OnJump(InputValue inputValue)
    {
        if (!inputValue.isPressed) return;
        
        var isGrounded = _feet.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (isGrounded)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnQuit(InputValue inputValue)
    {
        print("Quit");
        Application.Quit();
    }

    void Move()
    {
        _rigidbody.linearVelocity = new Vector2(_input.x * (_speed * Time.fixedDeltaTime), _rigidbody.linearVelocity.y);
        // _input * (_speed * Time.fixedDeltaTime);
    }
}
