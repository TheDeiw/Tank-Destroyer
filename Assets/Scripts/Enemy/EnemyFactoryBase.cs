using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyFactory
{
    GameObject CreateEnemy();
}

public abstract class EnemyFactoryBase : EnemyFactory
{
    protected IPrefabProvider prefabProvider;
    public EnemyFactoryBase(IPrefabProvider provider)
    {
        if (provider == null)
            throw new System.ArgumentNullException(nameof(provider), "Prefab provider cannot be null.");
        prefabProvider = provider;
    }

    public GameObject CreateEnemy()
    {
        var builder = new EnemyBuilder();
        ConfigureEnemy(builder);
        return builder.Build();
    }
    protected abstract void ConfigureEnemy(EnemyBuilder builder);
}

public class EasyEnemyFactory : EnemyFactoryBase
{
    public EasyEnemyFactory(IPrefabProvider provider) : base(provider) { }
    protected override void ConfigureEnemy(EnemyBuilder builder)
    {
        builder.SetSpeed(EnemyStats.SpeedByDifficulty[EnemyDifficulty.Easy]);
        builder.SetPrefab(prefabProvider.GetPrefab(0));
        builder.SetBehavior(new EvadeStrategy());
    }
}

public class HardEnemyFactory : EnemyFactoryBase
{
    public HardEnemyFactory(IPrefabProvider provider) : base(provider) { }
    protected override void ConfigureEnemy(EnemyBuilder builder)
    {
        builder.SetSpeed(EnemyStats.SpeedByDifficulty[EnemyDifficulty.Hard]);
        builder.SetPrefab(prefabProvider.GetPrefab(1));
        builder.SetBehavior(new ChasePlayerStrategy());
    }
}










//public interface EnemyFactory
//{
//    public GameObject CreateEnemy(GameObject prefab);
//}

//public class EasyEnemyFactory : EnemyFactory
//{
//    public GameObject CreateEnemy(GameObject prefab)
//    {
//        return new EnemyBuilder()
//            .SetPrefab(prefab)
//            .SetSpeed(3f)
//            .Build();
//    }
//}

//public class HardEnemyFactory : EnemyFactory
//{
//    public GameObject CreateEnemy(GameObject prefab)
//    {
//        return new EnemyBuilder()
//            .SetPrefab(prefab)
//            .SetSpeed(7f)
//            .Build();
//    }
//}