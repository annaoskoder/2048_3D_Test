using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField]
        private int winScore = 200;

        [SerializeField]
        private GameObject winPanel;

        private bool isGameOver;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void CheckWinCondition(int currentScore)
        {
            if (this.isGameOver) return;

            if (currentScore >= this.winScore)
            {
                this.WinGame();
            }
        }

        private void WinGame()
        {
            this.isGameOver = true;

            Time.timeScale = 0f;
            this.winPanel.SetActive(true);
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

}
