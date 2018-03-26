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
			if(NavMeshAgent.remainingDistance <= .2f)
				Animator.SetFloat("speed", 0);

			if (NavMeshAgent.remainingDistance <= 0)
				StartCoroutine(GoToNextWaypoint());

			else Animator.SetFloat("speed", 1);
		}

		if(status == GuardStatus.Pursuing)
		{
			TimeToUpdateNavmeshDestination += Time.deltaTime;
			Animator.SetFloat("speed", 1);
			if (TimeToUpdateNavmeshDestination >= NavmeshUpdateRate && NavMeshAgent.destination != Player.transform.position)
			{
				lastKnownPlayerPosition = Player.transform.position;
				NavMeshAgent.destination = lastKnownPlayerPosition;
				TimeToUpdateNavmeshDestination = 0;
			}
		}
	}

	IEnumerator GoToNextWaypoint()
	{
		status = GuardStatus.Idle;
		
		float randomIdleTime = Random.Range(MinMaxWaitBeforeMovingToNextWaypoint.x, MinMaxWaitBeforeMovingToNextWaypoint.y);
		yield return new WaitForSecondsRealtime(randomIdleTime);

		CurrentWaypointIndex	= (CurrentWaypointIndex + 1) % Waypoints.transform.childCount;
		status					= GuardStatus.Patrolling;
		NavMeshAgent.SetDestination(Waypoints.GetChild(CurrentWaypointIndex).transform.position);
		
	}

	protected virtual void OnStartChasingPlayer()
	{
		Animator.SetBool("running", true);
		status = GuardStatus.Pursuing;
		NavMeshAgent.speed = runSpeed;
		NavMeshAgent.SetDestination(Player.transform.position);
	}

	protected virtual void OnStartLosingPlayer()
	{
		status = GuardStatus.Searching;
		NavMeshAgent.stoppingDistance = 0;
	}

	public override void DistractedTo(GameObject obj, float time)
	{
		status = GuardStatus.Distracted;
	}
}
