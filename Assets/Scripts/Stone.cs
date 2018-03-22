using UnityEngine;

public class Stone : Distractor
{
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Guard")
		{
			Collider[] agentsWithinDistractorRange = Physics.OverlapSphere(transform.position, radius*2);

			foreach (Collider agent in agentsWithinDistractorRange)
			{
				Guard guard = agent.gameObject.GetComponent<Guard>();
				if (guard == null) continue;
				if (guard.canBeDistracted && (guard.status != GuardStatus.Pursuing && guard.status != GuardStatus.Distracted))
				{
					guard.DistractedTo(this.gameObject, distractionTime);
				}
			}
		}
	}
}
