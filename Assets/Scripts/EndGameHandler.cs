using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject losePanel;

    void Start()
    {
        Time.timeScale = 1f;
        losePanel.SetActive(false);
        playerHealth.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
}