using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuardDog))]
public class DogAttackPlayer : AttackPlayer
{
	public float attackCooldown = .5f;

	private GuardDog dog;

	protected override void Start()
	{
		player			= GameObject.FindGameObjectWithTag("Player");
		playerHealth	= player.GetComponent<Health>();
		dog				= GetComponent<GuardDog>();
	}

	private void Update()
	{
		if(dog.status == GuardStatus.Pursuing)
		{
			if (CanAttackPlayer())
			{
				timeToAttack += Time.deltaTime;

				if (timeToAttack >= attackCooldown)
				{
					playerHealth.TakeDamage(attackDamage);
					timeToAttack = 0;
				}
			}
			else Mathf.Clamp(timeToAttack - Time.deltaTime, 0, timeToAttack);

		}
	}

}
