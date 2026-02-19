using UnityEngine;

namespace GamePlay
{
    public class CubeSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Cube cubePrefab;
        [SerializeField] private Transform spawnPoint;

        [Header("Spawn Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceForTwo = 0.75f;

        [SerializeField] private float spawnDelay = 0.3f;
        public Cube ActiveCube { get; private set; }

        private void Start()
        {
           this.SpawnCube();
        }

       
        public void SpawnCube()
        {
            if (ActiveCube != null)
            {
                Debug.LogWarning("Trying to spawn while active cube exists.");
                return;
            }


            Cube newCube = Instantiate(cubePrefab, spawnPoint.position, Quaternion.identity);

            int value = GetRandomValue();
            newCube.SetValue(value);

            ActiveCube = newCube;
        }

        private int GetRandomValue()
        {
            float random = Random.value;

            if (random <= chanceForTwo)
                return 2;

            return 4;
        }

        public void ClearActiveCube()
        {
            ActiveCube = null;

            Invoke(nameof(SpawnCube), this.spawnDelay);
        }
    }
}
