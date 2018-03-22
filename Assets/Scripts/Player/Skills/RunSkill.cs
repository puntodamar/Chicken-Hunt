using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RunSkill : MonoBehaviour, ISkill
{
	public int cooldown				= 30;
	public bool canRun				= true;
	public bool isRunning			= false;
	public int runBoostMultiplier	= 2;
	public int maximumRunTime		= 1;

	private Image runCooldownSlider;
	private float originalMoveSpeed;
	public PlayerMovement playerMovement;
	private Animator animator;

	void Start()
	{
		playerMovement			= GetComponent<PlayerMovement>();
		animator				= PlayerManager.Singleton.playerAnimator;		
		runCooldownSlider		= GameObject.Find("RunCooldownSlider").GetComponent<Image>();
		if (PlayerSkillManager.Singleton.infiniteSkill)
			cooldown = 2;
	}

	public void Activate()
	{
		if (canRun && !isRunning)
		{
			isRunning						= true;
			canRun							= false;
			originalMoveSpeed				= playerMovement.moveSpeed;
			playerMovement.moveSpeed		= playerMovement.moveSpeed * runBoostMultiplier;
			runCooldownSlider.fillAmount	= 0;

			animator.SetBool("running", true);
			StartCoroutine(Exhausted());
			StartCoroutine(CountdownCooldown());
		}
	}

	IEnumerator Exhausted()
	{
		yield return new WaitForSecondsRealtime(maximumRunTime);

		while(playerMovement.moveSpeed > originalMoveSpeed)
		{
			playerMovement.moveSpeed = Mathf.Lerp(playerMovement.moveSpeed, originalMoveSpeed, maximumRunTime);
			yield return new WaitForSeconds(.1f);
		}
		isRunning = false;
		animator.SetBool("running", false);

	}

	IEnumerator CountdownCooldown()
	{
		float currentCooldownTime = 0f;
		while(currentCooldownTime < cooldown)
		{
			currentCooldownTime += 1;
			runCooldownSlider.fillAmount = currentCooldownTime/cooldown;

			yield return new WaitForSecondsRealtime(1);
			
		}

		canRun = true;

	}

	public void Deactivate()
	{
		isRunning = false;
		animator.SetBool("running", false);
	}

	public void ResetToRespawn()
	{
		Deactivate();
	}
}
