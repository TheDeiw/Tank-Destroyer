using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Pool;

namespace Terrain
{
    public class ChunkGeneration : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Chunk chunkPrefab;
        [SerializeField] private float chunkSize;
        [SerializeField] private int viewRadius;

        private Dictionary<Vector2Int, Chunk> activeChunks = new Dictionary<Vector2Int, Chunk>();

        private Vector2Int lastPlayerChunk;

        private ObjectPool<Chunk> chunkPool;

        void Start()
        {
            chunkPool = new ObjectPool<Chunk>(
                createFunc: CreateChunk,
                actionOnGet: OnTakeChunkFromPool,
                actionOnRelease: OnReturnChunkToPool,
                actionOnDestroy: OnDestroyChunk,
                collectionCheck: false,
                defaultCapacity: 25,
                maxSize: 50
            );

            lastPlayerChunk = GetChunkCoordFromPosition(player.position);
            UpdateVisibleChunks();
        }

        void Update()
        {
            Vector2Int currentPlayerChunk = GetChunkCoordFromPosition(player.position);

            if (currentPlayerChunk != lastPlayerChunk)
            {
                lastPlayerChunk = currentPlayerChunk;
                UpdateVisibleChunks();
            }
        }

        // Pull methods
        private Chunk CreateChunk()
        {
            Chunk newChunk = Instantiate(chunkPrefab);
            newChunk.transform.SetParent(this.transform);
            return newChunk;
        }
        private void OnTakeChunkFromPool(Chunk chunk)
        {
            chunk.gameObject.SetActive(true);
        }

        private void OnReturnChunkToPool(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
        }

        private void OnDestroyChunk(Chunk chunk)
        {
            Destroy(chunk.gameObject);
        }


        // Other methods
        private Vector2Int GetChunkCoordFromPosition(Vector3 position)
        {
            int x = Mathf.RoundToInt(position.x / chunkSize);
            int z = Mathf.RoundToInt(position.z / chunkSize);
            return new Vector2Int(x, z);
        }

        private void UpdateVisibleChunks()
        {
            HashSet<Vector2Int> desiredChunks = new HashSet<Vector2Int>();

            for (int x = -viewRadius; x <= viewRadius; x++)
            {
                for (int z = -viewRadius; z <= viewRadius; z++)
                {
                    desiredChunks.Add(new Vector2Int(lastPlayerChunk.x + x, lastPlayerChunk.y + z));
                }
            }

            List<Vector2Int> currentKeys = activeChunks.Keys.ToList();
            foreach (var chunkCoord in currentKeys)
            {
                if (!desiredChunks.Contains(chunkCoord))
                {
                    Chunk chunkToRemove = activeChunks[chunkCoord];

                    chunkPool.Release(chunkToRemove);

                    activeChunks.Remove(chunkCoord);
                }
            }

            foreach (var chunkCoord in desiredChunks)
            {
                if (!activeChunks.ContainsKey(chunkCoord))
                {
                    Chunk newChunk = chunkPool.Get();

                    Vector3 worldPosition = new Vector3(chunkCoord.x * chunkSize, 0, chunkCoord.y * chunkSize);
                    newChunk.Initialize(chunkCoord, worldPosition);

                    activeChunks.Add(chunkCoord, newChunk);
                }
            }
        }
    }
}