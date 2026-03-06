using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private HealthBar _healthBarReferenceForInitialSetup;

    private bool isGameOverHandled = false;
    private int maxHealthValue;      // -mainState
    private int currentHealthValue;  // -mainState

    public void InitializeHealth(int maxHealth, int startHealth)
    {
        maxHealthValue = maxHealth;
        currentHealthValue = Mathf.Clamp(startHealth, 0, maxHealthValue);
        isGameOverHandled = false;

        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.PlayerHealthChangedPublisher.NotifySubscribers(
                new PlayerHealthChangedEventArgs(currentHealthValue, maxHealthValue));
        }
    }

    public void ApplyDamage(int damage)
    {
        if (isGameOverHandled || currentHealthValue <= 0) return;

        currentHealthValue -= damage;
        currentHealthValue = Mathf.Clamp(currentHealthValue, 0, maxHealthValue);

        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.PlayerHealthChangedPublisher.NotifySubscribers(
                new PlayerHealthChangedEventArgs(currentHealthValue, maxHealthValue));
        }

        if (currentHealthValue <= 0)
        {
            TriggerPlayerDiedEvent();
        }
    }

    private void TriggerPlayerDiedEvent()
    {
        if (!isGameOverHandled)
        {
            isGameOverHandled = true;
            if (GameEventManager.Instance != null)
            {
                GameEventManager.Instance.PlayerDiedPublisher.NotifySubscribers(new PlayerDiedEventArgs());
            }
        }
    }

    public bool IsAlive() => currentHealthValue > 0 && !isGameOverHandled;

    public void SetCurrentHealth(int health) 
    {
        currentHealthValue = Mathf.Clamp(health, 0, maxHealthValue);
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.PlayerHealthChangedPublisher.NotifySubscribers(
                new PlayerHealthChangedEventArgs(currentHealthValue, maxHealthValue));
        }
        if (currentHealthValue <= 0 && !isGameOverHandled)
        {
            TriggerPlayerDiedEvent();
        }
    }
}