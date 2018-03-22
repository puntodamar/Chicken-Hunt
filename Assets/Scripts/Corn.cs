using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corn : Distractor
{
	public int health = 100;
	private new Rigidbody rigidbody;
	private bool isOnGround = false;
	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (isOnGround)
		{
			Collider[] agentsWithinDistractorRange = Physics.OverlapSphere(transform.position, radius, agentsToDistract);

			foreach (Collider agent in agentsWithinDistractorRange)
			{
				Chicken chicken = agent.gameObject.GetComponent<Chicken>();
				if (chicken.status == ChickenStatus.Lured) continue;

				chicken.DistractedTo(this.gameObject);
			}

			if (rigidbody.velocity != Vector3.zero)
				rigidbody.velocity = Vector3.zero;
		}

		if (health == 0)
			Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Ground")
		{
			isOnGround = true;
		}
	}
}
