using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DogFieldOfView), true)]
public class FieldOfViewEditor : Editor
{
	private void OnSceneGUI()
	{
		FieldOfView fov = (FieldOfView)target;
		Handles.color = Color.red;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.FOVRadius);

		Vector3 viewAngleA = fov.transform.DirectionFromAngle(-fov.viewAngle / 2, false);
		Vector3 viewAngleB = fov.transform.DirectionFromAngle(fov.viewAngle / 2, false);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.FOVRadius);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.FOVRadius);

		Handles.color = Color.red;

		foreach(Transform visibleTarget in fov.visibleTargets)
		{
			Handles.DrawLine(fov.transform.position, visibleTarget.position);
		}
	}
}
