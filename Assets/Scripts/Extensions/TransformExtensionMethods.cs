using UnityEngine;

public static class TransformExtensionMethods
{
	public static Vector3 DirectionFromAngle(this Transform position, float angle, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
			angle += position.eulerAngles.y;
		return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
	}
}
