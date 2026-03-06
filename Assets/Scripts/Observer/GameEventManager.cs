using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    public GameEventPublisher<EnemyDiedEventArgs> EnemyDiedPublisher { get; private set; }
    public GameEventPublisher<PlayerHealthChangedEventArgs> PlayerHealthChangedPublisher { get; private set; }
    public GameEventPublisher<PlayerDiedEventArgs> PlayerDiedPublisher { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 

            EnemyDiedPublisher = new GameEventPublisher<EnemyDiedEventArgs>();
            PlayerHealthChangedPublisher = new GameEventPublisher<PlayerHealthChangedEventArgs>();
            PlayerDiedPublisher = new GameEventPublisher<PlayerDiedEventArgs>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}