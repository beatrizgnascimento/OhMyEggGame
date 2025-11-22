using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    private BoxCollider2D _feet;
    
    [SerializeField] float _speed;
    [SerializeField] private float _jumpForce;

    private Vector2 _input;
    private PlayerController playerController;
    private bool isGrounded = false;
    private bool wasGrounded = true;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _feet = GetComponentInChildren<BoxCollider2D>();
        playerController = GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        Move();
        
        isGrounded = _feet.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (!isGrounded)
        {
            _rigidbody.gravityScale = 7;
        }
        else
        {
            _rigidbody.gravityScale = 25;
        }

        UpdatePlayerState();
        
        wasGrounded = isGrounded;
    }

    void OnMove(InputValue inputValue)
    {
        _input = inputValue.Get<Vector2>();
    }

    void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed && isGrounded)
        {
            print("Jump");
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
    }

    void UpdatePlayerState()
    {
        if (playerController == null) 
        {
            Debug.LogError("PlayerController não encontrado!");
            return;
        }

        // Se estava no chão e agora está no ar (pulou)
        if (wasGrounded && !isGrounded)
        {
            playerController.SetPlayerState(PlayerController.PlayerState.Pulando);
        }
        // Se estava no ar e agora está no chão (aterrissou)
        else if (!wasGrounded && isGrounded)
        {
            playerController.SetPlayerState(PlayerController.PlayerState.Normal);
        }
        // Se está no chão e não está se movendo verticalmente
        else if (isGrounded && Mathf.Abs(_rigidbody.linearVelocity.y) < 0.1f)
        {
            playerController.SetPlayerState(PlayerController.PlayerState.Normal);
        }
    }
}