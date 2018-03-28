using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPCs.Dog
{
	public class GuardDog : Guard
	{
		public int AttemptsToSearchNearbyLocations = 3;

		private DogFieldOfView _fov;
		private Vector3 _lastSearchLocation = Vector3.zero;

		protected override void Start()
		{
			base.Start();
			_fov		= GetComponentInChildren<DogFieldOfView>();
			Status	= GuardStatus.Patrolling;

			DogFieldOfView.OnDogHasSpottedPlayer += OnStartChasingPlayer;
			DogFieldOfView.OnDogIsLosingPlayer += OnStartLosingPlayer;
		}

		protected override void Update()
		{

			if (Status == GuardStatus.Patrolling)
			{
				if(NavMeshAgent.remainingDistance <= .2f)
					Animator.SetFloat("speed", 0);

				if (NavMeshAgent.remainingDistance <= 0)
					StartCoroutine(GoToNextWaypoint());

				else Animator.SetFloat("speed", 1);
			}

			else if(Status == GuardStatus.Pursuing)
			{
				TimeToUpdateNavmeshDestination += Time.deltaTime;
				Animator.SetFloat("speed", 1);
				if (TimeToUpdateNavmeshDestination >= NavmeshUpdateRate && NavMeshAgent.destination != Player.transform.position)
				{
					LastKnownPlayerPosition = Player.transform.position;
					NavMeshAgent.destination = LastKnownPlayerPosition;
					TimeToUpdateNavmeshDestination = 0;
				}
			}
			
			
			else if(Status == GuardStatus.Distracted && LostInterestToDistractor > 0)
			{
				if(Math.Abs(NavMeshAgent.remainingDistance) <= 0f)
				{
					LostInterestToDistractor -= Time.deltaTime;
					Animator.SetFloat("speed", 0);
				}
				
			}
			
			if (GoingBackToWaypoint)
			{
				if (NavMeshAgent.remainingDistance <= 1 && Status != GuardStatus.Distracted)
				{
					Animator.SetBool("running", false);
					Status = GuardStatus.Patrolling;
					NavMeshAgent.speed = OriginalSpeed;
					GoingBackToWaypoint = false;
				}
			}

			if(LostInterestToDistractor <= 0 && Status == GuardStatus.Distracted)
				ResumePatrolling();
		}


		private new IEnumerator GoToNextWaypoint()
		{
			if (LostInterestToDistractor > 0) StopCoroutine(GoToNextWaypoint());
			Status = GuardStatus.Idle;
			Animator.SetFloat("speed", 0);
			float randomIdleTime = Random.Range(MinMaxWaitBeforeMovingToNextWaypoint.x, MinMaxWaitBeforeMovingToNextWaypoint.y);
			yield return new WaitForSecondsRealtime(randomIdleTime);
		

			if (LostInterestToDistractor == 0)
			{
				CurrentWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.transform.childCount;
				NavMeshAgent.SetDestination(Waypoints.GetChild(CurrentWaypointIndex).transform.position);
				Status = GuardStatus.Patrolling;
				
			}
				
		}

		protected override void OnStartChasingPlayer()
		{
			base.OnStartChasingPlayer();
			NavMeshAgent.speed = RunSpeed;
		}

		protected IEnumerator LookingForPlayer()
		{
			int attempts = 1;
			NavMeshAgent.stoppingDistance = 0;

			while (attempts <= AttemptsToSearchNearbyLocations)
			{
				IsLookingForPlayer = true;
				yield return StartCoroutine(SearchRandomNearbyLocation());
				attempts++;
			}
		
			ResumePatrolling();
		}

		protected virtual void ResumePatrolling()
		{
			Debug.Log("resume");
			NavMeshAgent.speed = WalkSpeed;
			NavMeshAgent.stoppingDistance = 0;
			LostInterestToDistractor	= 0;
			IsLookingForPlayer			= false;
			Status						= GuardStatus.Patrolling;	
			GoingBackToWaypoint			= true;
			NavMeshAgent.GoToShortestWaypointLocation(Waypoints, ref CurrentWaypointIndex);
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
			Debug.Log("search random");
			Vector3 randomLocation					= Vector3.zero;
			float distanceFromCurrentLocation		= 0;
			do
			{
				randomLocation					= LastKnownPlayerPosition + (Vector3)Random.insideUnitCircle * SearchPlayerRadius;
				distanceFromCurrentLocation		= Vector3.Distance(transform.position, randomLocation);

				_lastSearchLocation = randomLocation;
			}
			while (!IsNavmeshPositionValid(randomLocation) && distanceFromCurrentLocation < MinimumSearchDistance);

			NavMeshAgent.destination = randomLocation;

			while (IsLookingForPlayer)
			{
				yield return null;
			}
		}

		protected override void OnStartLosingPlayer()
		{
			base.OnStartLosingPlayer();
			StartCoroutine(LookingForPlayer());
			NavMeshAgent.GoToShortestWaypointLocation(Waypoints, ref CurrentWaypointIndex);
			//navMeshAgent.destination	= navMeshAgent.getsho(transform, waypoints, ref currentWaypointIndex);
			GoingBackToWaypoint			= true;
		}

		public override void DistractedTo(GameObject obj, float time)
		{
			StopCoroutine(GoToNextWaypoint());
			//StopCoroutine(GoingBackToWaypoint());
			StopCoroutine(LookingForPlayer());
			LostInterestToDistractor = time;
			base.DistractedTo(obj, time);

			if (!IsNavmeshPositionValid(obj.transform.position))
			{
				ResumePatrolling();
				LostInterestToDistractor = 0;
			}
				
			Animator.SetFloat("speed", 1);
			NavMeshAgent.SetDestination(obj.transform.position);		
		}
	}
}
