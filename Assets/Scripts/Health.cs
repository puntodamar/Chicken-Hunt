using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class Health : MonoBehaviour
{
	public bool IsPlayer = false;
	public int CurrentHealth;
	public static event System.Action OnPlayerTakeDamage;

	public int GetCurrentHealth()
	{ return CurrentHealth; }

	public void TakeDamage(int damage)
	{
		GameManager.Singleton.SubstractPlayerHealth(CurrentHealth, damage);
		CurrentHealth -= damage;
		//Debug.Log("take damage");
		if (CurrentHealth < 0) CurrentHealth = 0;
		if (OnPlayerTakeDamage != null && IsPlayer)
			OnPlayerTakeDamage();

		
	}
}
