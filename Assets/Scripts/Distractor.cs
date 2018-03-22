using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distractor : MonoBehaviour
{
	[Range(0,20)]
	public float radius;

	[Range(1,20)]
	public float timeToDestroy;
	[Range(1,20)]

	public float distractionTime;
	public LayerMask agentsToDistract;
	public bool autoDestroy;

	private void Start()
	{
		if(autoDestroy)
			Destroy(gameObject, timeToDestroy);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		UnityEditor.Handles.DrawWireDisc(transform.position,Vector3.up, radius);
	}
}
