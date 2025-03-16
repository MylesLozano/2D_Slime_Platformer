using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private GameObject pointA; // Serialized to show in the inspector
    [SerializeField] private GameObject pointB; // Serialized to show in the inspector
    [SerializeField] private bool startAtPointB = true;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float switchThreshold = 0.1f;

    [Header("Combat Settings")]
    [SerializeField] private float bounceForce = 10f;

    [Header("Effects")]
    [SerializeField] private GameObject deathEffectPrefab;

    // Component references
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    private AudioSource audioSource;

    // State tracking
    private Vector3 originalScale;
    private bool isDead = false;

    private void Awake()
    {
        // Ensure patrol points are assigned
        if (pointA == null || pointB == null)
        {
            Debug.LogError($"Patrol points (pointA and pointB) are not set on {gameObject.name}. Disabling EnemyPatrol script.");
            this.enabled = false; // Disable the script if patrol points are not set
            return;
        }

        // Get component references
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        originalScale = transform.localScale;

        // Set initial patrol point
        currentPoint = startAtPointB ? pointB.transform : pointA.transform;
        anim.SetBool("isRunning", true);
    }

    void Update()
    {
        if (!isDead)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        Vector2 direction = (currentPoint.position - transform.position).normalized;

        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        // Flip sprite based on movement direction
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Check if we've reached the destination point
        if (Mathf.Abs(transform.position.x - currentPoint.position.x) < switchThreshold)
        {
            SwitchDirection();
        }
    }

    private void SwitchDirection()
    {
        currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StompTrigger") && !isDead)
        {
            // Log for debugging
            Debug.Log("StompTrigger hit!");

            // Kill the enemy
            Die();

            // Find the player and apply bounce
            ApplyPlayerBounce(collision);
        }
    }

    private void ApplyPlayerBounce(Collider2D collision)
    {
        // Get player rigidbody (from parent since StompTrigger is typically a child object)
        Rigidbody2D playerRb = collision.transform.parent.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            // Apply bounce force
            playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("isRunning", false);
        anim.SetTrigger("Dead");

        // Disable collision
        GetComponent<Collider2D>().enabled = false;

        // Spawn death effect if available
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Destroy after animation plays
        Destroy(gameObject, 1f);
    }
}