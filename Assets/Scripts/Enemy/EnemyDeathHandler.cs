using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
   [SerializeField] private ParticleSystem explosionEffect;
   private HealthComponent health;

   void Awake()
   {
      health = GetComponent<HealthComponent>();
      health.OnDied += HandleDeath;
   }

   void OnDestroy()
   {
      if (health != null)
         health.OnDied -= HandleDeath;
   }

   private void HandleDeath()
   {
      if (explosionEffect != null)
         Instantiate(explosionEffect, transform.position, Quaternion.identity);

      Destroy(gameObject);
   }
}
