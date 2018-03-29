using System.Collections;
using System.Collections.Generic;
using Managers;
using NPCs.Chicken;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Singleton;
    [HideInInspector]
    public RectTransform Healthbar;

    private Image _eatSkillImage;
    
    
    private float _playerCurrentHealth;
    private float _playerMissingHealth;
    private bool _isSubstractingPlayerHealth;
    private Image _healthbarImage;
    private GameObject _scorePanel;
    
    private void Awake()
    {
        Singleton = this;
        GameManager.OnGameOver += OnGameOver;
    }

    private void Start()
    {
        Healthbar       = GameObject.Find("HealthBar").GetComponent<RectTransform>();
        _healthbarImage = Healthbar.GetComponent<Image>();
        _eatSkillImage  = GameObject.Find("EatChickenIcon").GetComponent<Image>();
        _scorePanel     = GameObject.Find("ScorePanel");
        _scorePanel.SetActive(false);
        
        EatSkill.ChickenIsInRange     += ToggleEatSkillUI;
        Chicken.OnChickenDead         += ToggleEatSkillUI;
        Chicken.OnChickenIsNotInRange += ToggleEatSkillUI;
        ToggleEatSkillUI();
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
        _playerCurrentHealth        = health;
        _playerMissingHealth        = 100;
        Healthbar.sizeDelta         = new Vector2(_playerCurrentHealth * 3, Healthbar.sizeDelta.y);
        _healthbarImage.color       = Color.green;
    }

    IEnumerator CrtSubstractPlayerHealth()
    {
        while (_playerCurrentHealth > _playerMissingHealth && _playerCurrentHealth > 0)
        {
            _playerCurrentHealth   = Mathf.Lerp(_playerCurrentHealth, _playerMissingHealth, .5f * Time.deltaTime);
            Healthbar.sizeDelta    = new Vector2(_playerCurrentHealth * 3, Healthbar.sizeDelta.y);
            float healthPercentage = _playerCurrentHealth / 100;
            _healthbarImage.color  = Color.Lerp(Color.red, Color.green, healthPercentage);
            yield return null;
        }

        _isSubstractingPlayerHealth = false;
    }

    public void ToggleEatSkillUI()
    {
        _eatSkillImage.color = _eatSkillImage.color == Color.gray ? Color.white : Color.gray;
    }

    void OnGameOver()
    {
        
        float skor = ScoreManager.Singleton.CalculateScore();
        _scorePanel.SetActive(true);
        _scorePanel.transform.Find("Skor").GetComponent<Text>().text = "Skor" + skor;
        
        
    }
}
