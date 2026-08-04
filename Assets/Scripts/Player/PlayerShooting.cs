using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bullets;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject tankGun;
    private GameObject[] enemies;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private PlayerMovement playerMovement;
    private Vector3 direction;
    [SerializeField] private GameObject bulletSpawner;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime;

    [SerializeField] private AudioSource shootSound;
    [SerializeField] private ParticleSystem muzzleEffect;

    void Update()
    {
        GameObject closest = FindTheClosest();
        GunRotation(closest);

        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        Quaternion direction = tankGun.transform.rotation;
        Quaternion rotation = direction * Quaternion.Euler(90, 0, 0);

        Vector3 shootDirection = tankGun.transform.forward;
        
        GameObject bulletObj = Instantiate(bulletPrefab, bulletSpawner.transform.position, rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.InitializeDirection(shootDirection);
        Instantiate(muzzleEffect, bulletSpawner.transform.position, rotation);
        shootSound.Play();
    }

    private GameObject FindTheClosest()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = enemies
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .FirstOrDefault();
        return closest;
    }

    private void GunRotation(GameObject closest)
    {
        if (closest != null)
        {
            direction = closest.transform.position - tankGun.transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                tankGun.transform.rotation = Quaternion.Lerp(tankGun.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            direction = playerMovement.moveDirectionForGun;
            Quaternion rotation = Quaternion.LookRotation(direction);
            tankGun.transform.rotation = Quaternion.Lerp(tankGun.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }
}
