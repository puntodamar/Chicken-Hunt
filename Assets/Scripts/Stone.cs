using NPCs.Dog;
using UnityEngine;

public class Stone : Distractor
{
	private Rigidbody _rigidbody;
	private bool _isOnGround = false;


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Obstacle"))
		{

			Collider[] agentsWithinDistractorRange = Physics.OverlapSphere(transform.position, Radius*2,AgentsToDistract,QueryTriggerInteraction.Collide);
			
			foreach (Collider agent in agentsWithinDistractorRange)
			{

				GuardDog guard = agent.gameObject.GetComponent<GuardDog>();
				if (guard == null) continue;
				if (guard.CanBeDistracted && (guard.Status != GuardStatus.Pursuing && guard.Status != GuardStatus.Distracted))
				{

					guard.DistractedTo(this.gameObject, DistractionTime);
				}
			}
		}
	}
}
