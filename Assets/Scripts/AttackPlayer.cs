using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
	public int attackDamage		= 10;
	public float attackRadius	= .3f;
	public float attackAngle	= 50f;
	public LayerMask obstacleMask;

	protected Health playerHealth;
	protected GameObject player;
	protected float timeToAttack = 0;

	protected virtual void Start()
	{
		//playerHealth = GetC
		
		DogFieldOfView.OnDogIsLosingPlayer += OnStartLosingPlayer;
	}

	public virtual void Attack()
	{
		playerHealth.TakeDamage(attackDamage);
	}

	//public Vector3 DirectionFromAngle(float angle, bool angleIsGlobal)
	//{
	//	if (!angleIsGlobal)
	//		angle += transform.eulerAngles.y;
	//	return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
	//}

	protected bool CanAttackPlayer()
	{
		float distance = Vector3.Distance(transform.position, player.transform.position);
		if (distance < attackRadius)
		{
			if (distance < .5f) return true;

			Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;

			float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, dirToPlayer);

			if (angleBetweenGuardAndPlayer < attackAngle / 2f)
			{
				if (!Physics.Linecast(transform.position, player.transform.position, obstacleMask))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void OnStartLosingPlayer()
	{
		timeToAttack = 0;
	}
}
