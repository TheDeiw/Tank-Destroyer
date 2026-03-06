using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public GameObject bulletPrefab;

    void Start()
    {
        InvokeRepeating("LaunchBullet", 0.5f, 1.5f);
    }

    //void LaunchBullet()
    //{
    //    Transform target = GameObject.FindGameObjectWithTag("Player").transform;
    //    Vector3 direction = (target.position - transform.position).normalized;
    //    Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

    //    // Створення базової кулі через фабрику
    //    IBullet bullet = BulletFactory.CreateStandardBullet(bulletPrefab, transform.position, rotation);

    //    // Приклад декоратора (опціонально)
    //    // IBullet enhancedBullet = new DamageDecorator(bullet, 1);
    //    GameObject bulletObj = Instantiate(bulletPrefab, transform.position, rotation);
    //    //GameObject bulletObj = bullet.BulletInstance;
    //    BulletMovement movement = bulletObj.GetComponent<BulletMovement>();
    //    movement.SetBullet(bullet);
    //    movement.SetDirection(direction);
    //}

    void LaunchBullet()
    {
        Transform target = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

        // Створення IBullet через фабрику
        IBullet bullet = BulletFactory.CreateStandardBullet();

        // Опціонально: декорування кулі
        // IBullet bullet = BulletFactory.CreateEnhancedBullet();
        // IBullet bullet = BulletFactory.CreateFastBullet();

        // Створення GameObject з префаба
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, rotation);
        BulletMovement movement = bulletObj.GetComponent<BulletMovement>();

        if (movement != null)
        {
            movement.SetBullet(bullet);
            movement.SetDirection(direction);
        }
        else
        {
            Debug.LogError("BulletMovement component not found on bullet prefab.");
        }
    }
}
