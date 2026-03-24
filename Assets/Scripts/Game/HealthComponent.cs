using System;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamageable
{
   [SerializeField] private int maxHealth = 100;
   [SerializeField] private HealthBar healthBar;
   [SerializeField] private AudioSource hitSound;

   private int currentHealth;
   private bool isDead = false;

   public event Action OnDied;
   public event Action<int, int> OnHealthChanged;
   
   public int CurrentHealth => currentHealth;
   public int MaxHealth => maxHealth;

   void Start()
   {
      currentHealth = maxHealth;
      healthBar?.SetMaxHealth(maxHealth);
   }

   public void TakeDamage(int damage)
   {
      if (isDead) return;

      currentHealth -= damage;
      currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

      if (hitSound != null) 
      {
         hitSound.Play();
      }
      healthBar?.SetHealth(currentHealth);
      OnHealthChanged?.Invoke(currentHealth, maxHealth);

      if (currentHealth <= 0)
      {
         isDead = true;
         OnDied?.Invoke();
      }
   }
}
