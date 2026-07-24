using UnityEngine;

namespace Terrain
{
    public class Chunk : MonoBehaviour
    {
        public void Initialize(Vector2Int logicalCoord, Vector3 worldPosition)
        {
            transform.position = worldPosition;
            gameObject.SetActive(true);
            gameObject.name = $"Chunk_{logicalCoord.x}_{logicalCoord.y}";
        }
    }
}