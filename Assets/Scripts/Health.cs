using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class Health : MonoBehaviour
{
	public bool IsPlayer = false;
	public int MaximumHealth = 100;
	public int CurrentHealth;
	public static event System.Action OnPlayerTakeDamage;
	public static event System.Action OnPlayerDied;

	private void Start()
	{
		CurrentHealth = MaximumHealth;
	}

	public int GetCurrentHealth()
	{ return CurrentHealth; }

	public void TakeDamage(int damage)
	{
		if (PlayerManager.Singleton.IsRespawning) return;
		
		GameUIManager.Singleton.SubstractPlayerHealth(CurrentHealth, damage);
		CurrentHealth -= damage;

		if (CurrentHealth <= 0)
		{
			Dead();
		}
		if (OnPlayerTakeDamage != null && IsPlayer)
			OnPlayerTakeDamage();

		
	}

	void Dead()
	{
		CurrentHealth = 0;
		if (OnPlayerDied != null && IsPlayer)
			OnPlayerDied();
		CurrentHealth = MaximumHealth;
		GameUIManager.Singleton.ResetPlayerHealth(MaximumHealth);
	}
}
