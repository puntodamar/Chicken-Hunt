using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Buttons
{
    public class ReplayButton : MonoBehaviour
    {
        private Button _button;
    
        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Replay);
            Debug.Log("start");
            _button.onClick.AddListener(() =>
            {
                Debug.Log("from delegate");
                Replay();
            });
        }

        void Replay()
        {
            Debug.Log("replya");
            Scene scene = SceneManager.GetActiveScene();
            LevelManager.Singleton.GoToLevel(scene.buildIndex);
        }
    }
}
