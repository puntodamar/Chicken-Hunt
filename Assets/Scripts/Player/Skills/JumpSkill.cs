using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{
	public class JumpSkill : ThrowObject, ISkill
	{
		public int RemainingUsage		= 2;
		public bool InfiniteJump		= true;
		[HideInInspector]
		public bool IsFinished;
		
		private Text _remainingUsageText;
		private Image _iconImage;
		private PlayerMovement _playerMovement;
		private Animator _playerAnimator;

		protected override void Start()
		{
		
			base.Start();
			IsFinished				= true;
			_playerAnimator			= PlayerManager.Singleton.PlayerAnimator;
			_playerMovement			= GetComponent<PlayerMovement>();
			_remainingUsageText		= GameObject.Find("RemainingJump").GetComponent<Text>();
			_remainingUsageText.text = RemainingUsage.ToString();
			PlayerManager.OnRespawn += ResetToRespawn;
			_iconImage = GameObject.Find("JumpSkillIcon").GetComponent<Image>();
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
			if (PlayerSkillManager.Singleton.SkillInUse == SkillInUse.Jump)
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
			if (PlayerSkillManager.Singleton.SkillInUse == SkillInUse.Jump && IsBeingThrown)
			{
				if (collision.gameObject.CompareTag("Ground"))
				{
					Quaternion rotation		= transform.rotation;
					_playerAnimator.SetBool("jumping", false);

					TargetRigidbody.MoveRotation(rotation);
					IsBeingThrown			= false;				
					_playerMovement.Disabled = false;

					if (!PlayerSkillManager.Singleton.InfiniteSkill)
						RemainingUsage--;

					_remainingUsageText.text					= RemainingUsage.ToString();
					if (RemainingUsage == 0)
						_iconImage.color = Color.grey;

					IsFinished = true;
					PlayerSkillManager.Singleton.SkillInUse = SkillInUse.None;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (PlayerSkillManager.Singleton.SkillInUse == SkillInUse.Jump && IsBeingThrown)
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
