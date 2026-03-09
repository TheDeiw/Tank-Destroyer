using UnityEngine;

namespace Enemy
{
    public interface IEnemyBehaviorStrategy
    {
        void Execute(Transform enemyTransform, Transform playerTransform, float speed, EnemyLogic enemyLogic);
    }

    public class ChasePlayerStrategy : IEnemyBehaviorStrategy
    {
        public void Execute(Transform enemyTransform, Transform playerTransform, float speed, EnemyLogic enemyLogic)
        {
            if (playerTransform)
            {
                enemyTransform.LookAt(playerTransform);
                Vector3 direction = (playerTransform.position - enemyTransform.position).normalized;
                enemyTransform.position += direction * (speed * Time.deltaTime);
            }
        }
    }

    public class EvadeStrategy : IEnemyBehaviorStrategy
    {
        public void Execute(Transform enemyTransform, Transform playerTransform, float speed, EnemyLogic enemyLogic)
        {
            if (playerTransform)
            {
                Vector3 directionAwayFromPlayer = (enemyTransform.position - playerTransform.position).normalized;
                enemyTransform.LookAt(playerTransform);
                enemyTransform.position += directionAwayFromPlayer * (speed * Time.deltaTime);
            }
        }
    }
}