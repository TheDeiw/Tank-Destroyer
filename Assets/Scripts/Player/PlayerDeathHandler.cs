using System;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
   public event Action OnPlayerDied;
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
      OnPlayerDied?.Invoke();
   }
}
