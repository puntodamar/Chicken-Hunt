using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ThrowObject : MonoBehaviour
{
	public Transform objectToThrow;
	public int lineRendererCount	= 30;
	public float throwHeight		= 25;
	public float gravity			= -18;
	public float radius				= 10f;
	public new Camera camera;

	protected Vector3 launchPosition;
	protected Transform target;
	protected GameObject bullsEye;
	protected LineRenderer lineRenderer;

	protected bool isBeingThrown		= false;
	protected bool hasTarget			= false;
	protected Rigidbody targetRigidbody;

	protected virtual void Start()
	{
		//dadad
		targetRigidbody				= objectToThrow.GetComponent<Rigidbody>();
		bullsEye					= GameObject.Find("BullsEye");
		target						= bullsEye.transform;
		lineRenderer				= GetComponent<LineRenderer>();
		lineRenderer.positionCount	= lineRendererCount;
		launchPosition				= transform.position;
		lineRenderer.enabled		= false;
	}

	protected bool TargetAvailable()
	{
		if (hasTarget)
		{
			Throw();
			hasTarget = false;
			return true;
		}
		else return false;
	}

	protected void SetTargetLocation()
	{
		if (!bullsEye.activeInHierarchy && !isBeingThrown && PlayerSkillManager.Singleton.skillInUse != SkillInUse.None)
			bullsEye.SetActive(true);

		Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit cameraRayHit;

		if (Physics.Raycast(cameraRay, out cameraRayHit))
		{
			if (cameraRayHit.transform.tag == "Ground" || cameraRayHit.transform.tag == "Obstacle" || cameraRayHit.transform.tag == "Guard")
			{
				Vector3 targetPosition		= new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
				Vector3 direction			= targetPosition - transform.position;
				direction					= Vector3.ClampMagnitude(direction, radius);
				direction.y					= 0.2f;
				bullsEye.transform.position = transform.position + direction;

				DrawParabolicArc();
			}
		}
	}

	void DrawParabolicArc()
	{
		LaunchData launchData = CalculateLaunchData(targetRigidbody);

		for (int i = 0; i < lineRendererCount; i++)
		{
			float simulationTime	= i / (float)lineRendererCount * launchData.timeToTarget;
			Vector3 displacement	= launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
			Vector3 drawPoint		= launchPosition + displacement;
			lineRenderer.SetPosition(i, drawPoint);
		}
	}

	protected void Throw()
	{
		Physics.gravity				= Vector3.up * gravity;
		targetRigidbody.useGravity	= true;
		targetRigidbody.velocity	= CalculateLaunchData(targetRigidbody).initialVelocity;
	}

	protected LaunchData CalculateLaunchData(Rigidbody rigidbody)
	{
		float displacementY		= target.position.y - launchPosition.y;
		Vector3 displacementXZ	= new Vector3(target.position.x - launchPosition.x, 0, target.position.z - launchPosition.z);
		float time				= Mathf.Sqrt(-2 * throwHeight / gravity) + Mathf.Sqrt(2 * (displacementY - throwHeight) / gravity);
		Vector3 velocityY		= Vector3.up * Mathf.Sqrt(-2 * gravity * throwHeight);
		Vector3 velocityXZ		= displacementXZ / time;

		return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isBeingThrown)
		{
			isBeingThrown			= false;
			lineRenderer.enabled	= true;
			bullsEye.SetActive(false);
			PlayerSkillManager.Singleton.skillInUse = SkillInUse.None;
		}	
	}
}
