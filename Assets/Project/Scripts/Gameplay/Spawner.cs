using UnityEngine;

namespace GamePlay
{


    [DisallowMultipleComponent]
    public class Spawner : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The cube prefab that will be spawned in the scene.")]
        [SerializeField] private Cube cubePrefab;

        [Tooltip("The transform point where the new cube will appear.")]
        [SerializeField] private Transform spawnPoint;

        [Space(10)]
        [Header("Spawn Settings")]

        [Tooltip("Probability of spawning a 2 (0.75 = 75%). Otherwise, a 4 is spawned.")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceForTwo = 0.75f;

        [Tooltip("Delay in seconds before a new cube spawns after clearing the active one.")]
        [Range(0f, 5f)]
        [SerializeField] private float spawnDelay = 0.3f;
        public Cube ActiveCube { get; private set; }

        private void Start()
        {
           this.SpawnCube();
        }


        /// <summary>
        /// Spawns a new cube if there is no active cube in the scene.
        /// </summary>
        [ContextMenu("Force Spawn Cube")]
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


        /// <summary>
        /// Generates a value for the cube (2 or 4) based on the specified probability.
        /// </summary>
        /// <returns>Integer value of either 2 or 4.</returns>
        private int GetRandomValue()
        {
            float random = Random.value;

            if (random <= chanceForTwo)
                return 2;

            return 4;
        }

        /// <summary>
        /// Clears the reference to the active cube and invokes the spawn of a new one with a delay.
        /// </summary>
        public void ClearActiveCube()
        {
            ActiveCube = null;

            Invoke(nameof(SpawnCube), this.spawnDelay);
        }
    }
}
