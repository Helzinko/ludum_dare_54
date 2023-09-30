using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skidmarks : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        trailRenderer.emitting = false;
    }

    private void Update()
    {
        if (player.IsTireScreeching(out float lateralVelocity, out bool isBraking) && !player.isDead)
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }
}
