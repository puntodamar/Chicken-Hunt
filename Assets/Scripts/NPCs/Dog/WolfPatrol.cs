using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class WolfPatrol : MonoBehaviour
{
	public float moveSpeed			= 5;
	public float smoothMoveTime		= .1f;
	public float turnSpeed			= 90;
	public float timeToSpotPlayer	= .5f;
	public float viewDistance;
	public float timeBeforePatrollingAgain = 1f;
	public Vector2 minMaxWaitTime;	
	public LayerMask viewMask;
	public Transform pathHolder;
	public GuardStatus status;
	

	private Transform player;
	private Color originalSpotlightColour;
	private float originalSpotlightIntensity;
	private float viewAngle;
	private float playerVisibleTimer;
	private Animator animator;
	new private Rigidbody rigidbody;
	
	private NavMeshAgent navMeshAgent;

	void Start()
	{
		navMeshAgent				= GetComponent<NavMeshAgent>();
		navMeshAgent.enabled		= false;
		animator					= GetComponent<Animator>();
		rigidbody					= GetComponent<Rigidbody>();
		player						= GameObject.FindGameObjectWithTag ("Player").transform;
		Vector3[] waypoints			= new Vector3[pathHolder.childCount];

		for (int i = 0; i < waypoints.Length; i++)
		{
			waypoints [i] = pathHolder.GetChild (i).position;
			waypoints [i] = new Vector3 (waypoints [i].x, transform.position.y, waypoints [i].z);
		}

		status			= GuardStatus.Patrolling;
	}

	void Update()
	{
		if (CanSeePlayer ())
			playerVisibleTimer += Time.deltaTime;
		else
		playerVisibleTimer -= Time.deltaTime;

		playerVisibleTimer	= Mathf.Clamp (playerVisibleTimer, 0, timeToSpotPlayer);

		if (playerVisibleTimer >= timeToSpotPlayer)
		{
			status = GuardStatus.Pursuing;
			navMeshAgent.enabled = true;

		}

		if (status == GuardStatus.Pursuing)
		{
			ChasePlayer();
		}
	}

	void ChasePlayer()
	{
		Vector3 playerPosition		= player.transform.position;
		playerPosition.y			= 0;
		navMeshAgent.destination	= playerPosition; 
	}

	bool CanSeePlayer()
	{
		if (Vector3.Distance(transform.position,player.position) < viewDistance)
		{
			Vector3 dirToPlayer = (player.position - transform.position).normalized;

			float angleBetweenGuardAndPlayer = Vector3.Angle (transform.forward, dirToPlayer);

			if (angleBetweenGuardAndPlayer < viewAngle / 2f)
			{
				if (!Physics.Linecast (transform.position, player.position, viewMask))
				{
					return true;
				}
			}
		}
		return false;
	}



	//void FixedUpdate()
	//{
	//	Vector3 velocity = transform.forward * moveSpeed * smoothMoveTime;
	//	rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
		
	//}


	IEnumerator FollowPath(Vector3[] waypoints)
	{
		transform.position		= waypoints [0];
		int targetWaypointIndex = 1;
		Vector3 targetWaypoint	= waypoints [targetWaypointIndex];
		transform.LookAt (targetWaypoint);
		

		while (true)
		{
			
			transform.position = Vector3.MoveTowards (transform.position, targetWaypoint, moveSpeed * Time.deltaTime);
			if (transform.position == targetWaypoint)
			{
				targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
				targetWaypoint		= waypoints [targetWaypointIndex];
				animator.SetFloat("speed", 0);
				yield return new WaitForSeconds (Random.Range(minMaxWaitTime.x,minMaxWaitTime.y));
				//StraightTurn(targetWaypoint);
				yield return StartCoroutine (TurnToFace (targetWaypoint));
			}
			else animator.SetFloat("speed", 1);
			yield return null;
		}
	}


	void StraightTurn(Vector3 lookTarget)
	{
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
		transform.eulerAngles = Vector3.up*targetAngle;
	}

	IEnumerator TurnToFace(Vector3 lookTarget)
	{
		Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
		float targetAngle		= 90 - Mathf.Atan2 (dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

		while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
		{
			float angle				= Mathf.MoveTowardsAngle (transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
			transform.eulerAngles	= Vector3.up * angle;
			yield return null;
		}
	}

	void OnDrawGizmos()
	{
		Vector3 startPosition = pathHolder.GetChild (0).position;
		Vector3 previousPosition = startPosition;

		foreach (Transform waypoint in pathHolder)
		{
			Gizmos.DrawSphere (waypoint.position, .3f);
			Gizmos.DrawLine (previousPosition, waypoint.position);
			previousPosition = waypoint.position;
		}
		Gizmos.DrawLine (previousPosition, startPosition);

		Gizmos.color = Color.red;
		Gizmos.DrawRay (transform.position, transform.forward * viewDistance);
	}

}
