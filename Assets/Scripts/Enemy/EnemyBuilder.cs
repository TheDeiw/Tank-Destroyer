using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyBuilder : MonoBehaviour
{
    private GameObject enemyPrefab;
    private float speed;
    private IEnemyBehaviorStrategy behaviorStrategy;

    public EnemyBuilder SetPrefab(GameObject prefab)
    {
        enemyPrefab = prefab;
        return this;
    }

    public EnemyBuilder SetSpeed(float speed)
    {
        if (speed < 0)
        {
            speed = EnemyStats.SpeedByDifficulty[EnemyDifficulty.Default];
        }
        this.speed = speed;
        return this;
    }

    public EnemyBuilder SetBehavior(IEnemyBehaviorStrategy strategy)
    {
        this.behaviorStrategy = strategy;
        return this;
    }

    public GameObject Build()
    {
        if (enemyPrefab == null)
        {
            throw new System.InvalidOperationException("Cannot build enemy: Prefab is not set.");
        }

        GameObject enemy = Object.Instantiate(enemyPrefab);
        var enemyLogic = enemy.GetComponent<EnemyLogic>();
        if (enemyLogic == null)
        {
            throw new System.InvalidOperationException("Enemy prefab does not have EnemyLogic component.");
        }

        enemyLogic.speed = speed;

        if (behaviorStrategy == null)
        {
            behaviorStrategy = new ChasePlayerStrategy(); // Default behavior
        }
        enemyLogic.behaviorStrategy = behaviorStrategy;

        return enemy;
    }
}