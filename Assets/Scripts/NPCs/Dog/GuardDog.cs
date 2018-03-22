using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardDog : Guard
{
	public int attemptsToSearchNearbyLocations = 3;

	private DogFieldOfView fov;
	private Vector3 lastSearchLocation = Vector3.zero;

	protected override void Start()
	{
		base.Start();
		fov		= GetComponentInChildren<DogFieldOfView>();
		status	= GuardStatus.Patrolling;

		DogFieldOfView.OnDogHasSpottedPlayer += OnStartChasingPlayer;
		DogFieldOfView.OnDogIsLosingPlayer += OnStartLosingPlayer;
	}

	protected override void Update()
	{
		base.Update();

		if (goingBackToWaypoint)
		{
			if (navMeshAgent.remainingDistance <= 1)
			{
				animator.SetBool("running", false);
				status = GuardStatus.Patrolling;
				navMeshAgent.speed = originalSpeed;
				goingBackToWaypoint = false;
			}
		}

		if(status == GuardStatus.Distracted && lostInterestToDistractor > 0)
		{
			if(navMeshAgent.remainingDistance == 0)
			{
				lostInterestToDistractor -= Time.deltaTime;
				animator.SetFloat("speed", 0);
			}
				
		}

		if(lostInterestToDistractor <= 0 && status == GuardStatus.Distracted)
			ResumePatrolling();
	}

	IEnumerator GoToNextWaypoint()
	{
		status = GuardStatus.Idle;
		animator.SetFloat("speed", 0);
		float randomIdleTime = Random.Range(minMaxWaitBeforeMovingToNextWaypoint.x, minMaxWaitBeforeMovingToNextWaypoint.y);
		yield return new WaitForSecondsRealtime(randomIdleTime);
		
		currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.transform.childCount;
		navMeshAgent.SetDestination(waypoints.GetChild(currentWaypointIndex).transform.position);
		status = GuardStatus.Patrolling;
	}

	protected override void OnStartChasingPlayer()
	{
		base.OnStartChasingPlayer();
	}

	protected IEnumerator LookingForPlayer()
	{
		int attempts = 1;
		navMeshAgent.stoppingDistance = 0;

		while (attempts <= attemptsToSearchNearbyLocations)
		{
			isLookingForPlayer = true;
			yield return StartCoroutine(SearchRandomNearbyLocation());
			attempts++;
		}
		
		ResumePatrolling();
	}

	protected virtual void ResumePatrolling()
	{
		lostInterestToDistractor	= 0;
		isLookingForPlayer			= false;
		status						= GuardStatus.Patrolling;	
		goingBackToWaypoint			= true;
		navMeshAgent.GoToShortestWaypointLocation(waypoints, ref currentWaypointIndex);
	}

	protected bool IsNavmeshPositionValid(Vector3 target)
	{
		NavMeshHit hit;
		float distance = Vector3.Distance(target,transform.position);
		if (NavMesh.SamplePosition(target, out hit, distance, 1))
		{
			return true;
		}
			
		else return false;
	}

	protected IEnumerator SearchRandomNearbyLocation()
	{
		Vector3 randomLocation					= Vector3.zero;
		float distanceFromCurrentLocation		= 0;
		do
		{
			randomLocation					= lastKnownPlayerPosition + (Vector3)Random.insideUnitCircle * searchPlayerRadius;
			distanceFromCurrentLocation		= Vector3.Distance(transform.position, randomLocation);

			lastSearchLocation = randomLocation;
		}
		while (!IsNavmeshPositionValid(randomLocation) && distanceFromCurrentLocation < minimumSearchDistance);

		navMeshAgent.destination = randomLocation;

		while (isLookingForPlayer)
		{
			yield return null;
		}
	}

	protected override void OnStartLosingPlayer()
	{
		base.OnStartLosingPlayer();
		StartCoroutine(LookingForPlayer());
		navMeshAgent.GoToShortestWaypointLocation(waypoints, ref currentWaypointIndex);
		//navMeshAgent.destination	= navMeshAgent.getsho(transform, waypoints, ref currentWaypointIndex);
		goingBackToWaypoint			= true;
	}

	public override void DistractedTo(GameObject obj, float time)
	{
		base.DistractedTo(obj, time);

		if (!IsNavmeshPositionValid(obj.transform.position)) return;

		StopCoroutine(LookingForPlayer());
		StopCoroutine(GoToNextWaypoint());

		animator.SetFloat("speed", 1);
		navMeshAgent.SetDestination(obj.transform.position);
		lostInterestToDistractor = time;
	}
}
