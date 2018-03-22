﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	
	public float FOVRadius	= 10f;
	[Range(0, 360)]
	public float viewAngle = 100f, meshResolution, edgeDistanceTreshold;
	public Color defaultFOVColor;
	public int edgeResolveIteration;		
	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();
	public MeshFilter viewMeshFilter;

	protected new Renderer renderer;
	private Mesh viewMesh;

	protected virtual void Start()
	{
		viewMesh				= new Mesh();
		viewMesh.name			= "View Mesh";
		viewMeshFilter.mesh		= viewMesh;
		renderer				= GetComponent<Renderer>();
		renderer.material.color = defaultFOVColor;
		//StartCoroutine(FindTargetsWithDelay(2));
		
	}

	//protected void FindVisibleTargets()
	//{
	//	visibleTargets.Clear();
	//	Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, FOVRadius, targetMask);

	//	for (int i = 0; i < targetsInViewRadius.Length; i++)
	//	{
	//		Transform target = targetsInViewRadius[i].transform;
	//		Vector3 directionToTarget = (target.position - transform.position).normalized;

	//		if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
	//		{
	//			float distanceToTarget = Vector3.Distance(transform.position, target.position);
	//			if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
	//			{
	//				visibleTargets.Add(target);
	//			}
	//		}
	//	}
	//}

	protected void LateUpdate()
	{
		DrawFieldOfView();
	}

	protected void DrawFieldOfView()
	{
		int stepCount				= Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize			= viewAngle / stepCount;
		List<Vector3> viewPoints	= new List<Vector3>();
		ViewCastInfo oldViewCast	= new ViewCastInfo();

		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);
			
			Debug.DrawLine(transform.position, transform.position + transform.DirectionFromAngle(angle, true) * newViewCast.distance, Color.black);

			if (i > 0)
			{
				bool edgeDistanceTresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceTreshold;

				if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceTresholdExceeded))
				{
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if(edge.pointA != Vector3.zero)
					{
						viewPoints.Add(edge.pointA);
					}
					if(edge.pointB != Vector3.zero)
					{
						viewPoints.Add(edge.pointB);
					}
				}
			}

			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;

		}

		int vertexCount		= viewPoints.Count + 1;
		Vector3[] vertices	= new Vector3[vertexCount];
		int[] triangles		= new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for (int i = 0; i < vertexCount-1; i++)
		{
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if(i < vertexCount - 2)
			{
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}

			viewMesh.Clear();
			viewMesh.vertices	= vertices;
			viewMesh.triangles	= triangles;
			viewMesh.RecalculateNormals();

		}
	}

	protected ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 direction = transform.DirectionFromAngle(globalAngle, true);
		RaycastHit hit;

		if(Physics.Raycast(transform.position, direction, out hit, FOVRadius, obstacleMask) || Physics.Raycast(transform.position, direction, out hit, FOVRadius, targetMask))
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		else
			return new ViewCastInfo(false, transform.position + direction * FOVRadius,FOVRadius, globalAngle);
	}

	protected EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;

		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIteration; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

			bool edgeDistanceTresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceTreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDistanceTresholdExceeded)
			{
				minViewCast.angle	= angle;
				minPoint			= newViewCast.point;
			}

			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo(minPoint, maxPoint);

	}

	//protected IEnumerator FindTargetsWithDelay(float delay)
	//{
	//	while (true)
	//	{
	//		yield return new WaitForSeconds(delay);
	//		FindVisibleTargets();
	//	}
	//}
}