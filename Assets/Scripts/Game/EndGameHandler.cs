using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private PlayerDeathHandler playerDeathHandler;
    [SerializeField] private GameObject losePanel;

    void Start()
    {
        Time.timeScale = 1f;
        losePanel.SetActive(false);
        playerDeathHandler.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        if (playerDeathHandler != null)
            playerDeathHandler.OnPlayerDied -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f;
    }
}