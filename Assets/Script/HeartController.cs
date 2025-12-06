using UnityEngine;

public class HeartController : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float rotationSpeed = 100f;

    void Update()
    {
        transform.Translate(Vector2.down * (fallSpeed * Time.deltaTime), Space.World);
        
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (transform.position.y < -15f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.HealFull();
        }
        Destroy(gameObject);
    }
}