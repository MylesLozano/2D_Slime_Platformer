using System.Collections;
using UnityEngine;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead || isHurt) return;

        HandleMovementInput();
        HandleJumpInput();
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        if (!isDead && !isHurt)
        {
            // Handle movement directly
            float horizontalVal = Input.GetAxisRaw("Horizontal");
            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            float velocityX = horizontalVal * baseVelocity * (isRunning ? runMultiplier : 1f);
            rb.velocity = new Vector2(velocityX, rb.velocity.y);
        }
    }

    private void HandleMovementInput()
    {
        float horizontalVal = Input.GetAxisRaw("Horizontal");
        bool isWalking = horizontalVal != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Update animation states
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isRunning", isWalking && isRunning);

        // Flip sprite direction
        if (isWalking)
            transform.localScale = new Vector3(Mathf.Sign(horizontalVal), 1, 1);

        // Calculate velocity
        float velocityX = horizontalVal * baseVelocity * (isRunning ? runMultiplier : 1f);
        rb.velocity = new Vector2(velocityX, rb.velocity.y);
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            animator.SetTrigger("Jump");
            isGrounded = false;
        }
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TriggerHurt();
        }
    }

    private void TriggerHurt()
    {
        if (isHurt || isDead) return;

        isHurt = true;
        animator.SetTrigger("Hurt");
        rb.velocity = Vector2.zero;
        StartCoroutine(RecoverFromHurt());
    }

    private IEnumerator RecoverFromHurt()
    {
        yield return new WaitForSeconds(1f);
        isHurt = false;
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Dead");
        rb.velocity = Vector2.zero;
    }
}
