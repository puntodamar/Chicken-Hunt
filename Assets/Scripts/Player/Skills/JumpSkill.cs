﻿using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{
	public class JumpSkill : ThrowObject, ISkill
	{
		public int RemainingUsage		= 2;
		public bool InfiniteJump		= true;

		public Text RemainingUsageText;
		public Image IconImage;
		[HideInInspector]
		public bool IsFinished;
		private PlayerMovement _playerMovement;
		private Animator _playerAnimator;

		protected override void Start()
		{
		
			base.Start();
			IsFinished				= true;
			_playerAnimator			= PlayerManager.Singleton.PlayerAnimator;
			_playerMovement			= GetComponent<PlayerMovement>();
			//remainingUsageText		= GameObject.Find("RemainingJump").GetComponent<Text>();
			RemainingUsageText.text = RemainingUsage.ToString();
			PlayerManager.OnRespawn += ResetToRespawn;
		}

		public void Deactivate()
		{
			IsFinished				= false;
			LineRenderer.enabled	= false;
			BullsEye.SetActive(false);
			IsBeingThrown			= false;
		}

		void Update()
		{
			if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.Jump)
			{
			
				if (!IsBeingThrown)
				{
					if (!BullsEye.activeInHierarchy)
						BullsEye.SetActive(true);
					if (!LineRenderer.enabled)
						LineRenderer.enabled = true;
				}


				LaunchPosition = transform.position;

				if (!TargetAvailable())
					SetTargetLocation();

				if (Input.GetMouseButtonUp(0) && !HasTarget && !IsBeingThrown)
				{
					BullsEye.SetActive(false);
					HasTarget				= true;
					LineRenderer.enabled	= false;				
					IsBeingThrown			= true;
					IsFinished				= false;
					_playerMovement.Disabled = true;

					_playerAnimator.SetBool("jumping", true);
					transform.LookAt(Target);
				

				}
			}

		}

		private void OnCollisionEnter(Collision collision)
		{
			if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.Jump && IsBeingThrown)
			{
				if (collision.gameObject.CompareTag("Ground"))
				{
					Quaternion rotation		= transform.rotation;
					_playerAnimator.SetBool("jumping", false);

					TargetRigidbody.MoveRotation(rotation);
					IsBeingThrown			= false;				
					_playerMovement.Disabled = false;

					if (!PlayerSkillManager.Singleton.infiniteSkill)
						RemainingUsage--;

					RemainingUsageText.text					= RemainingUsage.ToString();
					if (RemainingUsage == 0)
						IconImage.color = Color.grey;

					IsFinished = true;
					PlayerSkillManager.Singleton.skillInUse = SkillInUse.None;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.Jump && IsBeingThrown)
			{
				if (other.gameObject.CompareTag("Chicken"))
				{
					Destroy(other.gameObject);
				}
			}
		}

		public void ResetToRespawn()
		{
			Deactivate();
			TargetRigidbody.velocity = Vector3.zero;
		}
	}
}
