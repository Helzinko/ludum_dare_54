using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Pickup : GameEntity
{
    public bool isMovingFromPlayer = false;
    [SerializeField] private float moveSpeed = 75f;

    [SerializeField] private ParticleSystem explosionEffect;

    [SerializeField] private float scaleFactor = 1.5f;
    [SerializeField] private float scaleDuration = 0.5f;

    private Tween idleTween;

    [HideInInspector] public bool isEvil = false;

    private Rigidbody2D rb;
    private PlayerController player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Vector3 initialScale = transform.localScale;
        idleTween = transform.DOScale(initialScale * scaleFactor, scaleDuration).SetLoops(-1, LoopType.Yoyo);
    }

    private void FixedUpdate()
    {
        if (isMovingFromPlayer)
            rb.velocity = (transform.position - player.transform.position).normalized * moveSpeed * Time.deltaTime;
    }

    public void Pick()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation, null);
        GameController.instance.PickedPickup(isEvil, transform.position);
        idleTween.Kill();
        Destroy(gameObject);
    }
}