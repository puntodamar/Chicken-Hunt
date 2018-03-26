using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
	public float ViewDistance;	
	//public LayerMask viewMask;
	public Transform Waypoints;
	public Vector2 MinMaxWaitBeforeMovingToNextWaypoint;
	public bool MoveAcrossWaypoints = true;
	public Color WaypointGizmoColor;

	protected Animator Animator;
	protected Transform Player;
	protected new  Rigidbody Rigidbody;
	protected NavMeshAgent NavMeshAgent;
	protected float TimeToUpdateNavmeshDestination;
	public float NavmeshUpdateRate = .5f;
	protected float ViewAngle;
	protected float OriginalSpeed;
	protected int CurrentWaypointIndex = 0;

	protected virtual void Start()
	{
		Animator		= GetComponent<Animator>();
		NavMeshAgent	= GetComponent<NavMeshAgent>();		
		Player			= GameObject.FindGameObjectWithTag("Player").transform;
		OriginalSpeed	= NavMeshAgent.speed;

		if (MoveAcrossWaypoints && NavMeshAgent.enabled)
		{
			NavMeshAgent.Warp(transform.position);
			NavMeshAgent.SetDestination(Waypoints.GetChild(CurrentWaypointIndex).transform.position);
		}
	}

	public virtual void DistractedTo(GameObject obj, float time)
	{
		
	}

	protected void OnDrawGizmos()
	{
		if (Waypoints.childCount == 0) return;

		Vector3 startPosition		= Waypoints.GetChild(0).position;
		Vector3 previousPosition	= startPosition;

		Gizmos.color = Color.red;

		foreach (Transform waypoint in Waypoints)
		{
			Gizmos.DrawSphere(waypoint.position, .3f);
			Gizmos.DrawLine(previousPosition, waypoint.position);
			previousPosition = waypoint.position;
		}
		Gizmos.DrawLine(previousPosition, startPosition);
	}
}



