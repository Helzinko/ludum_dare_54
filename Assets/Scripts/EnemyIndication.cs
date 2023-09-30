using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyIndication : MonoBehaviour
{
    [SerializeField] private float scaleFactor = 1.5f;
    [SerializeField] private float scaleDuration = 0.5f;

    private Tween idleTween;
    void Start()
    {
        Vector3 initialScale = transform.localScale;
        idleTween = transform.DOScale(initialScale * scaleFactor, scaleDuration).SetLoops(-1, LoopType.Yoyo);
    }
}
