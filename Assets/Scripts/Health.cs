using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public bool isPlayer = false;
	public int health;
	public static event System.Action OnPlayerTakeDamage;

	public int GetCurrentHealth()
	{ return health; }

	public void TakeDamage(int damage)
	{
		GameUIManager.Singleton.SubstractPlayerHealth(health, damage);
		health -= damage;
		//Debug.Log("take damage");
		if (health < 0) health = 0;
		if (OnPlayerTakeDamage != null && isPlayer)
			OnPlayerTakeDamage();

		
	}
}
