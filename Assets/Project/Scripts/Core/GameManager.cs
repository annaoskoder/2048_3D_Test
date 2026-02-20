using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// <summary>
    /// Controls the core game flow, including win conditions, restarting, and quitting the game.
    /// </summary>
 [DisallowMultipleComponent]
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [Tooltip("The score required to trigger the win condition.")]
        [SerializeField]
        private int winScore = 200;

        [Header("UI References")]
        [Tooltip("The UI panel displayed when the player wins the game.")]
        [SerializeField]
        private GameObject winPanel;

        private bool isGameOver;

        private void OnEnable()
        {
            // Subscribe to the global score event
            ScoreManager.OnScoreChanged += CheckWinCondition;
        }

        private void OnDisable()
        {
            // Unsubscribe to prevent memory leaks when the object is destroyed
            ScoreManager.OnScoreChanged -= CheckWinCondition;
        }

        /// <summary>
        /// Evaluates if the current score meets or exceeds the winning threshold.
        /// </summary>
        /// <param name="currentScore">The player's current total score.</param>
        public void CheckWinCondition(int currentScore)
        {
            if (this.isGameOver) return;

            if (currentScore >= this.winScore)
            {
                this.WinGame();
            }
        }

        /// <summary>
        /// Handles the game over state, pausing the game and displaying the win screen.
        /// </summary>
        private void WinGame()
        {
            this.isGameOver = true;

            // Pauses the game physics and time-dependent logic
            Time.timeScale = 0f;

            if (this.winPanel != null)
            {
                this.winPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[GameManager] Win Panel reference is missing!");
            }
        }

        /// <summary>
        /// Reloads the current scene and resets the time scale to normal.
        /// </summary>
        public void RestartGame()
        {
            // It is crucial to reset time scale before reloading, otherwise the new scene will be frozen
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Quits the application. Works in built games and stops play mode in the Unity Editor.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            // This line stops Play Mode when you test the Quit button inside the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

#if UNITY_EDITOR
        private void Reset()
        {
            // Ensures only one instance of the GameManager exists in the scene during editing
            var instances = FindObjectsByType<GameManager>(FindObjectsSortMode.None);
            if (instances.Length > 1)
            {
                Debug.LogError($"[Architecture Error] A GameManager already exists in the scene! Found {instances.Length} instances.");
            }
        }
#endif
    }
}