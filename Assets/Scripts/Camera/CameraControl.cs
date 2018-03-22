using UnityEngine;

public class CameraControl : MonoBehaviour
{

	Transform player;
	Camera m_Camera;                        // Used for referencing the camera.
	Vector3 m_DesiredPosition;              // The position the camera is moving towards.


	private void Awake()
	{
		m_Camera	= GetComponentInChildren<Camera>();
		player		= GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void Start()
	{
		transform.position = player.position;
	}


	private void FixedUpdate()
	{
		m_DesiredPosition = player.position;

		//transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
		transform.position = m_DesiredPosition;
	}
}