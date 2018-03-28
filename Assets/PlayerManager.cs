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
	public static System.Action OnRespawn;
	[HideInInspector]
	public Vector3 InitialPosition;

	[HideInInspector]
	public Rigidbody PlayerRigidbody;

	//[HideInInspector]
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

		Health.OnPlayerDied += Respawn;
	}

	public void Respawn()
	{
		IsRespawning = true;
		transform.position = InitialPosition;
		StartCoroutine(Blink());

	}

	IEnumerator Blink()
	{
		int blink = 5;
		SkinnedMeshRenderer renderer = GetComponentInChildren<SkinnedMeshRenderer>();
		while (blink < 5)
		{
			renderer.enabled = false;
			yield return new WaitForSecondsRealtime(.5f);
			renderer.enabled = true;
			blink++;
		}

		IsRespawning = false;
		if (OnRespawn != null)
			OnRespawn();
	}
}
