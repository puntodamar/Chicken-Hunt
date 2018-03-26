using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerSkillManager))]
[RequireComponent(typeof(PlayerMovement))]

public class PlayerManager : MonoBehaviour
{
	public static PlayerManager Singleton;
	public static event System.Action OnRespawn;
	public Vector3 InitialPosition;	

	[HideInInspector]
	public Rigidbody PlayerRigidbody;

	[HideInInspector]
	public Animator PlayerAnimator;

	[HideInInspector]
	public PlayerMovement PlayerMovement;

	[HideInInspector]
	public PlayerSkillManager PlayerSkillManager;

	public bool IsRespawning = false;

	private void Awake()
	{
		if (Singleton == null)
			Singleton = this;
		else if (Singleton != this)
			Destroy(this);		
	}

	private void Start()
	{
		PlayerRigidbody		= GetComponent<Rigidbody>();
		PlayerAnimator		= GetComponent<Animator>();
		PlayerMovement		= GetComponent<PlayerMovement>();
		PlayerSkillManager	= GetComponent<PlayerSkillManager>();
		InitialPosition		= transform.position;
	}

	public void Respawn()
	{
		IsRespawning = false;
	}
}
