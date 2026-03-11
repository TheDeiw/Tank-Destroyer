using UnityEngine;

namespace Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float speed;
        [SerializeField] private float lifetime = 5f;
        private Vector3 Direction { get; set; }

        public int Damage => damage;

        void Start()
        {
            Destroy(gameObject, lifetime);
        }

        public void InitializeDirection(Vector3 dir)
        {
            Direction = dir;
        }

        void Update()
        {
            transform.position += Direction * (speed * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
