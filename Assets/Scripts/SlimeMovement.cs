using System.Collections;
using UnityEngine;
using TMPro;

public class SlimeMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public float baseVelocity = 10f;
    public float runMultiplier = 1.5f;
    public float jumpForce = 300f;

    private bool isGrounded = true;
    private bool isDead = false;
    private bool isHurt = false;

    // Coin Collection
    private int coinCounter = 0; // Tracks collected coins
    public TMP_Text counter_Text;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        coinCounter = 0;
        UpdateCoinUI();
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    void Update()
    {
        if (isDead || isHurt) return;

        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed = baseVelocity * (isRunning ? runMultiplier : 1f);
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isGrounded", isGrounded);

        if (horizontal != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontal), 1, 1);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            animator.SetTrigger("Jump");
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("Enemy"))
            TriggerHurt();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin")) // Detect the coin
        {
            coinCounter++; // Increment coin count
            UpdateCoinUI(); // Update UI
            Destroy(collision.gameObject); // Remove the coin from the scene
        }
    }

    private void UpdateCoinUI()
    {
        if (counter_Text != null)
        {
            counter_Text.text = "Coins: " + coinCounter.ToString();
        }
    }

    private void TriggerHurt()
    {
        if (isHurt || isDead) return;

        isHurt = true;
        animator.SetTrigger("Hurt");
        StartCoroutine(RecoverFromHurt());
    }

    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(1.0f);
        isHurt = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Dead");
    }
}
