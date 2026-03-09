using UnityEngine;

namespace Enemy
{
    public class EnemyLogic : MonoBehaviour
    {
        private Transform target;
        public float speed;
        public IEnemyBehaviorStrategy behaviorStrategy;

        private bool isDead = false;

        void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        void Update()
        {
            if (target != null && behaviorStrategy != null && !isDead)
            {
                behaviorStrategy.Execute(transform, target, speed, this);
            }
        }
    }
}
