using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerMovement : MonoBehaviour
{
	public float moveSpeed			= 7f;
	public float smoothMoveTime		= .1f;
	public float turnSpeed			= 8;
	public bool disabled			= false;

	private float angle;
	private float smoothInputMagnitude;
	private float smoothMoveVelocity;
	private Vector3 velocity;
	private new Rigidbody rigidbody;
	private Animator animator;

	void Start()
	{
		rigidbody	= PlayerManager.Singleton.playerRigidbody;
		animator	= PlayerManager.Singleton.playerAnimator;
	}

	void Update ()
	{
		if (PlayerManager.Singleton.isRespawning) return;

		Vector3 inputDirection = Vector3.zero;
		if (!disabled)
		{
			
			inputDirection			= new Vector3 (Input.GetAxisRaw ("Vertical1"), 0, -Input.GetAxisRaw ("Horizontal1")).normalized;
			float inputMagnitude	= inputDirection.magnitude;
			animator.SetFloat("speed", inputMagnitude);
			smoothInputMagnitude	= Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

			float targetAngle		= Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
			angle					= Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
		}
		velocity			= transform.forward * moveSpeed * smoothInputMagnitude;


	}

	void FixedUpdate()
	{
		if (!disabled)
		{
			rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
			rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
		}
	}
}
