﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlimeMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseVelocity = 10f;
    public float runMultiplier = 1.5f;
    public float jumpForce = 300f;
    public float coyoteTime = 0.1f; // Time in seconds to allow jumping after leaving the ground

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI References")]
    public Slider healthBar;
    public TMP_Text healthText;
    public TMP_Text counter_Text;
    public GameObject deathScreen;
    public Button restartButton;
    public Button menuButton;

    [Header("Audio")]
    public AudioClip hurtSFX;
    public AudioClip jumpSFX;
    public AudioClip coinSFX;

    [Header("Level Management")]
    public int currentLevel;

    // Component references
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    // State tracking
    private bool isGrounded = true;
    private bool isDead = false;
    private bool isHurt = false;
    private int coinCounter = 0;
    private float coyoteTimeCounter; // Tracks the remaining coyote time

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        currentHealth = maxHealth;
        UpdateHealthUI();

        LoadCoins();
        InitializeUI();
    }

    private void LoadCoins()
    {
        if (PlayerPrefs.HasKey("CoinCount"))
        {
            coinCounter = PlayerPrefs.GetInt("CoinCount");
        }
        else
        {
            coinCounter = 0;
        }
        UpdateCoinUI();
    }

    private void InitializeUI()
    {
        if (deathScreen != null)
            deathScreen.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
        if (menuButton != null)
            menuButton.onClick.AddListener(BackToMenu);
    }

    void Update()
    {
        if (isDead || isHurt) return;

        HandleMovement();
        HandleJump();

        // Update coyote time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time when not grounded
        }
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
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || coyoteTimeCounter > 0))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // Reset vertical velocity before jumping
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
            isGrounded = false;
            coyoteTimeCounter = 0; // Reset coyote time after jumping

            if (jumpSFX != null)
                PlaySFX(jumpSFX);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("KillZone"))
        {
            currentHealth = 0;
            UpdateHealthUI();
            Die();
        }

        if (collision.CompareTag("Coin"))
        {
            audioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
            if (coinSFX != null)
                PlaySFX(coinSFX);
            else
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

    public void SaveCoinsOnLevelComplete()
    {
        PlayerPrefs.SetInt("CoinCount", coinCounter);
        PlayerPrefs.Save();
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

        DisableMovement();
        UnlockAllPreviousLevels(); // Unlocks all previous levels!

        if (deathScreen != null)
            deathScreen.SetActive(true);

        PlayerPrefs.DeleteKey("CoinCount");
        PlayerPrefs.Save();
    }

    private void DisableMovement()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = false;
    }

    public void RestartLevel()
    {
        ResetPlayer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        UnlockAllPreviousLevels(); // Unlock before going to menu
        ResetPlayer();
        SceneManager.LoadScene("StartMenu");
    }

    private void ResetPlayer()
    {
        Time.timeScale = 1f;
    }

    private void UnlockAllPreviousLevels()
    {
        for (int i = 1; i <= currentLevel; i++)
        {
            PlayerPrefs.SetInt($"Level{i}Unlocked", 1);
        }
        PlayerPrefs.Save();
    }
}