using System.Collections;
using Managers;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public enum GuardStatus { Patrolling, Pursuing, Idle, Searching, Distracted }

public class Guard: NPC
{
	public static System.Action OnPlayerDetected;
	public static bool IsChasingPlayer = false;
	
	public GuardStatus Status;
	public float RunSpeed = 6;
	public float WalkSpeed = 3.5f;
	public float SearchPlayerRadius				= 5f;
	public float MinimumSearchDistance			= 2f;

	[HideInInspector]
	public bool CanAttackPlayer					= false;
	public bool CanBeDistracted					= false;

	protected Vector3 LastKnownPlayerPosition;
	protected bool GoingBackToWaypoint			= false;
	protected bool IsLookingForPlayer			= false;
	public float LostInterestToDistractor		= 0f;
	
	
	protected override void Start()
	{
		base.Start();
		Status = GuardStatus.Patrolling;
		GameManager.OnGameOver += Disable;
	}

	protected virtual void Update()
	{

	}

	protected virtual IEnumerator GoToNextWaypoint()
	{
		if (LostInterestToDistractor > 0) StopCoroutine(GoToNextWaypoint());
		Status = GuardStatus.Idle;
		Debug.Log("from parent");
		float randomIdleTime = Random.Range(MinMaxWaitBeforeMovingToNextWaypoint.x, MinMaxWaitBeforeMovingToNextWaypoint.y);
		yield return new WaitForSecondsRealtime(randomIdleTime);

		CurrentWaypointIndex	= (CurrentWaypointIndex + 1) % Waypoints.transform.childCount;
		Status					= GuardStatus.Patrolling;
		NavMeshAgent.SetDestination(Waypoints.GetChild(CurrentWaypointIndex).transform.position);
		
	}

	protected virtual void OnStartChasingPlayer()
	{
		Animator.SetBool("running", true);
		Status = GuardStatus.Pursuing;
		NavMeshAgent.speed = RunSpeed;
		NavMeshAgent.SetDestination(Player.transform.position);

		if (OnPlayerDetected != null)
			OnPlayerDetected();
	}

	protected virtual void OnStartLosingPlayer()
	{
		IsChasingPlayer = false;
		Status = GuardStatus.Searching;
		NavMeshAgent.stoppingDistance = 0;
	}

	public override void DistractedTo(GameObject obj, float time)
	{
		Status = GuardStatus.Distracted;
		StopCoroutine(GoToNextWaypoint());
	}

	void Disable()
	{
		this.enabled = false;
	}
}
