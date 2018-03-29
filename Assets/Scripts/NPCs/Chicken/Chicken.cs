using System.Collections;
using System.Collections.Generic;
using Extensions;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace NPCs.Chicken
{
	public enum ChickenState { Idle, Moving, Lured }
	public sealed class Chicken : NPC
	{
		public static System.Action OnChickenDead;
		public static System.Action OnChickenIsNotInRange;
		
		public ChickenState ChickenState;
		public float EatingSpeed			= .5f;
		public int EatSubstraction			= 5;
		public bool IsPecking				= false;
		public bool IsEatingCorn			= false;
		public float Health 				= 100;
		
		private Corn _corn					= null;
		private Vector3 _lastCornLocation	= Vector3.zero;
		public float _currentHealth;
		private Slider _eatingProgress;

		protected override void Start()
		{
			base.Start();
			NavMeshAgent.updateRotation = false;
			Animator.SetFloat("speed", (MoveAcrossWaypoints && Waypoints.childCount > 0) ? 1 : 0);
			_eatingProgress = transform.FindDeepChild("EatSlider").GetComponent<Slider>();
			_currentHealth = Health;
		}

		private void Update()
		{
			if ((NavMeshAgent.destination - transform.position).magnitude < .1f && ChickenState != ChickenState.Idle && ChickenState != ChickenState.Lured)
				StartCoroutine(GoToNextWaypoint());

			if(ChickenState == ChickenState.Lured)
			{
				if (!IsEatingCorn)
				{
					TimeToUpdateNavmeshDestination += Time.deltaTime;

					Vector3 cornPosition = _corn.gameObject.transform.position;

					if (_lastCornLocation != cornPosition && TimeToUpdateNavmeshDestination >= NavmeshUpdateRate)
					{
						NavMeshAgent.SetDestination(cornPosition);
						_lastCornLocation = cornPosition;
						InstantTurn(_corn.gameObject.transform);
					}

					if (Vector3.Distance(_lastCornLocation, transform.position) < .6f)
						IsEatingCorn = true;
				}			

				if (IsEatingCorn && !IsPecking)
				{
					IsPecking = true;
					StartCoroutine(CrtEatCorn());
					Animator.SetBool("pecking", true);
					Animator.SetFloat("speed", 0);
					Debug.Log("start eating");
				}
			}			
		}

		IEnumerator GoToNextWaypoint()
		{
			IsPecking = (Random.Range(0f, 1f) > .3f) ? true : false;
			Animator.SetBool("pecking", IsPecking);
			Animator.SetFloat("speed", 0);

			ChickenState					= ChickenState.Idle;		
			float randomIdleTime	= Random.Range(MinMaxWaitBeforeMovingToNextWaypoint.x, MinMaxWaitBeforeMovingToNextWaypoint.y);
			yield return new WaitForSecondsRealtime(randomIdleTime);

			CurrentWaypointIndex = (CurrentWaypointIndex + 1) % Waypoints.transform.childCount;
			InstantTurn(Waypoints.GetChild(CurrentWaypointIndex));
			NavMeshAgent.SetDestination(Waypoints.GetChild(CurrentWaypointIndex).transform.position);
			IsPecking	= false;
			ChickenState		= ChickenState.Moving;
			Animator.SetBool("pecking", false);
			Animator.SetFloat("speed", 1);
		}

		void InstantTurn(Transform destination)
		{
			Vector3 direction = (destination.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(direction);
		}

		public void DistractedTo(GameObject obj)
		{
			ChickenState = ChickenState.Lured;
			TimeToUpdateNavmeshDestination = NavmeshUpdateRate;
			_corn = obj.GetComponent<Corn>();
			NavMeshAgent.stoppingDistance = .5f;
		}

		IEnumerator CrtEatCorn()
		{
			while(_corn.Health > 0 || _corn != null)
			{
				_corn.Health -= EatSubstraction;
				yield return new WaitForSecondsRealtime(EatingSpeed);
			}

			_corn			= null;
			IsEatingCorn	= false;
			IsPecking		= false;
			NavMeshAgent.GoToShortestWaypointLocation(Waypoints, ref CurrentWaypointIndex);
			ChickenState = ChickenState.Moving;
			Animator.SetFloat("speed", 1);
			Animator.SetBool("pecking", false);
			NavMeshAgent.stoppingDistance = 0;
			//avmeshUpdateRate = .1f;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Player")) return;
			if (Guard.IsChasingPlayer) return;

			other.gameObject.GetComponent<EatSkill>().CanEatChicken(true, this);
			//Destroy(gameObject);
		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.CompareTag("Player")) return;

			if (OnChickenIsNotInRange != null)
				OnChickenIsNotInRange();
		}

		public void Eaten()
		{
			_currentHealth -= 25;
			
			if (_currentHealth == 0)
			{
				GameManager.Singleton.ChickenCaught();
				if (OnChickenDead != null)
					OnChickenDead();
				Destroy(gameObject, .5f);
			}
			
			float healthPercentage = _currentHealth / 100;
			_eatingProgress.value = healthPercentage;

		}
	}
	
}