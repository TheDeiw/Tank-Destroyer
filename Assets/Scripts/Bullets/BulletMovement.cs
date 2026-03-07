using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    private BulletData bullet;

    public void SetBullet(BulletData bullet)
    {
        this.bullet = bullet;
    }

    public void SetDirection(Vector3 direction)
    {
        bullet.SetDirection(direction);
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
        }
    }
}
