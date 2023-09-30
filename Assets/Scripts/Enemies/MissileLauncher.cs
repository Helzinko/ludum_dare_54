using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : Enemy
{
    [SerializeField] private MissileEnemy missile;

    public int missilesToLaunch = 1;
    [SerializeField] private float missileSpawnRadius = 9f;
    [SerializeField] private float missileSpawnRate = 2f;

    private void Start()
    {
        StartCoroutine(MissileSpawnLoop());
    }

    IEnumerator MissileSpawnLoop()
    {
        while (true)
        {
            for (int i = 0; i < missilesToLaunch; i++)
            {
                Enemy spawnedMissile = Instantiate(missile, SpawnPosition(), default, transform);
            }
            yield return new WaitForSeconds(missileSpawnRate);
        }
    }

    public override Vector2 SpawnPosition()
    {
        float randomAngle = Random.Range(0.0f, 1.0f) * 2 * 3.14f;

        var randomX = missileSpawnRadius * Mathf.Cos(randomAngle);
        var randomY = missileSpawnRadius * Mathf.Sin(randomAngle);
        var randomPosition = new Vector2(randomX, randomY);

        return randomPosition;
    }
}