using System;
using UnityEngine;

public interface IPlayerState
{
    void OnEnter(PlayerStateManager manager);
    void OnUpdate();
    void OnExit();
    void HandleDamage(ref int damage);
    float GetSpeedMultiplier();
}

public class NormalPlayerState : IPlayerState
{
    private PlayerStateManager manager;
    public void OnEnter(PlayerStateManager manager) { this.manager = manager; Debug.Log("Player entered Normal State"); }
    public void OnUpdate() { }
    public void OnExit() { }
    public void HandleDamage(ref int damage) { }
    public float GetSpeedMultiplier() { return 1f; }
}


public class SpeedBoostState : IPlayerState
{
    private PlayerStateManager manager;
    private float duration;
    private float timer;
    private float speedMultiplier;

    public SpeedBoostState(float duration, float multiplier)
    {
        this.duration = duration;
        this.speedMultiplier = multiplier;
    }

    public void OnEnter(PlayerStateManager manager)
    {
        this.manager = manager;
        this.timer = 0f;
        Debug.Log("Player entered Speed Boost State");
    }

    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            manager.SetState(new NormalPlayerState());
        }
    }

    public void OnExit() { Debug.Log("Player exited Speed Boost State"); }
    public void HandleDamage(ref int damage) { /* Звичайна шкода */ }
    public float GetSpeedMultiplier() { return speedMultiplier; }
}


public class ShieldState : IPlayerState
{
    private PlayerStateManager manager;
    private float duration;
    private float timer;

    public ShieldState(float duration)
    {
        this.duration = duration;
    }
    public void OnEnter(PlayerStateManager manager)
    {
        this.manager = manager;
        this.timer = 0f;
        Debug.Log("Player entered Shield State");
    }
    public void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            manager.SetState(new NormalPlayerState());
        }
    }
    public void OnExit() { Debug.Log("Player exited Shield State"); }
    public void HandleDamage(ref int damage)
    {
        damage = 0;
        Debug.Log("Damage Blocked by Shield");
    }
    public float GetSpeedMultiplier() { return 1f; }
}