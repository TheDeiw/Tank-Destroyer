using UnityEngine;

namespace Enemy
{
    public interface IEnemyFactory
    {
        GameObject CreateEnemy();
    }

    public abstract class EnemyFactory : IEnemyFactory
    {
        protected readonly IPrefabProvider prefabProvider;

        protected EnemyFactory(IPrefabProvider provider)
        {
            prefabProvider = provider ?? throw new System.ArgumentNullException(nameof(provider), "Prefab provider cannot be null.");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public GameObject CreateEnemy()
        {
            var (prefab, speed, strategy) = GetEnemyConfig();
            GameObject enemy = Object.Instantiate(prefab);
            var logic = enemy.GetComponent<EnemyLogic>();
            logic.speed = speed;
            logic.behaviorStrategy = strategy;
            return enemy;
        }
        protected abstract (GameObject prefab, float speed, IEnemyBehaviorStrategy strategy) GetEnemyConfig();
    }

    public class EasyEnemyFactory : EnemyFactory
    {
        public EasyEnemyFactory(IPrefabProvider provider) : base(provider) { }

        protected override (GameObject prefab, float speed, IEnemyBehaviorStrategy strategy) GetEnemyConfig()
        {
            var speed = EnemyStats.SpeedByDifficulty[EnemyDifficulty.Easy];
            var prefab = prefabProvider.GetPrefab(0);
            var strategy = new EvadeStrategy();
            return (prefab, speed, strategy);
        }
    }

    public class HardEnemyFactory : EnemyFactory
    {
        public HardEnemyFactory(IPrefabProvider provider) : base(provider) { }
        protected override (GameObject prefab, float speed, IEnemyBehaviorStrategy strategy) GetEnemyConfig()
        {
            var speed = EnemyStats.SpeedByDifficulty[EnemyDifficulty.Hard];
            var prefab = prefabProvider.GetPrefab(1);
            var strategy = new ChasePlayerStrategy();
            return (prefab, speed, strategy);
        }
    }
}