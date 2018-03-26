using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AttackPlayer), true)]

public class AttackRadiusEditor : UnityEditor.Editor
{
    private void OnSceneGUI()
    {
        AttackPlayer attackPlayer = (AttackPlayer)target;
        Handles.color = Color.red;
        Handles.DrawWireArc(attackPlayer.transform.position, Vector3.up, Vector3.forward, attackPlayer.AttackAngle, attackPlayer.AttackRadius);

        Vector3 viewAngleA = attackPlayer.transform.DirectionFromAngle(-attackPlayer.AttackAngle / 2, false);
        Vector3 viewAngleB = attackPlayer.transform.DirectionFromAngle(attackPlayer.AttackAngle / 2, false);

        Handles.DrawLine(attackPlayer.transform.position, attackPlayer.transform.position + (viewAngleB-viewAngleA*-1) * attackPlayer.AttackRadius/2);
        Handles.DrawLine(attackPlayer.transform.position, attackPlayer.transform.position + viewAngleA * attackPlayer.AttackRadius);
        Handles.DrawLine(attackPlayer.transform.position, attackPlayer.transform.position + viewAngleB * attackPlayer.AttackRadius);
    }
}