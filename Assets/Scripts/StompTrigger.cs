using UnityEngine;

public class StompTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip stompSFX; // Stomp sound effect
    private AudioSource audioSource;

    private void Start()
    {
        // Find or add an AudioSource on the parent object
        audioSource = GetComponentInParent<AudioSource>() ?? GetComponentInParent<Transform>().gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit the StompTrigger!"); // Debugging line
            EnemyPatrol enemy = GetComponentInParent<EnemyPatrol>();
            if (enemy != null)
            {
                enemy.Die();

                // Bounce the player upwards
                Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
                }

                // Play the stomp sound effect
                if (stompSFX != null && audioSource != null)
                {
                    audioSource.PlayOneShot(stompSFX);
                }
            }
        }
    }
}