using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{
	public class ThrowSkill : ThrowObject, ISkill
	{
		public int RemainingUsage = 2;
		public Text RemainingUsageText;
		public Image IconImage;
		public SkillInUse SkillInUse;
		public bool ApplyRandomRotation = false;

		protected override void Start()
		{
			base.Start();
			PlayerManager.OnRespawn += ResetToRespawn;
			RemainingUsageText.text = RemainingUsage.ToString();
		}

		private void Update()
		{
			if (PlayerSkillManager.Singleton.SkillInUse == SkillInUse)
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
					HasTarget = true;
					IsBeingThrown = true;
					LineRenderer.enabled = false;
					BullsEye.SetActive(false);
					LaunchPosition = transform.position + Vector3.up * 2;
					TargetRigidbody = Instantiate(ObjectToThrow, LaunchPosition, Quaternion.identity).GetComponent<Rigidbody>();

					if (ApplyRandomRotation)
						TargetRigidbody.angularVelocity = Random.insideUnitSphere * 5;
					//ApplyRandomRotation(targetRigidbody);

					Throw();
					HasTarget = false;
					if (!PlayerSkillManager.Singleton.InfiniteSkill)
						RemainingUsage--;

					RemainingUsageText.text = RemainingUsage.ToString();

					if (RemainingUsage == 0)
						IconImage.color = Color.gray;

					PlayerSkillManager.Singleton.SkillInUse = SkillInUse.None;
					Deactivate();

				}
			}
		}

		public void Deactivate()
		{
			LineRenderer.enabled = false;
			BullsEye.SetActive(false);
			IsBeingThrown = false;
		}

		public void ResetToRespawn()
		{
			Deactivate();
		}
	}
}
