using UnityEngine;

namespace Enemy
{
    public interface IPrefabProvider
    {
        GameObject GetPrefab(int index);
    }

    public class PrefabProvider : MonoBehaviour, IPrefabProvider
    {
        [SerializeField] private GameObject[] enemyPrefabs;

        public GameObject GetPrefab(int index)
        {
            return enemyPrefabs[index];
        }
    }
}