using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq; 


[TestFixture]
public class BulletSystemIntegrationTests 
{
    private BulletConfiguration defaultConfig;

    [SetUp]
    public void Setup()
    {
        defaultConfig = new BulletConfiguration(10, 100f);
    }

    [Test]
    public void BulletConfiguration_Constructor_SetsPropertiesCorrectly()
    {
        int damage = 5;
        float speed = 50f;
        BulletConfiguration config = new BulletConfiguration(damage, speed);
        Assert.AreEqual(damage, config.Damage);
        Assert.AreEqual(speed, config.Speed, 0.001f);
    }
    
    [Test]
    public void StandardBullet_Constructor_SetsPropertiesFromConfig()
    {
        StandardBullet bullet = new StandardBullet(defaultConfig);
        Assert.AreEqual(defaultConfig.Damage, bullet.Damage);
        Assert.AreEqual(defaultConfig.Speed, bullet.Speed, 0.001f);
    }

    [Test]
    public void StandardBullet_Constructor_NullConfig_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new StandardBullet(null));
    }

    [Test]
    public void StandardBullet_Shoot_SetsDirection()
    {
        StandardBullet bullet = new StandardBullet(defaultConfig);
        Vector3 direction = Vector3.forward;
        bullet.Shoot(direction);
        Assert.AreEqual(direction, bullet.Direction);
    }
    
    [Test]
    public void BulletDecorator_Constructor_NullBullet_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DamageDecorator(null, 5));
    }
    

    [Test]
    public void DamageDecorator_ModifiesDamageCorrectly()
    {
        IBullet standardBullet = new StandardBullet(defaultConfig); // 10 damage
        int extraDamage = 5;
        DamageDecorator decoratedBullet = new DamageDecorator(standardBullet, extraDamage);

        Assert.AreEqual(defaultConfig.Damage + extraDamage, decoratedBullet.Damage);
        Assert.AreEqual(standardBullet.Speed, decoratedBullet.Speed, 0.001f);
        standardBullet.Shoot(Vector3.up);
        decoratedBullet.Shoot(Vector3.right);
        Assert.AreEqual(Vector3.right, decoratedBullet.Direction);
        Assert.AreEqual(Vector3.right, standardBullet.Direction);
    }
    

    [Test]
    public void SpeedDecorator_ModifiesSpeedCorrectly()
    {
        IBullet standardBullet = new StandardBullet(defaultConfig); // 100 speed
        float speedMultiplier = 1.5f;
        SpeedDecorator decoratedBullet = new SpeedDecorator(standardBullet, speedMultiplier);

        Assert.AreEqual(defaultConfig.Speed * speedMultiplier, decoratedBullet.Speed, 0.001f);
        Assert.AreEqual(standardBullet.Damage, decoratedBullet.Damage);
        standardBullet.Shoot(Vector3.left);
        decoratedBullet.Shoot(Vector3.down);
        Assert.AreEqual(Vector3.down, decoratedBullet.Direction);
        Assert.AreEqual(Vector3.down, standardBullet.Direction);
    }

    [Test]
    public void BulletFactory_CreateStandardBullet_ReturnsCorrectTypeAndProperties()
    {
        IBullet bullet = BulletFactory.CreateStandardBullet();
        bullet.Shoot(Vector3.one);

        Assert.IsInstanceOf<StandardBullet>(bullet);
        Assert.AreEqual(1, bullet.Damage);
        Assert.AreEqual(20f, bullet.Speed, 0.001f);
        Assert.AreEqual(Vector3.one, bullet.Direction);
    }

    [Test]
    public void BulletFactory_CreateEnhancedBullet_ReturnsCorrectTypeAndProperties()
    {
        IBullet bullet = BulletFactory.CreateEnhancedBullet();
        bullet.Shoot(Vector3.one);

        Assert.IsInstanceOf<DamageDecorator>(bullet);
        Assert.AreEqual(1 + 1, bullet.Damage);
        Assert.AreEqual(20f, bullet.Speed, 0.001f);
    }

    [Test]
    public void BulletFactory_CreateFastBullet_ReturnsCorrectTypeAndProperties()
    {
        IBullet bullet = BulletFactory.CreateFastBullet();
        bullet.Shoot(Vector3.one);

        Assert.IsInstanceOf<SpeedDecorator>(bullet);
        Assert.AreEqual(1, bullet.Damage);
        Assert.AreEqual(20f * 1.5f, bullet.Speed, 0.001f);
    }

    [Test]
    public void BulletFactory_GetConfiguration_CachesConfiguration()
    {
        MethodInfo getConfigurationMethod = typeof(BulletFactory)
            .GetMethod("GetConfiguration", BindingFlags.NonPublic | BindingFlags.Static);
        
        string uniqueKey = "UniqueTestCacheKey_" + Guid.NewGuid().ToString();

        BulletConfiguration config1 = (BulletConfiguration)getConfigurationMethod
            .Invoke(null, new object[] { uniqueKey, 5, 50f });

        BulletConfiguration config2 = (BulletConfiguration)getConfigurationMethod
            .Invoke(null, new object[] { uniqueKey, 5, 50f });

        Assert.AreSame(config1, config2, "GetConfiguration should return cached instance for the same key.");
    }

    [Test]
    public void BulletFactory_GetConfiguration_NullOrEmptyKey_ThrowsArgumentException()
    {
        MethodInfo getConfigurationMethod = typeof(BulletFactory)
            .GetMethod("GetConfiguration", BindingFlags.NonPublic | BindingFlags.Static);

        var exNull = Assert.Throws<TargetInvocationException>(() =>
            getConfigurationMethod.Invoke(null, new object[] { null, 5, 50f }),
            "Should throw when key is null"
        );
        Assert.IsInstanceOf<ArgumentException>(exNull.InnerException);

        var exEmpty = Assert.Throws<TargetInvocationException>(() =>
            getConfigurationMethod.Invoke(null, new object[] { "", 5, 50f }),
            "Should throw when key is empty"
        );
        Assert.IsInstanceOf<ArgumentException>(exEmpty.InnerException);
    }

    [Test]
    public void CombinedDecorators_PropertiesAreCorrect()
    {
        IBullet baseBullet = BulletFactory.CreateStandardBullet(); // 1 damage, 20 speed
        IBullet damageEnhanced = new DamageDecorator(baseBullet, 4); // 1+4=5 damage, 20 speed
        IBullet fastAndDamaged = new SpeedDecorator(damageEnhanced, 2.0f); // 5 damage, 20*2=40 speed

        fastAndDamaged.Shoot(Vector3.back);

        Assert.AreEqual(5, fastAndDamaged.Damage);
        Assert.AreEqual(40f, fastAndDamaged.Speed, 0.001f);
        Assert.AreEqual(Vector3.back, fastAndDamaged.Direction);
    }
}