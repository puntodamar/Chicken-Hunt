using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuardDog))]
public class DogFieldOfView : FieldOfView
{
	public static event System.Action OnDogHasSpottedPlayer;
	public static event System.Action OnDogIsLosingPlayer;
	public float timeToSpotPlayer = .5f;

	[HideInInspector]

	private Transform player;
	private float playerVisibleTimer	= 0f;
	private GuardDog dog;
	private bool fovColorIsOverriden	= false;
	private IEnumerator activeCoroutine;

	protected override void Start()
	{
		base.Start();
		dog			= GetComponentInParent<GuardDog>();
		player		= GameObject.Find("Player").transform;
	}

	private void Update()
	{
		if (CanSeePlayer())
		{
			if (dog.status == GuardStatus.Idle || dog.status == GuardStatus.Patrolling || dog.status != GuardStatus.Patrolling)
				playerVisibleTimer += Time.deltaTime;
			if (dog.status == GuardStatus.Pursuing || dog.status == GuardStatus.Searching)
				playerVisibleTimer = timeToSpotPlayer;
		}
		else
			playerVisibleTimer -= Time.deltaTime;

			

		if (dog.status != GuardStatus.Searching)
		{
			playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

			if (!fovColorIsOverriden)
				renderer.material.color = Color.Lerp(defaultFOVColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
		}

		if (playerVisibleTimer >= timeToSpotPlayer && dog.status != GuardStatus.Pursuing)
		{
			if (OnDogHasSpottedPlayer != null)
			{
				OnDogHasSpottedPlayer();
			}
		}

		if (playerVisibleTimer == 0 && dog.status == GuardStatus.Pursuing)
		{
			if (OnDogIsLosingPlayer != null)
				OnDogIsLosingPlayer();
		}

		//if(playerVisibleTimer == time)
	}

	public void ChangeFOVColor(bool transitionToDefaultFOVColor, Color destination)
	{
		fovColorIsOverriden = !transitionToDefaultFOVColor;
		if (activeCoroutine != null) StopCoroutine(activeCoroutine);
		
		if(fovColorIsOverriden)
			StartCoroutine(ChangeFOVColorTo(destination));
	}

	public IEnumerator ChangeFOVColorTo(Color destination)
	{
		Color current = renderer.material.color;

		while (renderer.material.color != destination)
		{
			//renderer.material.color = Color.Lerp(defaultFOVColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
			renderer.material.color = Color.Lerp(current, destination, Time.time);
			yield return null;
		}
	}

	bool CanSeePlayer()
	{
		float distance = Vector3.Distance(transform.position, player.position);
		if (distance < FOVRadius)
		{
			if (distance < .5f) return true;

			Vector3 dirToPlayer					= (player.position - transform.position).normalized;
			float angleBetweenGuardAndPlayer	= Vector3.Angle(transform.forward, dirToPlayer);

			if (angleBetweenGuardAndPlayer < viewAngle / 2f)
			{
				if (!Physics.Linecast(transform.position, player.position, obstacleMask))
				{
					return true;
				}
			}
		}
		return false;
	}
}
