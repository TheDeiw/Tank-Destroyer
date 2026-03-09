using Bullets;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    private Transform target;
    

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        InvokeRepeating("LaunchBullet", 0.5f, 1.5f);
    }
    

    void LaunchBullet()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, rotation);
        Bullet logic = bulletObj.GetComponent<Bullet>();
        logic.InitializeDirection(direction);
        
    }
}
