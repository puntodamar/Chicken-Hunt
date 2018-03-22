using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackPlayer), true)]

public class AttackRadiusEditor : Editor
{
	private void OnSceneGUI()
	{
		AttackPlayer attackPlayer = (AttackPlayer)target;
		Handles.color = Color.red;
		Handles.DrawWireArc(attackPlayer.transform.position, Vector3.up, Vector3.forward, attackPlayer.attackAngle, attackPlayer.attackRadius);

		Vector3 viewAngleA = attackPlayer.transform.DirectionFromAngle(-attackPlayer.attackAngle / 2, false);
		Vector3 viewAngleB = attackPlayer.transform.DirectionFromAngle(attackPlayer.attackAngle / 2, false);

		Handles.DrawLine(attackPlayer.transform.position, attackPlayer.transform.position + (viewAngleB-viewAngleA*-1) * attackPlayer.attackRadius/2);
		Handles.DrawLine(attackPlayer.transform.position, attackPlayer.transform.position + viewAngleA * attackPlayer.attackRadius);
		Handles.DrawLine(attackPlayer.transform.position, attackPlayer.transform.position + viewAngleB * attackPlayer.attackRadius);
	}
}
