using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distractor : MonoBehaviour
{
	[Range(0,20)]
	public float Radius;

	[Range(1,20)]
	public float TimeToDestroy;
	[Range(1,20)]

	public float DistractionTime;
	public LayerMask AgentsToDistract;
	public bool AutoDestroy;

	private void Start()
	{
		if(AutoDestroy)
			Destroy(gameObject, TimeToDestroy);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		UnityEditor.Handles.DrawWireDisc(transform.position,Vector3.up, Radius);
	}
}
