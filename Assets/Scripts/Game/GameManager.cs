using System.Collections;
using Enemy;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject player;
    [SerializeField] private PrefabProvider prefabProvider;
    private IEnemyFactory[] enemyFactories;
    [SerializeField] private float minSpawnRadius;
    [SerializeField] private float maxSpawnRadius;

    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 3f;
    [SerializeField] private float minIntervalLimit = 0.5f;
    [SerializeField] private float intervalDecreaseRate = 0.9f;
    [SerializeField] private float decreaseInterval = 5f;
    private float timeSinceLastDecrease = 0f;


    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    public int KillsCount { get; private set; } = 0;
    public int HighScore { get; private set; } = 0;

    private const string HighScoreKey = "HighScoreKills";


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        enemyFactories = new IEnemyFactory[]
        {
            new EasyEnemyFactory(prefabProvider),
            new HardEnemyFactory(prefabProvider)
        };

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        PlayerPrefs.SetInt(HighScoreKey, HighScore);
        PlayerPrefs.Save();
    }

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    void Update()
    {
        timeSinceLastDecrease += Time.deltaTime;
        if (timeSinceLastDecrease >= decreaseInterval)
        {
            minSpawnInterval = Mathf.Max(minIntervalLimit, minSpawnInterval * intervalDecreaseRate);
            maxSpawnInterval = Mathf.Max(minIntervalLimit, maxSpawnInterval * intervalDecreaseRate);
            timeSinceLastDecrease = 0f;
            //Debug.Log($"New spawn intervals: min={minSpawnInterval}, max={maxSpawnInterval}");
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SpawnEnemy()
    {
        if (enemyFactories == null || enemyFactories.Length == 0)
        {
            //Debug.LogError("No enemy factories initialized!");
            return;
        }

        //Debug.Log("Attempting to spawn enemy...");
        int attempts = 10;
        for (int i = 0; i < attempts; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
            Vector3 spawnPosition = player.transform.position + offset;

            if (!Physics.CheckSphere(spawnPosition, 1f))
            {
                int factoryIndex = Random.Range(0, enemyFactories.Length);
                GameObject enemy = enemyFactories[factoryIndex].CreateEnemy();
                enemy.transform.position = spawnPosition;
                enemy.transform.rotation = Quaternion.identity;
                //Instantiate(enemy, spawnPosition, Quaternion.identity);
                //Debug.Log($"Enemy spawned at {spawnPosition} using {enemyFactories[factoryIndex].GetType().Name}");
                break;
            }
        }
    }
    public void AddKill()
    {
        KillsCount++;
        scoreText.text = $"Score: {KillsCount}";
        if (KillsCount > HighScore)
        {
            HighScore = KillsCount;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
        highScoreText.text = $"High Score: {HighScore}";
    }
}