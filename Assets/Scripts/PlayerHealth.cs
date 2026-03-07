using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthBar healthBar;

    private int maxHealthValue;
    private int currentHealthValue;
    private bool isDead = false;

    public event Action OnPlayerDied;

    public void InitializeHealth(int maxHealth)
    {
        maxHealthValue = maxHealth;
        currentHealthValue = maxHealth;
        isDead = false;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealthValue -= damage;
        currentHealthValue = Mathf.Clamp(currentHealthValue, 0, maxHealthValue);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealthValue);
        }

        if (currentHealthValue <= 0)
        {
            isDead = true;
            OnPlayerDied?.Invoke();
        }
    }

    public bool IsAlive() => !isDead && currentHealthValue > 0;
}