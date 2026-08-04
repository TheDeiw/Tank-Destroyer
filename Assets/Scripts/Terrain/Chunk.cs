using UnityEngine;

namespace Terrain
{
    public class Chunk : MonoBehaviour
    {
        [Header("Grass Settings")]
        [SerializeField] private int grassCount = 1000;
        [SerializeField] private Mesh grassMesh;
        [SerializeField] private Material grassMaterial;

        private Matrix4x4[] grassMatrices;
        private float chunkSize;

        public void Initialize(Vector2Int logicalCoord, Vector3 worldPosition, float size)
        {
            transform.position = worldPosition;
            chunkSize = size;

            gameObject.SetActive(true);

            if (grassMatrices == null || grassMatrices.Length != grassCount)
            {
                grassMatrices = new Matrix4x4[grassCount];
            }

            GenerateGrass(logicalCoord);
        }

        private void GenerateGrass(Vector2Int coord)
        {
            int seed = coord.x * 73856 + coord.y * 19349;
            Random.InitState(seed);

            float halfSize = chunkSize / 2f;

            for (int i = 0; i < grassCount; i++)
            {
                float localX = Random.Range(-halfSize, halfSize);
                float localZ = Random.Range(-halfSize, halfSize);

                Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                Vector3 randomScale = Vector3.one * Random.Range(0.8f, 1.2f);

                Vector3 worldPos = transform.position + new Vector3(localX, 0.5f, localZ);

                grassMatrices[i] = Matrix4x4.TRS(worldPos, randomRotation, randomScale);
            }
        }

        void Update()
        {
            if (grassMesh == null || grassMaterial == null || grassMatrices == null) return;
            Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, grassMatrices, grassCount);
        }
    }
}