using UnityEngine;

public class Shredder : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.name);
        Destroy(collision.gameObject);
    }
}
