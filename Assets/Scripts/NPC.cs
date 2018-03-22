using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
	public float viewDistance;	
	//public LayerMask viewMask;
	public Transform waypoints;
	public Vector2 minMaxWaitBeforeMovingToNextWaypoint;
	public bool moveAcrossWaypoints = true;
	public Color waypointGizmoColor;

	protected Animator animator;
	protected Transform player;
	protected new  Rigidbody rigidbody;
	protected NavMeshAgent navMeshAgent;
	protected float timeToUpdateNavmeshDestination;
	public float navmeshUpdateRate = .5f;
	protected float viewAngle;
	protected float originalSpeed;
	protected int currentWaypointIndex = 0;

	protected virtual void Start()
	{
		animator		= GetComponent<Animator>();
		navMeshAgent	= GetComponent<NavMeshAgent>();		
		player			= GameObject.FindGameObjectWithTag("Player").transform;
		originalSpeed	= navMeshAgent.speed;

		if (moveAcrossWaypoints && navMeshAgent.enabled)
		{
			navMeshAgent.Warp(transform.position);
			navMeshAgent.SetDestination(waypoints.GetChild(currentWaypointIndex).transform.position);
		}
	}

	public virtual void DistractedTo(GameObject obj, float time)
	{
		
	}

	protected void OnDrawGizmos()
	{
		if (waypoints.childCount == 0) return;

		Vector3 startPosition		= waypoints.GetChild(0).position;
		Vector3 previousPosition	= startPosition;

		Gizmos.color = Color.red;

		foreach (Transform waypoint in waypoints)
		{
			Gizmos.DrawSphere(waypoint.position, .3f);
			Gizmos.DrawLine(previousPosition, waypoint.position);
			previousPosition = waypoint.position;
		}
		Gizmos.DrawLine(previousPosition, startPosition);
	}
}



