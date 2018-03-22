using UnityEngine;
using UnityEngine.AI;

public static class NavMeshAgentExtensionMethods
{
	public static void GoToShortestWaypointLocation(this NavMeshAgent agent, Transform waypoints, ref int index)
	{
		float distance = 1000;
		Vector3 target = Vector3.zero;

		for (int i = 0; i < waypoints.childCount; i++)
		{
			Vector3 waypointPosition = waypoints.GetChild(i).transform.position;
			float distanceToWaypoint = Vector3.Distance(waypointPosition, agent.transform.position);
			if (distanceToWaypoint < distance)
			{
				index = i;
				distance = distanceToWaypoint;
				target = waypointPosition;
			}

			agent.SetDestination(target);
		}
	}
}
