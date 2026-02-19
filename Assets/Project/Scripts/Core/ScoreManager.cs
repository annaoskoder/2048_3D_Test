using GamePlay;
using TMPro;
using UnityEngine;

namespace Core
{
    public class ScoreManager : MonoBehaviour
    {
       

        [SerializeField]
        private TextMeshProUGUI scoreText;

    
        private int score;

        public int CurrentScore => score;

        private void Awake()
        {
            
            this.UpdateUI();
        }

        public void AddScore(int mergeValue)
        {
            int reward = mergeValue / 4;
            this.score += reward;
            this.UpdateUI();

            GameManager.Instance.CheckWinCondition(this.score);
        }

        private void UpdateUI()
        {
            this.scoreText.text = "Scores: " + this.score;
        }

        private void OnEnable()
        {
            Cube.OnMerged += this.AddScore;
        }

        private void OnDisable()
        {
            Cube.OnMerged -= this.AddScore;
        }

       

    }

}

