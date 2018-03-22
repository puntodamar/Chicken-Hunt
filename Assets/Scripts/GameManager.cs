using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private void Start()
	{
		//Health.OnPlayerTakeDamage += ResetGame;
	}

	private void ResetGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
