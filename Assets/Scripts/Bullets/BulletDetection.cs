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
    [SerializeField] private AudioSource hitSound;
    
    private bool isDead = false;

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
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
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
    }
}

