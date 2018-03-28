using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Singleton;
    [HideInInspector]
    public RectTransform Healthbar;
    
    private float _playerCurrentHealth;
    private float _playerMissingHealth;
    private bool _isSubstractingPlayerHealth;
    private Image healthbarImage;
    
    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Healthbar = GameObject.Find("HealthBar").GetComponent<RectTransform>();
        healthbarImage	= Healthbar.GetComponent<Image>();
    }
    
    public void SubstractPlayerHealth(int current, int missing)
    {
        _playerCurrentHealth = current;
        _playerMissingHealth = current - missing;

        if (!_isSubstractingPlayerHealth)
            StartCoroutine(CrtSubstractPlayerHealth());
    }

    public void ResetPlayerHealth(int health)
    {
        StopCoroutine("CrtSubstractPlayerHealth");
        _isSubstractingPlayerHealth = false;
        _playerCurrentHealth 	= health;
        _playerMissingHealth 	= 100;
        Healthbar.sizeDelta		= new Vector2(_playerCurrentHealth*3, Healthbar.sizeDelta.y);
        healthbarImage.color 	= Color.green;
    }

    IEnumerator CrtSubstractPlayerHealth()
    {		
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
