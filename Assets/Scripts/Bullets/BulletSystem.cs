//using System.Collections.Generic;
//using UnityEngine;
//public class BulletConfiguration
//{
//    public int Damage { get; }
//    public float Speed { get; }

//    public BulletConfiguration(int damage, float speed)
//    {
//        Damage = damage;
//        Speed = speed;
//    }
//}

//public interface IBullet
//{
//    void Shoot(Vector3 direction);
//    int Damage { get; }
//    float Speed { get; }
//    Vector3 Direction { get; }
//}

//public class StandardBullet : IBullet
//{
//    private BulletConfiguration config;
//    public Vector3 Direction { get; private set; }

//    public StandardBullet(BulletConfiguration config)
//    {
//        this.config = config;
//    }

//    public int Damage => config.Damage;
//    public float Speed => config.Speed;

//    public void Shoot(Vector3 direction)
//    {
//        Direction = direction;
//    }
//}

//public abstract class BulletDecorator : IBullet
//{
//    protected IBullet bullet;

//    public BulletDecorator(IBullet bullet)
//    {
//        this.bullet = bullet;
//    }

//    public virtual void Shoot(Vector3 direction)
//    {
//        bullet.Shoot(direction);
//    }

//    public virtual int Damage => bullet.Damage;
//    public virtual float Speed => bullet.Speed;
//    public virtual Vector3 Direction => bullet.Direction;
//}

//public class DamageDecorator : BulletDecorator
//{
//    private int extraDamage;

//    public DamageDecorator(IBullet bullet, int extraDamage) : base(bullet)
//    {
//        this.extraDamage = extraDamage;
//    }

//    public override int Damage => bullet.Damage + extraDamage;
//}

//public class SpeedDecorator : BulletDecorator
//{
//    private float speedMultiplier;

//    public SpeedDecorator(IBullet bullet, float speedMultiplier) : base(bullet)
//    {
//        this.speedMultiplier = speedMultiplier;
//    }

//    public override float Speed => bullet.Speed * speedMultiplier;
//}

//public static class BulletFactory
//{
//    private static Dictionary<string, BulletConfiguration> configCache = new Dictionary<string, BulletConfiguration>();

//    private static BulletConfiguration GetConfiguration(string key, int damage, float speed)
//    {
//        if (!configCache.ContainsKey(key))
//        {
//            configCache[key] = new BulletConfiguration(damage, speed);
//        }
//        return configCache[key];
//    }

//    public static IBullet CreateStandardBullet()
//    {
//        BulletConfiguration config = GetConfiguration("Standard", 1, 20f);
//        return new StandardBullet(config);
//    }

//    public static IBullet CreateEnhancedBullet()
//    {
//        IBullet baseBullet = CreateStandardBullet();
//        return new DamageDecorator(baseBullet, 1);
//    }

//    public static IBullet CreateFastBullet()
//    {
//        IBullet baseBullet = CreateStandardBullet();
//        return new SpeedDecorator(baseBullet, 1.5f);
//    }
//}




using System;
using System.Collections.Generic;
using UnityEngine;

// Переконайтеся, що BulletEffectType.cs та BulletEffectAttribute.cs доступні

public class BulletConfiguration
{
    public int Damage { get; }
    public float Speed { get; }

    public BulletConfiguration(int damage, float speed)
    {
        Damage = damage;
        Speed = speed;
    }
}

public interface IBullet
{
    void Shoot(Vector3 direction);
    int Damage { get; }
    float Speed { get; }
    Vector3 Direction { get; }
}

// StandardBullet не є декоратором, але може мати базовий ефект, якщо потрібно
// Або можна вважати, що декоратори додають ефекти *поверх* базової поведінки
[BulletEffect(BulletEffectType.None, "Standard projectile with no special effects.")]
public class StandardBullet : IBullet
{
    private BulletConfiguration config;
    public Vector3 Direction { get; private set; }

    public StandardBullet(BulletConfiguration config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        this.config = config;
    }

    public int Damage => config.Damage;
    public float Speed => config.Speed;

    public void Shoot(Vector3 direction)
    {
        Direction = direction;
    }
}

// Абстрактний декоратор може не мати конкретного ефекту,
// або можна визначити загальний, якщо це має сенс.
// Тут ми його не будемо позначати, оскільки ефекти стосуються конкретних реалізацій.
public abstract class BulletDecorator : IBullet
{
    protected IBullet bullet;

    public BulletDecorator(IBullet bullet)
    {
        if (bullet == null) throw new ArgumentNullException(nameof(bullet));
        this.bullet = bullet;
    }

    public virtual void Shoot(Vector3 direction)
    {
        bullet.Shoot(direction);
    }

    public virtual int Damage => bullet.Damage;
    public virtual float Speed => bullet.Speed;
    public virtual Vector3 Direction => bullet.Direction;
}

[BulletEffect(BulletEffectType.DamageModification, "Increases bullet's base damage.", Priority = 10)]
public class DamageDecorator : BulletDecorator
{
    private int extraDamage;

    public DamageDecorator(IBullet bullet, int extraDamage) : base(bullet)
    {
        this.extraDamage = extraDamage;
    }

    public override int Damage => bullet.Damage + extraDamage;
}

[BulletEffect(BulletEffectType.SpeedModification, "Modifies bullet's travel speed.", Priority = 5)]
public class SpeedDecorator : BulletDecorator
{
    private float speedMultiplier;

    public SpeedDecorator(IBullet bullet, float speedMultiplier) : base(bullet)
    {
        this.speedMultiplier = speedMultiplier;
    }

    public override float Speed => bullet.Speed * speedMultiplier;
}

public static class BulletFactory
{
    private static Dictionary<string, BulletConfiguration> configCache = new Dictionary<string, BulletConfiguration>();

    private static BulletConfiguration GetConfiguration(string key, int damage, float speed)
    {
        if (string.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        if (!configCache.ContainsKey(key))
        {
            configCache[key] = new BulletConfiguration(damage, speed);
        }
        return configCache[key];
    }

    public static IBullet CreateStandardBullet()
    {
        BulletConfiguration config = GetConfiguration("Standard", 1, 20f);
        return new StandardBullet(config);
    }

    public static IBullet CreateEnhancedBullet()
    {
        IBullet baseBullet = CreateStandardBullet();
        return new DamageDecorator(baseBullet, 1);
    }

    public static IBullet CreateFastBullet()
    {
        IBullet baseBullet = CreateStandardBullet();
        return new SpeedDecorator(baseBullet, 1.5f);
    }
}