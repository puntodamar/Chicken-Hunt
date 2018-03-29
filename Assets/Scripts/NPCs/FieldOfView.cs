using System.Collections.Generic;
using Extensions;
using Managers;
using UnityEngine;

namespace NPCs
{
	public class FieldOfView : MonoBehaviour
	{				
		public float FovRadius	= 10f;
		[Range(0, 360)]
		public float ViewAngle = 100f, MeshResolution, EdgeDistanceTreshold;
		public Color DefaultFovColor;
		public int EdgeResolveIteration;		
		public LayerMask TargetMask;
		public LayerMask ObstacleMask;

		[HideInInspector]
		public List<Transform> VisibleTargets = new List<Transform>();
		public MeshFilter ViewMeshFilter;

		protected new Renderer Renderer;
		private Mesh ViewMesh;

		protected virtual void Start()
		{
			ViewMesh				= new Mesh();
			ViewMesh.name			= "View Mesh";
			ViewMeshFilter.mesh		= ViewMesh;
			Renderer				= GetComponent<Renderer>();
			Renderer.material.color = DefaultFovColor;

			GameManager.OnGameOver += Disable;

		}

		protected void LateUpdate()
		{
			DrawFieldOfView();
		}

		protected void DrawFieldOfView()
		{
			int stepCount				= Mathf.RoundToInt(ViewAngle * MeshResolution);
			float stepAngleSize			= ViewAngle / stepCount;
			List<Vector3> viewPoints	= new List<Vector3>();
			ViewCastInfo oldViewCast	= new ViewCastInfo();

			for (int i = 0; i <= stepCount; i++)
			{
				float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
				ViewCastInfo newViewCast = ViewCast(angle);
			
				Debug.DrawLine(transform.position, transform.position + transform.DirectionFromAngle(angle, true) * newViewCast.distance, Color.black);

				if (i > 0)
				{
					bool edgeDistanceTresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > EdgeDistanceTreshold;

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

				ViewMesh.Clear();
				ViewMesh.vertices	= vertices;
				ViewMesh.triangles	= triangles;
				ViewMesh.RecalculateNormals();

			}
		}

		protected ViewCastInfo ViewCast(float globalAngle)
		{
			Vector3 direction = transform.DirectionFromAngle(globalAngle, true);
			RaycastHit hit;

			if(Physics.Raycast(transform.position, direction, out hit, FovRadius, ObstacleMask) || Physics.Raycast(transform.position, direction, out hit, FovRadius, TargetMask))
				return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
			else
				return new ViewCastInfo(false, transform.position + direction * FovRadius,FovRadius, globalAngle);
		}

		protected EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
		{
			float minAngle = minViewCast.angle;
			float maxAngle = maxViewCast.angle;

			Vector3 minPoint = Vector3.zero;
			Vector3 maxPoint = Vector3.zero;

			for (int i = 0; i < EdgeResolveIteration; i++)
			{
				float angle = (minAngle + maxAngle) / 2;
				ViewCastInfo newViewCast = ViewCast(angle);

				bool edgeDistanceTresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > EdgeDistanceTreshold;
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
		
		void Disable()
		{
			this.enabled = false;
		}
	}
}