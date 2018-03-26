using UnityEngine;

public class Stone : Distractor
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Guard"))
		{
			Collider[] agentsWithinDistractorRange = Physics.OverlapSphere(transform.position, Radius*2);

			foreach (Collider agent in agentsWithinDistractorRange)
			{
				Guard guard = agent.gameObject.GetComponent<Guard>();
				if (guard == null) continue;
				if (guard.canBeDistracted && (guard.status != GuardStatus.Pursuing && guard.status != GuardStatus.Distracted))
				{
					guard.DistractedTo(this.gameObject, DistractionTime);
				}
			}
		}
	}
}
