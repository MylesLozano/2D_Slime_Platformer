using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;

    public float speed = 2f;
    public float switchThreshold = 0.1f;

    private Vector3 originalScale;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalScale = transform.localScale;

        currentPoint = pointB.transform;
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

        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        if (Mathf.Abs(transform.position.x - currentPoint.position.x) < switchThreshold)
        {
            currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StompTrigger") && !isDead)
        {
            Debug.Log("StompTrigger hit!"); // Debugging line
            Die();

            // Bounce the player upwards
            Rigidbody2D playerRb = collision.transform.parent.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("isRunning", false);
        anim.SetTrigger("Dead");

        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
    }
}
