using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    private IPlayerState currentState;
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        SetState(new NormalPlayerState());
    }

    public void SetState(IPlayerState newState)
    {
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
        ApplyStateEffects();
    }

    void Update()
    {
        currentState?.OnUpdate();
    }

    private void ApplyStateEffects()
    {
        if (playerMovement != null)
        {
            // PlayerMovement повинен враховувати цей множник
            // Наприклад, PlayerMovement.actualSpeed = PlayerMovement.baseSpeed * currentState.GetSpeedMultiplier();
        }
    }

    public float GetCurrentSpeedMultiplier()
    {
        return currentState?.GetSpeedMultiplier() ?? 1f;
    }
}