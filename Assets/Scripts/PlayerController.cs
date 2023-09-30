using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform shadow;
    [SerializeField] private float shadowTransformOffset = 0.1f;

    [SerializeField] private Transform visual;
    [SerializeField] private Transform shadowVisual;

    [SerializeField] private CinemachineImpulseSource pickupShake;
    [SerializeField] private CinemachineImpulseSource dieShake;

    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem dieParticle;

    [SerializeField] private Vector2 shadowInitialOffset = new Vector2(0.08f, -0.08f);

    [SerializeField] private float jumpCooldown = 0.5f;

    private Rigidbody2D rb;

    private float timeSinceLastJump = 0;

    public bool isDead { private set; get; } = false;

    private Vector2 initialPos;

    [SerializeField] private float driftFactor = 0.95f;
    [SerializeField] private float accelarationFactor = 10.0f;
    [SerializeField] private float turnFactor = 3.5f;
    [SerializeField] private float maxSpeed = 10f;

    private float accelarationInput = 0;
    private float steeringInput = 0;
    private float velocityVsUp = 0;
    private float rotationAngle = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        initialPos = transform.position;

        timeSinceLastJump = jumpCooldown;
    }

    private void Update()
    {
        if (GameController.instance.isPaused) return;

        if (isDead)
        {
            if (Input.GetKeyDown(KeyCode.Space) && GameController.instance.canRestart)
            {
                Respawn();
            }

            return;
        }

        accelarationInput = Input.GetAxisRaw("Vertical");
        steeringInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && timeSinceLastJump >= jumpCooldown && !isDead && !GameController.instance.isPaused)
        {
            StartCoroutine(Jump());
            timeSinceLastJump = 0;
        }

        timeSinceLastJump += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDead || GameController.instance.isPaused) return;

        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();

        if (!isJumping) shadow.transform.position = (Vector2)transform.position + shadowInitialOffset;
        else shadow.transform.position = (Vector2)transform.position + new Vector2(shadowInitialOffset.x, shadowInitialOffset.y * 10f);
    }

    private void ApplyEngineForce()
    {
        velocityVsUp = Vector2.Dot(transform.up, rb.velocity);

        if (isJumping) return;

        if (velocityVsUp > maxSpeed && accelarationInput > 0)
            return;

        if (velocityVsUp < -maxSpeed * 0.5f && accelarationInput < 0)
            return;

        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelarationInput > 0)
            return;

        if (accelarationInput == 0)
        {
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else rb.drag = 0f;

        Vector2 engineForceVector = transform.up * accelarationInput * accelarationFactor;

        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        float minSpeedBeforeAllowTurningFactor = (rb.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        rotationAngle -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        rb.MoveRotation(rotationAngle);
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, rb.velocity);
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = GetLateralVelocity();
        isBraking = false;

        if (accelarationInput < 0 && velocityVsUp > 0)
        {
            isBraking = true;
            return true;
        }

        if (Mathf.Abs(GetLateralVelocity()) > 2.5f)
            return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Pickup pickup))
        {
            pickupShake.GenerateImpulse();
            pickup.Pick();
        }

        if (collision.transform.parent.TryGetComponent(out Enemy enemy))
        {
            if (enemy.destroyOnCollision)
                Destroy(enemy.gameObject);
            Die();
        }
    }

    private bool isJumping = false;

    private IEnumerator Jump()
    {
        IgnoreLayersOnJump(true);
        isJumping = true;

        visual.DOScale(new Vector2(1.5f, 1.5f), 0.25f);
        shadowVisual.DOScale(Vector2.one * 0.75f, 0.25f);

        yield return new WaitForSeconds(0.35f);

        visual.DOScale(Vector2.one, 0.1f);
        shadowVisual.DOScale(Vector2.one, 0.1f);

        yield return new WaitForSeconds(0.1f);

        jumpParticle.Play();
        IgnoreLayersOnJump(false);
        isJumping = false;
    }

    private void IgnoreLayersOnJump(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(3, 7, ignore); // Pickup layer
        Physics2D.IgnoreLayerCollision(3, 8, ignore); // Enemy layer
    }

    private void Die()
    {
        if (isDead) return;

        rb.velocity = Vector2.zero;
        accelarationInput = 0;
        steeringInput = 0;

        isDead = true;
        visual.GetComponent<SpriteRenderer>().enabled = false;
        shadowVisual.GetComponent<SpriteRenderer>().enabled = false;
        dieShake.GenerateImpulse();
        dieParticle.Play();

        GameController.instance.GameOver();
    }

    private void Respawn()
    {
        transform.position = initialPos;
        rotationAngle = 0f;

        visual.GetComponent<SpriteRenderer>().enabled = true;
        shadowVisual.GetComponent<SpriteRenderer>().enabled = true;

        isDead = false;
        GameController.instance.Restart();
    }
}
