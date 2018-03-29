using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Skills
{
	public class RunSkill : MonoBehaviour, ISkill
	{
		public int Cooldown				= 30;
		public bool CanRun				= true;
		public bool IsRunning			= false;
		public int RunBoostMultiplier	= 2;
		public int MaximumRunTime		= 1;
		public PlayerMovement PlayerMovement;
		
		private Image _runCooldownSlider;
		private float _originalMoveSpeed;
		
		private Animator _animator;

		private void Start()
		{
			PlayerMovement			= GetComponent<PlayerMovement>();
			//_animator				= PlayerManager.Singleton.PlayerAnimator;	
			_animator = GetComponent<Animator>();
			_runCooldownSlider		= GameObject.Find("RunCooldownSlider").GetComponent<Image>();
			if (PlayerSkillManager.Singleton.InfiniteSkill)
				Cooldown = 2;
		}

		public void Activate()
		{
			if (!CanRun || IsRunning) return;
			
			IsRunning						= true;
			CanRun							= false;
			_originalMoveSpeed				= PlayerMovement.MoveSpeed;
			PlayerMovement.MoveSpeed		= PlayerMovement.MoveSpeed * RunBoostMultiplier;
			_runCooldownSlider.fillAmount	= 0;

			_animator.SetBool("running", true);
			StartCoroutine(Exhausted());
			StartCoroutine(CountdownCooldown());
		}

		IEnumerator Exhausted()
		{
			yield return new WaitForSecondsRealtime(MaximumRunTime);

			while(PlayerMovement.MoveSpeed > _originalMoveSpeed)
			{
				PlayerMovement.MoveSpeed = Mathf.Lerp(PlayerMovement.MoveSpeed, _originalMoveSpeed, MaximumRunTime);
				yield return new WaitForSeconds(.1f);
			}
			IsRunning = false;
			_animator.SetBool("running", false);

		}

		private IEnumerator CountdownCooldown()
		{
			float currentCooldownTime = 0f;
			while(currentCooldownTime < Cooldown)
			{
				currentCooldownTime += 1;
				_runCooldownSlider.fillAmount = currentCooldownTime/Cooldown;

				yield return new WaitForSecondsRealtime(1);
			
			}

			CanRun = true;

		}

		public void Deactivate()
		{
			IsRunning = false;
			_animator.SetBool("running", false);
		}

		public void ResetToRespawn()
		{
			Deactivate();
		}
	}
}
