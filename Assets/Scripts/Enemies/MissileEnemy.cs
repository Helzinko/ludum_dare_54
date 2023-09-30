using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileEnemy : Enemy
{
    [SerializeField] private Transform body;

    void Start()
    {
        direction = (Vector2.zero - (Vector2)transform.position).normalized;
        body.rotation = Quaternion.LookRotation(Vector3.forward, Vector2.zero - (Vector2)transform.position);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        transform.Translate(direction * speed * Time.deltaTime);
    }
}