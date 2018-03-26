using UnityEditor;
using UnityEngine;

public class DistractorEditor : Editor
{

	private void OnSceneGUI()
	{
		Distractor distractor = (Distractor)target;
		Handles.color = Color.white;

		Handles.DrawWireArc(distractor.transform.position, Vector3.up, Vector3.forward, 360, distractor.Radius);
	}
}
