using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISubscriber<TContext>
{
    void OnNotify(TContext context);
}


public struct PlayerHealthChangedEventArgs
{
    public int CurrentHealth;
    public int MaxHealth;

    public PlayerHealthChangedEventArgs(int current, int max)
    {
        CurrentHealth = current;
        MaxHealth = max;
    }
}

public struct EnemyDiedEventArgs
{
    public GameObject EnemyObject;

    public EnemyDiedEventArgs(GameObject enemy)
    {
        EnemyObject = enemy;
    }
}

public struct PlayerDiedEventArgs { }