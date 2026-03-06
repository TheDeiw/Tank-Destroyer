using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject player;
    //public GameObject[] enemyPrefabs;
    [SerializeField] private PrefabProvider prefabProvider;
    private EnemyFactory[] enemyFactories;
    public float minSpawnRadius;
    public float maxSpawnRadius;

    private float minSpawnInterval = 1f;
    private float maxSpawnInterval = 3f;
    private float minIntervalLimit = 0.5f;
    private float intervalDecreaseRate = 0.9f;
    private float timeSinceLastDecrease = 0f;
    private float decreaseInterval = 5f;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instantiate(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        enemyFactories = new EnemyFactory[]
        {
            new EasyEnemyFactory(prefabProvider),
            new HardEnemyFactory(prefabProvider)
        };
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
            Debug.Log($"New spawn intervals: min={minSpawnInterval}, max={maxSpawnInterval}");
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
            Debug.LogError("No enemy factories initialized!");
            return;
        }


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
                Debug.Log($"Enemy spawned at {spawnPosition} using {enemyFactories[factoryIndex].GetType().Name}");
                break;
            }
        }
    }
}