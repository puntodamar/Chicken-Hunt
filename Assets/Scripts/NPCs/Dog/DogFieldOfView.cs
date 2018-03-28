using System.Collections;
using Player.Skills;
using UnityEngine;

namespace NPCs.Dog
{
	[RequireComponent(typeof(GuardDog))]
	public class DogFieldOfView : FieldOfView
	{
		public static event System.Action OnDogHasSpottedPlayer;
		public static event System.Action OnDogIsLosingPlayer;
		public float TimeToSpotPlayer = .5f;
		
		[HideInInspector]
		private Transform _player;
		private float _playerVisibleTimer	= 0f;
		private GuardDog _dog;
		private bool _fovColorIsOverriden	= false;
		private IEnumerator _activeCoroutine;
		private bool _playerIsHiding = false;
		
		public DogFieldOfView(IEnumerator activeCoroutine)
		{
			this._activeCoroutine = activeCoroutine;
		}

		protected override void Start()
		{
			base.Start();
			_dog			= GetComponentInParent<GuardDog>();
			_player		= GameObject.Find("Player").transform;
			HideSkill.OnPlayerEnterBushes += HidePlayerFromGuard;
			HideSkill.OnPlayerExitBushes += ShowPlayerFromGuard;
		}

		private void Update()
		{
			if (CanSeePlayer())
			{
				if (_dog.Status == GuardStatus.Idle || _dog.Status == GuardStatus.Patrolling || _dog.Status != GuardStatus.Patrolling)
					_playerVisibleTimer += Time.deltaTime;
				if (_dog.Status == GuardStatus.Pursuing || _dog.Status == GuardStatus.Searching)
					_playerVisibleTimer = TimeToSpotPlayer;
			}
			else
				_playerVisibleTimer -= Time.deltaTime;

			

			if (_dog.Status != GuardStatus.Searching)
			{
				_playerVisibleTimer = Mathf.Clamp(_playerVisibleTimer, 0, TimeToSpotPlayer);

				if (!_fovColorIsOverriden)
					Renderer.material.color = Color.Lerp(DefaultFovColor, Color.red, _playerVisibleTimer / TimeToSpotPlayer);
			}

			if (_playerVisibleTimer >= TimeToSpotPlayer && _dog.Status != GuardStatus.Pursuing)
			{
				if (OnDogHasSpottedPlayer != null)
				{
					OnDogHasSpottedPlayer();
				}
			}

			if (_playerVisibleTimer == 0f && _dog.Status == GuardStatus.Pursuing)
			{
				if (OnDogIsLosingPlayer != null)
					OnDogIsLosingPlayer();
			}

			//if(playerVisibleTimer == time)
		}

		public void ChangeFovColor(bool transitionToDefaultFovColor, Color destination)
		{
			_fovColorIsOverriden = !transitionToDefaultFovColor;
			if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
		
			if(_fovColorIsOverriden)
				StartCoroutine(ChangeFovColorTo(destination));
		}

		public IEnumerator ChangeFovColorTo(Color destination)
		{
			Color current = Renderer.material.color;

			while (Renderer.material.color != destination)
			{
				//renderer.material.color = Color.Lerp(defaultFOVColor, Color.red, playerVisibleTimer / timeToSpotPlayer);
				Renderer.material.color = Color.Lerp(current, destination, Time.time);
				yield return null;
			}
		}

		bool CanSeePlayer()
		{		
			if(_playerIsHiding) return false;
			
			float distance = Vector3.Distance(transform.position, _player.position);
			if (distance < FovRadius)
			{
				if (distance < .5f) return true;

				Vector3 dirToPlayer					= (_player.position - transform.position).normalized;
				float angleBetweenGuardAndPlayer	= Vector3.Angle(transform.forward, dirToPlayer);

				if (angleBetweenGuardAndPlayer < ViewAngle / 2f)
				{
					if (!Physics.Linecast(transform.position, _player.position, ObstacleMask))
					{
						return true;
					}
				}								
			}

			return false;
		}

		void HidePlayerFromGuard()
		{
			if (_dog.Status != GuardStatus.Pursuing)
				_playerIsHiding = true;
				
		}

		void ShowPlayerFromGuard()
		{
			_playerIsHiding = false;
		}
	}
}
