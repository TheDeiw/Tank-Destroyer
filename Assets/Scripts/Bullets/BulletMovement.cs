using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private IBullet bullet;

    public void SetBullet(IBullet bullet)
    {
        this.bullet = bullet;
    }

    public void SetDirection(Vector3 dir)
    {
        bullet.Shoot(dir);
    }

    void Update()
    {
        transform.position += bullet.Direction * bullet.Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            Debug.Log("Bullet destroyed by wall");
        }
    }
}
