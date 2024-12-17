using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlimeMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    public float baseVelocity = 10f;
    public float runMultiplier = 1.5f;
    public float jumpForce = 300f;

    private bool isGrounded = true;
    private bool isDead = false;
    private bool isHurt = false;

    // Health System
    public int maxHealth = 100;
    private int currentHealth;

    // UI Elements
    public Slider healthBar;
    public TMP_Text healthText;

    // Coin Collection
    private int coinCounter = 0;
    public TMP_Text counter_Text;

    // Audio Clips for SFX
    public AudioClip hurtSFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        UpdateHealthUI();

        coinCounter = 0;
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
        {
            TakeDamage(20); // Take damage when colliding with an enemy
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            audioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
            audioSource.Play();
            coinCounter++;
            UpdateCoinUI();
            Destroy(collision.gameObject);
        }
    }

    private void UpdateCoinUI()
    {
        if (counter_Text != null)
        {
            counter_Text.text = "Coins: " + coinCounter.ToString();
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            TriggerHurt();
            StartCoroutine(FlashEffect());
            PlaySFX(hurtSFX);
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
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
        animator.ResetTrigger("Hurt");
        animator.Play("MoveBlendTree");
    }

    private IEnumerator FlashEffect()
    {
        int flashCount = 4;
        float flashDuration = 0.1f;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Dead");
    }
}
