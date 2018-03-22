using System.Collections;
using UnityEngine;

public enum GuardStatus { Patrolling, Pursuing, Idle, Searching, Distracted }

public class Guard: NPC
{
	public GuardStatus status;
	public float runSpeed = 10f;
	public float searchPlayerRadius				= 5f;
	public float minimumSearchDistance			= 2f;

	[HideInInspector]
	public bool canAttackPlayer					= false;
	public bool canBeDistracted					= false;

	protected Vector3 lastKnownPlayerPosition;
	protected bool goingBackToWaypoint			= false;
	protected bool isLookingForPlayer			= false;
	public float lostInterestToDistractor		= 0f;

	protected override void Start()
	{
		base.Start();
		status = GuardStatus.Patrolling;
	}

	protected virtual void Update()
	{
		if (status == GuardStatus.Patrolling)
		{
			if(navMeshAgent.remainingDistance <= .2f)
				animator.SetFloat("speed", 0);

			if (navMeshAgent.remainingDistance <= 0)
				StartCoroutine(GoToNextWaypoint());

			else animator.SetFloat("speed", 1);
		}

		if(status == GuardStatus.Pursuing)
		{
			timeToUpdateNavmeshDestination += Time.deltaTime;
			animator.SetFloat("speed", 1);
			if (timeToUpdateNavmeshDestination >= navmeshUpdateRate && navMeshAgent.destination != player.transform.position)
			{
				lastKnownPlayerPosition = player.transform.position;
				navMeshAgent.destination = lastKnownPlayerPosition;
				timeToUpdateNavmeshDestination = 0;
			}
		}
	}

	IEnumerator GoToNextWaypoint()
	{
		status = GuardStatus.Idle;
		
		float randomIdleTime = Random.Range(minMaxWaitBeforeMovingToNextWaypoint.x, minMaxWaitBeforeMovingToNextWaypoint.y);
		yield return new WaitForSecondsRealtime(randomIdleTime);

		currentWaypointIndex	= (currentWaypointIndex + 1) % waypoints.transform.childCount;
		status					= GuardStatus.Patrolling;
		navMeshAgent.SetDestination(waypoints.GetChild(currentWaypointIndex).transform.position);
		
	}

	protected virtual void OnStartChasingPlayer()
	{
		animator.SetBool("running", true);
		status = GuardStatus.Pursuing;
		navMeshAgent.speed = runSpeed;
		navMeshAgent.SetDestination(player.transform.position);
	}

	protected virtual void OnStartLosingPlayer()
	{
		status = GuardStatus.Searching;
		navMeshAgent.stoppingDistance = 0;
	}

	public override void DistractedTo(GameObject obj, float time)
	{
		status = GuardStatus.Distracted;
	}
}
