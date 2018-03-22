using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChickenStatus { Idle, Moving, Lured }
public class Chicken : NPC
{
	public ChickenStatus status;
	public float eatingSpeed	= .5f;
	public int eatSubstraction	= 5;

	public bool isPecking				= false;
	private Corn corn					= null;
	private Vector3 lastCornLocation	= Vector3.zero;
	public bool isEatingCorn			= false;

	protected override void Start()
	{
		base.Start();
		navMeshAgent.updateRotation = false;
		animator.SetFloat("speed", (moveAcrossWaypoints && waypoints.childCount > 0) ? 1 : 0);
	}

	protected virtual void Update()
	{
		if ((navMeshAgent.destination - transform.position).magnitude < .1f && status != ChickenStatus.Idle && status != ChickenStatus.Lured)
			StartCoroutine(GoToNextWaypoint());

		if(status == ChickenStatus.Lured)
		{
			if (!isEatingCorn)
			{
				timeToUpdateNavmeshDestination += Time.deltaTime;

				Vector3 cornPosition = corn.gameObject.transform.position;

				if (lastCornLocation != cornPosition && timeToUpdateNavmeshDestination >= navmeshUpdateRate)
				{
					navMeshAgent.SetDestination(cornPosition);
					lastCornLocation = cornPosition;
					InstantTurn(corn.gameObject.transform);
				}

				else isEatingCorn = true;
			}			

			if (isEatingCorn && !isPecking)
			{
				isPecking = true;
				StartCoroutine(CrtEatCorn());
				animator.SetBool("pecking", true);
				animator.SetFloat("speed", 0);
				Debug.Log("start eating");
			}
		}			
	}

	IEnumerator GoToNextWaypoint()
	{
		isPecking = (Random.Range(0f, 1f) > .3f) ? true : false;
		animator.SetBool("pecking", isPecking);
		animator.SetFloat("speed", 0);

		status					= ChickenStatus.Idle;		
		float randomIdleTime	= Random.Range(minMaxWaitBeforeMovingToNextWaypoint.x, minMaxWaitBeforeMovingToNextWaypoint.y);
		yield return new WaitForSecondsRealtime(randomIdleTime);

		currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.transform.childCount;
		InstantTurn(waypoints.GetChild(currentWaypointIndex));
		navMeshAgent.SetDestination(waypoints.GetChild(currentWaypointIndex).transform.position);
		isPecking	= false;
		status		= ChickenStatus.Moving;
		animator.SetBool("pecking", false);
		animator.SetFloat("speed", 1);
	}

	void InstantTurn(Transform destination)
	{
		Vector3 direction = (destination.position - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation(direction);
	}

	public void DistractedTo(GameObject obj)
	{
		status = ChickenStatus.Lured;
		timeToUpdateNavmeshDestination = navmeshUpdateRate;
		corn = obj.GetComponent<Corn>();
		navMeshAgent.stoppingDistance = .5f;
	}

	IEnumerator CrtEatCorn()
	{
		while(corn.health > 0 || corn != null)
		{
			corn.health -= eatSubstraction;
			yield return new WaitForSecondsRealtime(eatingSpeed);
		}

		corn			= null;
		isEatingCorn	= false;
		isPecking		= false;
		navMeshAgent.GoToShortestWaypointLocation(waypoints, ref currentWaypointIndex);
		status = ChickenStatus.Moving;
		animator.SetFloat("speed", 1);
		animator.SetBool("pecking", false);
		navMeshAgent.stoppingDistance = 0;
	}
}
