using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Buttons
{
    public class NextLevelButton : MonoBehaviour
    {
        private Button _button;
        private int    _currentLevel;
        private void Start()
        {
            _button       = GetComponent<Button>();
            _currentLevel = LevelManager.Singleton.CurrentLevel;
        
            if(LevelManager.Singleton.TotalLevel < _currentLevel)
                this.gameObject.SetActive(false);
            else
                _button.onClick.AddListener(GoToNextLevel);                  
        }

        void GoToNextLevel()
        {
            _currentLevel++;
            SceneManager.LoadScene("Level"+_currentLevel);
        }
    }
}
