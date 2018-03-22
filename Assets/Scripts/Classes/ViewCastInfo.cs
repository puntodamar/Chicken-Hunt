using UnityEngine;

public struct ViewCastInfo
{
	public bool hit;
	public Vector3 point;
	public float distance;
	public float angle;

	public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
	{
		this.hit		= hit;
		this.point		= point;
		this.distance	= distance;
		this.angle		= angle;
	}
}