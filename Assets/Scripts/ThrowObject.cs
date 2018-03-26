using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ThrowObject : MonoBehaviour
{
	public Transform ObjectToThrow;
	public int LineRendererCount	= 30;
	public float ThrowHeight		= 25;
	public float Gravity			= -18;
	public float Radius				= 10f;
	public new Camera Camera;

	protected Vector3 LaunchPosition;
	protected Transform Target;
	protected GameObject BullsEye;
	protected LineRenderer LineRenderer;

	protected bool IsBeingThrown		= false;
	protected bool HasTarget			= false;
	protected Rigidbody TargetRigidbody;

	protected virtual void Start()
	{
		//dadad
		TargetRigidbody				= ObjectToThrow.GetComponent<Rigidbody>();
		BullsEye					= GameObject.Find("BullsEye");
		Target						= BullsEye.transform;
		LineRenderer				= GetComponent<LineRenderer>();
		LineRenderer.positionCount	= LineRendererCount;
		LaunchPosition				= transform.position;
		LineRenderer.enabled		= false;
	}

	protected bool TargetAvailable()
	{
		if (HasTarget)
		{
			Throw();
			HasTarget = false;
			return true;
		}
		else return false;
	}

	protected void SetTargetLocation()
	{
		if (!BullsEye.activeInHierarchy && !IsBeingThrown && PlayerSkillManager.Singleton.skillInUse != SkillInUse.None)
			BullsEye.SetActive(true);

		Ray cameraRay = Camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit cameraRayHit;

		if (Physics.Raycast(cameraRay, out cameraRayHit))
		{
			if (cameraRayHit.transform.tag == "Ground" || cameraRayHit.transform.tag == "Obstacle" || cameraRayHit.transform.tag == "Guard")
			{
				Vector3 targetPosition		= new Vector3(cameraRayHit.point.x, transform.position.y, cameraRayHit.point.z);
				Vector3 direction			= targetPosition - transform.position;
				direction					= Vector3.ClampMagnitude(direction, Radius);
				direction.y					= 0.2f;
				BullsEye.transform.position = transform.position + direction;

				DrawParabolicArc();
			}
		}
	}

	void DrawParabolicArc()
	{
		LaunchData launchData = CalculateLaunchData(TargetRigidbody);

		for (int i = 0; i < LineRendererCount; i++)
		{
			float simulationTime	= i / (float)LineRendererCount * launchData.timeToTarget;
			Vector3 displacement	= launchData.initialVelocity * simulationTime + Vector3.up * Gravity * simulationTime * simulationTime / 2f;
			Vector3 drawPoint		= LaunchPosition + displacement;
			LineRenderer.SetPosition(i, drawPoint);
		}
	}

	protected void Throw()
	{
		Physics.gravity				= Vector3.up * Gravity;
		TargetRigidbody.useGravity	= true;
		TargetRigidbody.velocity	= CalculateLaunchData(TargetRigidbody).initialVelocity;
	}

	protected LaunchData CalculateLaunchData(Rigidbody rigidbody)
	{
		float displacementY		= Target.position.y - LaunchPosition.y;
		Vector3 displacementXZ	= new Vector3(Target.position.x - LaunchPosition.x, 0, Target.position.z - LaunchPosition.z);
		float time				= Mathf.Sqrt(-2 * ThrowHeight / Gravity) + Mathf.Sqrt(2 * (displacementY - ThrowHeight) / Gravity);
		Vector3 velocityY		= Vector3.up * Mathf.Sqrt(-2 * Gravity * ThrowHeight);
		Vector3 velocityXZ		= displacementXZ / time;

		return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(Gravity), time);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (IsBeingThrown)
		{
			IsBeingThrown			= false;
			LineRenderer.enabled	= true;
			BullsEye.SetActive(false);
			PlayerSkillManager.Singleton.skillInUse = SkillInUse.None;
		}	
	}
}
