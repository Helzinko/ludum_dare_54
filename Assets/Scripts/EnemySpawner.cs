using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public void AddEnemy(Enemy enemy)
    {
        Enemy spawnedEnemy = Instantiate(enemy, enemy.SpawnPosition(), default, transform);
    }
}
