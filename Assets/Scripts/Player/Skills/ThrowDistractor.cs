using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{
	public class ThrowDistractor : ThrowObject, ISkill
	{
		public int RemainingUsage = 2;		
		public Image IconImage;
		
		private Text _remainingUsageText;
		//private Image _
		
		protected override void Start()
		{
			base.Start();
			PlayerManager.OnRespawn += ResetToRespawn;
			_remainingUsageText = GameObject.Find("RemainingStone").GetComponent<Text>();
			_remainingUsageText.text = RemainingUsage.ToString();
			IconImage = GameObject.Find("ThrowStoneIcon").GetComponent<Image>();
		}

		private void Update()
		{
			if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.ThrowStone)
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
					HasTarget				= true;
					IsBeingThrown			= true;
					LineRenderer.enabled	= false;
					BullsEye.SetActive(false);
					LaunchPosition			= transform.position + Vector3.up * 2;
					TargetRigidbody			= Instantiate(ObjectToThrow, LaunchPosition, Quaternion.identity).GetComponent<Rigidbody>();
					Throw();
					HasTarget				= false;
					if (!PlayerSkillManager.Singleton.infiniteSkill)
						RemainingUsage--;

					_remainingUsageText.text = RemainingUsage.ToString();

					if(RemainingUsage == 0)
						IconImage.color = Color.gray;

					PlayerSkillManager.Singleton.skillInUse = SkillInUse.None;
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
