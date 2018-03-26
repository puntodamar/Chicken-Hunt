using System.Collections;
using System.Collections.Generic;
using NPCs.Dog;
using UnityEngine;

[RequireComponent(typeof(GuardDog))]
public class DogAttackPlayer : AttackPlayer
{
	public float AttackCooldown = .5f;

	private GuardDog _dog;

	protected override void Start()
	{
		Player			= GameObject.FindGameObjectWithTag("Player");
		PlayerHealth	= Player.GetComponent<Health>();
		_dog			= GetComponent<GuardDog>();
	}

	private void Update()
	{
		if(_dog.status == GuardStatus.Pursuing)
		{
			if (CanAttackPlayer())
			{
				TimeToAttack += Time.deltaTime;

				if (TimeToAttack >= AttackCooldown)
				{
					PlayerHealth.TakeDamage(AttackDamage);
					TimeToAttack = 0;
				}
			}
			else Mathf.Clamp(TimeToAttack - Time.deltaTime, 0, TimeToAttack);

		}
	}

}
