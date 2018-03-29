using Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
	public class MainMenuManager : MonoBehaviour
	{
		public int TotalLevel;
		public GameObject SelectLevelPanel;
		public Transform LevelPrefab;

		private void Start()
		{
			for (int i = 0; i < TotalLevel; i++)
			{
				Transform levelTransform = Instantiate(LevelPrefab, SelectLevelPanel.transform);
				Transform ratingTransform = levelTransform.GetChild(0);
				Transform buttonTransform = levelTransform.GetChild(1);
				buttonTransform.gameObject.GetComponent<LevelButton>().Level = i + 1;

				CheckProgress(ratingTransform, i);

				levelTransform.GetChild(1).GetComponentInChildren<Text>().text = (i + 1).ToString();
			}
			

		}
		void CheckProgress(Transform ratingTransform, int level)
		{
			if (PlayerPrefs.HasKey("Level" + level))
			{
				int ratingScore = PlayerPrefs.GetInt("Level" + level + "score");
				ratingTransform.GetComponent<Slider>().value = ratingScore;
			}
			else ratingTransform.gameObject.SetActive(false);
			
		}

		public void TogglePanel()
		{
			SelectLevelPanel.transform.parent.gameObject.SetActive(!SelectLevelPanel.transform.parent.gameObject.activeInHierarchy);
		}
	}
	

}
