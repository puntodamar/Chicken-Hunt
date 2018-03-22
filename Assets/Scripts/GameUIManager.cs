using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
	public RectTransform healthbar;
	public static GameUIManager Singleton;

	private float playerCurrentHealth;
	private float playerMissingHealth;
	private bool isSubstractingPlayerHealth;

	private void Start()
	{
		if (Singleton == null)
			Singleton = this;
		else if (Singleton != this)
			Destroy(this);
	}

	public void SubstractPlayerHealth(int current, int missing)
	{
		playerCurrentHealth = current;
		playerMissingHealth = current - missing;

		if (!isSubstractingPlayerHealth)
			StartCoroutine(CrtSubstractPlayerHealth());
	}

	IEnumerator CrtSubstractPlayerHealth()
	{
		Image healthbarImage = healthbar.GetComponent<Image>();
		

		while(playerCurrentHealth > playerMissingHealth && playerCurrentHealth > 0)
		{
			playerCurrentHealth		= Mathf.Lerp(playerCurrentHealth, playerMissingHealth, .5f * Time.deltaTime);
			healthbar.sizeDelta		= new Vector2(playerCurrentHealth*3, healthbar.sizeDelta.y);
			float healthPercentage	= playerCurrentHealth / 100;
			healthbarImage.color	= Color.Lerp(Color.red, Color.green, healthPercentage);
			yield return null;
		}

		isSubstractingPlayerHealth = false;
	}
	
}
