using System;
using GamePlay;
using TMPro;
using UnityEngine;

namespace Core
{
    /// <summary>
    /// Manages the player's score, updates the UI, and broadcasts score changes globally.
    /// </summary>
    [DisallowMultipleComponent]
    public class ScoreManager : MonoBehaviour
    {

        /// <summary>
        /// Triggered whenever the player's score increases. Passes the new total score.
        /// </summary>
        public static event Action<int> OnScoreChanged;

        [Header("UI References")]
        [Tooltip("The TextMeshPro UI element used to display the current score.")]
        [SerializeField]
        private TextMeshProUGUI scoreText;

    
        private int score;

        /// <summary>
        /// Gets the player's current score. Read-only from outside the class.
        /// </summary>
        public int CurrentScore => score;

        private void Awake()
        {
            
            this.UpdateUI();
        }

        /// <summary>
        /// Calculates the reward based on the merged cube's value and updates the total score.
        /// </summary>
        /// <param name="mergeValue">The value of the newly merged cube.</param>
        public void AddScore(int mergeValue)
        {
            // Calculate reward: e.g., merging into an 8 gives 2 points
            int reward = mergeValue / 4;
            this.score += reward;
            this.UpdateUI();

            OnScoreChanged?.Invoke(this.score);
        }

        /// <summary>
        /// Updates the text component to reflect the current score.
        /// </summary>
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

