using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    private Transform target;
    public float speed;
    public IEnemyBehaviorStrategy behaviorStrategy;

    private bool isDead = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        //if (behaviorStrategy == null)
        //{
        //    SetBehaviorStrategy(new ChasePlayerStrategy());
        //}
    }

    //public void SetBehaviorStrategy(IEnemyBehaviorStrategy newStrategy)
    //{
    //    this.behaviorStrategy = newStrategy;
    //}

    void Update()
    {
        if (target != null && behaviorStrategy != null && !isDead)
        {
            behaviorStrategy.Execute(transform, target, speed, this);
        }
    }
}
