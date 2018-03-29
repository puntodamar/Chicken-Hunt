using Extensions;
using NPCs;
using NPCs.Dog;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DogFieldOfView), true)]
public class FieldOfViewEditor : Editor
{
	private void OnSceneGUI()
	{
		var fov = (FieldOfView) target;
		Handles.color = Color.red;
		Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.FovRadius);

		var viewAngleA = fov.transform.DirectionFromAngle(-fov.ViewAngle / 2, false);
		var viewAngleB = fov.transform.DirectionFromAngle(fov.ViewAngle / 2, false);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.FovRadius);
		Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.FovRadius);

		Handles.color = Color.red;

		foreach (var visibleTarget in fov.VisibleTargets) 
			Handles.DrawLine(fov.transform.position, visibleTarget.position);
	}
}
