using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : Enemy
{
    [SerializeField] private Transform body;

    [SerializeField] private float spawnDistance = 8f;

    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private Transform upHolder;
    [SerializeField] private Transform downHolder;

    [SerializeField] private float stoppingXpos = 6f;
    public float restTime = 2f;

    public void Start()
    {
        direction = (Vector2.zero - (Vector2)transform.position).normalized;

        StartCoroutine(Movement());
    }

    public override Vector2 SpawnPosition()
    {
        var randomSign = Random.Range(0, 2) * 2 - 1;
        Vector2 randomPos = new Vector2(randomSign * spawnDistance, 0);
        return randomPos;
    }

    public override void Update()
    {
        base.Update();
        laserRenderer.SetPosition(0, upHolder.position);
        laserRenderer.SetPosition(1, downHolder.position);
    }

    private IEnumerator Movement()
    {
        while (true)
        {
            if (((direction.x > 0 && transform.position.x < stoppingXpos) ||
    direction.x < 0 && transform.position.x > -stoppingXpos))
            {
                transform.Translate(direction * speed * Time.deltaTime);
            }
            else
            {
                yield return new WaitForSeconds(restTime);
                direction.x *= -1f;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}