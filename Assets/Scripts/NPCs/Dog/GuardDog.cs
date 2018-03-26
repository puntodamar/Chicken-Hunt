using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
			status	= GuardStatus.Patrolling;

			DogFieldOfView.OnDogHasSpottedPlayer += OnStartChasingPlayer;
			DogFieldOfView.OnDogIsLosingPlayer += OnStartLosingPlayer;
		}

		protected override void Update()
		{
			base.Update();

			if (goingBackToWaypoint)
			{
				if (NavMeshAgent.remainingDistance <= 1)
				{
					Animator.SetBool("running", false);
					status = GuardStatus.Patrolling;
					NavMeshAgent.speed = OriginalSpeed;
					goingBackToWaypoint = false;
				}
			}

			if(status == GuardStatus.Distracted && lostInterestToDistractor > 0)
			{
				if(NavMeshAgent.remainingDistance == 0)
				{
					lostInterestToDistractor -= Time.deltaTime;
					Animator.SetFloat("speed", 0);
				}
				
			}

			if(lostInterestToDistractor <= 0 && status == GuardStatus.Distracted)
				ResumePatrolling();

//			if (status == GuardStatus.Pursuing)
//			{
//				if(NavMeshAgent.remainingDistance < 4f)
//					NavMeshAgent.stoppingDistance = 5f;
//				else NavMeshAgent.stoppingDistance = 0f;
//			}
		}

		IEnumerator GoToNextWaypoint()
		{
			status = GuardStatus.Idle;
			Animator.SetFloat("speed", 0);
			float randomIdleTime = Random.Range(MinMaxWaitBeforeMovingToNextWaypoint.x, MinMaxWaitBeforeMovingToNextWaypoint.y);
			yield return new WaitForSecondsRealtime(randomIdleTime);
		
			CurrentWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.transform.childCount;
			NavMeshAgent.SetDestination(Waypoints.GetChild(CurrentWaypointIndex).transform.position);
			status = GuardStatus.Patrolling;
		}

		protected override void OnStartChasingPlayer()
		{
			base.OnStartChasingPlayer();
			//NavMeshAgent.stoppingDistance = 4f;
		}

		protected IEnumerator LookingForPlayer()
		{
			int attempts = 1;
			NavMeshAgent.stoppingDistance = 0;

			while (attempts <= AttemptsToSearchNearbyLocations)
			{
				isLookingForPlayer = true;
				yield return StartCoroutine(SearchRandomNearbyLocation());
				attempts++;
			}
		
			ResumePatrolling();
		}

		protected virtual void ResumePatrolling()
		{
			NavMeshAgent.stoppingDistance = 0;
			lostInterestToDistractor	= 0;
			isLookingForPlayer			= false;
			status						= GuardStatus.Patrolling;	
			goingBackToWaypoint			= true;
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
			Vector3 randomLocation					= Vector3.zero;
			float distanceFromCurrentLocation		= 0;
			do
			{
				randomLocation					= lastKnownPlayerPosition + (Vector3)Random.insideUnitCircle * searchPlayerRadius;
				distanceFromCurrentLocation		= Vector3.Distance(transform.position, randomLocation);

				_lastSearchLocation = randomLocation;
			}
			while (!IsNavmeshPositionValid(randomLocation) && distanceFromCurrentLocation < minimumSearchDistance);

			NavMeshAgent.destination = randomLocation;

			while (isLookingForPlayer)
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
			goingBackToWaypoint			= true;
		}

		public override void DistractedTo(GameObject obj, float time)
		{
			base.DistractedTo(obj, time);

			if (!IsNavmeshPositionValid(obj.transform.position)) return;

			StopCoroutine(LookingForPlayer());
			StopCoroutine(GoToNextWaypoint());

			Animator.SetFloat("speed", 1);
			NavMeshAgent.SetDestination(obj.transform.position);
			lostInterestToDistractor = time;
		}
	}
}
