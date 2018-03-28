using Managers;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    private Button _button;
	public int Level;
	
	private void Start()
	{
		_button = GetComponent<Button>();
		_button.onClick.AddListener(GoToLevel);
	}

	void GoToLevel()
	{
		LevelManager.Singleton.CurrentLevel = Level;
		LevelManager.Singleton.GoToLevel(Level);
	}
}
