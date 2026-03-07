using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletData
{
    private int Damage { get; }
    private float Speed { get; }
    private Vector3 Direction { get; set; }

    public BulletData(int damage, float speed)
    {
        Damage = damage;
        Speed = speed;
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }
}