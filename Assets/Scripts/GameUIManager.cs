using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
	public RectTransform Healthbar;
	public static GameUIManager Singleton;

	private float _playerCurrentHealth;
	private float _playerMissingHealth;
	private bool _isSubstractingPlayerHealth;

	private void Start()
	{
		if (Singleton == null)
			Singleton = this;
		else if (Singleton != this)
			Destroy(this);
	}

	public void SubstractPlayerHealth(int current, int missing)
	{
		_playerCurrentHealth = current;
		_playerMissingHealth = current - missing;

		if (!_isSubstractingPlayerHealth)
			StartCoroutine(CrtSubstractPlayerHealth());
	}

	IEnumerator CrtSubstractPlayerHealth()
	{
		Image healthbarImage = Healthbar.GetComponent<Image>();
		

		while(_playerCurrentHealth > _playerMissingHealth && _playerCurrentHealth > 0)
		{
			_playerCurrentHealth		= Mathf.Lerp(_playerCurrentHealth, _playerMissingHealth, .5f * Time.deltaTime);
			Healthbar.sizeDelta		= new Vector2(_playerCurrentHealth*3, Healthbar.sizeDelta.y);
			float healthPercentage	= _playerCurrentHealth / 100;
			healthbarImage.color	= Color.Lerp(Color.red, Color.green, healthPercentage);
			yield return null;
		}

		_isSubstractingPlayerHealth = false;
	}
	
}
