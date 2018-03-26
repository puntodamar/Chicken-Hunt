using System.Collections;
using System.Collections.Generic;
using NPCs.Dog;
using UnityEngine;
// ReSharper disable All

public class AttackPlayer : MonoBehaviour
{
	public int AttackDamage		= 10;
	public float AttackRadius	= .3f;
	public float AttackAngle	= 50f;
	public LayerMask ObstacleMask;

	protected Health PlayerHealth;
	protected GameObject Player;
	protected float TimeToAttack = 0;

	protected virtual void Start()
	{
		//playerHealth = GetC
		
		DogFieldOfView.OnDogIsLosingPlayer += OnStartLosingPlayer;
	}

	public virtual void Attack()
	{
		PlayerHealth.TakeDamage(AttackDamage);
	}

	//public Vector3 DirectionFromAngle(float angle, bool angleIsGlobal)
	//{
	//	if (!angleIsGlobal)
	//		angle += transform.eulerAngles.y;
	//	return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
	//}

	protected bool CanAttackPlayer()
	{
		float distance = Vector3.Distance(transform.position, Player.transform.position);
		if (!(distance < AttackRadius)) return false;
		if (distance < .5f) return true;

		Vector3 dirToPlayer 				= (Player.transform.position - transform.position).normalized;
		float angleBetweenGuardAndPlayer 	= Vector3.Angle(transform.forward, dirToPlayer);

		if (!(angleBetweenGuardAndPlayer < AttackAngle / 2f)) return false;
		
		return !Physics.Linecast(transform.position, Player.transform.position, ObstacleMask);
	}

	private void OnStartLosingPlayer()
	{
		TimeToAttack = 0;
	}
}
