using UnityEngine;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Singleton;
        private float _score      = 3f;
        private int _detectedCount = 0;
        private int _deadCount     = 0;
    
        public float Score
        {
            get { return _score; }
        }
    
        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            Health.OnPlayerDied += OnPlayerDead;
            Guard.OnPlayerDetected += OnPlayerDetected;
        }

        void OnPlayerDead()
        {
            _deadCount++;
        }

        void OnPlayerDetected()
        {
            _detectedCount++;
        }

        public float CalculateScore()
        {
            _score -= (.3f) * _detectedCount;
            _score -= 1 * _deadCount;
            
            float totalScore = (_score < 1) ? 1 : _score;
            int currentLevel = LevelManager.Singleton.CurrentLevel;
            PlayerPrefs.SetFloat("Level"+currentLevel+"score", totalScore);
            PlayerPrefs.SetInt("Level" +currentLevel,1);
            
            return totalScore;
        }
    
    
    }
}
