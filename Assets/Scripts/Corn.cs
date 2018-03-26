using System.Collections;
using System.Collections.Generic;
using NPCs.Chicken;
using UnityEngine;

public class Corn : Distractor
{
	public int Health = 100;
	private new Rigidbody _rigidbody;
	private bool _isOnGround = false;
	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (_isOnGround)
		{
			Collider[] agentsWithinDistractorRange = Physics.OverlapSphere(transform.position, Radius, AgentsToDistract);

			foreach (Collider agent in agentsWithinDistractorRange)
			{
				Chicken chicken = agent.gameObject.GetComponent<Chicken>();
				if (chicken.ChickenState == ChickenState.Lured) continue;

				chicken.DistractedTo(this.gameObject);
			}

			if (_rigidbody.velocity != Vector3.zero)
				_rigidbody.velocity = Vector3.zero;
		}

		if (Health == 0)
			Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.CompareTag("Ground"))
		{
			_isOnGround = true;
		}
	}
}
