using UnityEngine;

namespace Player
{
	[RequireComponent(typeof(PlayerManager))]
	public class PlayerMovement : MonoBehaviour
	{
		public float MoveSpeed			= 7f;
		public float SmoothMoveTime		= .1f;
		public float TurnSpeed			= 8;
		public bool Disabled			= false;

		private float _angle;
		private float _smoothInputMagnitude;
		private float _smoothMoveVelocity;
		private Vector3 _velocity;
		private new Rigidbody _rigidbody;
		private Animator _animator;

		void Start()
		{
			_rigidbody	= PlayerManager.Singleton.PlayerRigidbody;
			_animator	= PlayerManager.Singleton.PlayerAnimator;
		}

		void Update ()
		{
		
			if (PlayerManager.Singleton.IsRespawning) return;

			Vector3 inputDirection = Vector3.zero;
			if (!Disabled)
			{
			
				inputDirection			= new Vector3 (Input.GetAxisRaw ("Vertical"), 0, -Input.GetAxisRaw ("Horizontal")).normalized;
				float inputMagnitude	= inputDirection.magnitude;
				_animator.SetFloat("speed", inputMagnitude);
				_smoothInputMagnitude	= Mathf.SmoothDamp(_smoothInputMagnitude, inputMagnitude, ref _smoothMoveVelocity, SmoothMoveTime);

				float targetAngle		= Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
				_angle					= Mathf.LerpAngle(_angle, targetAngle, Time.deltaTime * TurnSpeed * inputMagnitude);
			}
			_velocity			= transform.forward * MoveSpeed * _smoothInputMagnitude;


		}

		void FixedUpdate()
		{
			if (!Disabled)
			{
				_rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * _angle));
				_rigidbody.MovePosition(_rigidbody.position + _velocity * Time.deltaTime);
			}
		}
	}
}
