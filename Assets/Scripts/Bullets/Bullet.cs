using UnityEngine;

namespace Bullets
{
    public class Bullet : MonoBehaviour
    {
        //[SerializeField] private int damage;
        [SerializeField] private float speed;
        [SerializeField] private float lifetime = 5f;
        private Vector3 Direction { get; set; }
    
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
    
    }
}
