using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDetection : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public HealthBar healthBar;

    public ParticleSystem explosionEffect;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }
    void Update()
    {
        if (currentHealth <= 0)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Debug.Log("Enemy destroyed by bullet");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            currentHealth--;
            if (healthBar != null)
            {
                healthBar.SetHealth(currentHealth);
            }
        }
        //Destroy(other.gameObject);
        //// Припустимо, що шкода залежить від типу кулі
        //int damage = other.gameObject.GetComponent<IBullet>().Damage;
        //GetComponent<PlayerHealth>().TakeDamage(damage); // Для гравця
        //// Або для ворога: currentHealth -= damage;
    }
}

