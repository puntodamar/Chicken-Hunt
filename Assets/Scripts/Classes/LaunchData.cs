using UnityEngine;

public struct LaunchData
{
	public readonly Vector3 initialVelocity;
	public readonly float timeToTarget;

	public LaunchData(Vector3 initialVelocity, float timeToTarget)
	{
		this.initialVelocity	= initialVelocity;
		this.timeToTarget		= timeToTarget;
	}
}