using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerSkillManager))]
[RequireComponent(typeof(PlayerMovement))]

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Singleton;
	public static event System.Action OnRespawn;
	public Vector3 initialPosition;	

	[HideInInspector]
	public Rigidbody playerRigidbody;

	[HideInInspector]
	public Animator playerAnimator;

	[HideInInspector]
	public PlayerMovement playerMovement;

	[HideInInspector]
	public PlayerSkillManager playerSkillManager;

	public bool isRespawning = false;

	private void Awake()
	{
		if (Singleton == null)
			Singleton = this;
		else if (Singleton != this)
			Destroy(this);		
	}

	private void Start()
	{
		playerRigidbody		= GetComponent<Rigidbody>();
		playerAnimator		= GetComponent<Animator>();
		playerMovement		= GetComponent<PlayerMovement>();
		playerSkillManager	= GetComponent<PlayerSkillManager>();
		initialPosition		= transform.position;
	}

	public void Respawn()
	{
		isRespawning = false;
	}
}
