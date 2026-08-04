using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
   [SerializeField] private ParticleSystem explosionEffect;
   private HealthComponent health;
   [SerializeField] private AudioClip deathSound;

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
      if (deathSound != null)
         AudioSource.PlayClipAtPoint(deathSound, transform.position);
      Destroy(gameObject);

      if (GameManager.Instance != null)
      {
         GameManager.Instance.AddKill();
      }
   }
}
