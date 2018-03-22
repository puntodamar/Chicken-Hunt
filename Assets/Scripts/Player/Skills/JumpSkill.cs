using UnityEngine;
using UnityEngine.UI;

public class JumpSkill : ThrowObject, ISkill
{
	public int remainingUsage		= 2;
	public bool infiniteJump		= true;

	public Text remainingUsageText;
	public Image iconImage;
	[HideInInspector]
	public bool isFinished;
	private PlayerMovement playerMovement;
	private Animator playerAnimator;

	protected override void Start()
	{
		
		base.Start();
		isFinished				= true;
		playerAnimator			= PlayerManager.Singleton.playerAnimator;
		playerMovement			= GetComponent<PlayerMovement>();
		//remainingUsageText		= GameObject.Find("RemainingJump").GetComponent<Text>();
		remainingUsageText.text = remainingUsage.ToString();
		PlayerManager.OnRespawn += ResetToRespawn;
	}

	public void Deactivate()
	{
		isFinished				= false;
		lineRenderer.enabled	= false;
		bullsEye.SetActive(false);
		isBeingThrown			= false;
	}

	void Update()
	{
		if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.Jump)
		{
			
			if (!isBeingThrown)
			{
				if (!bullsEye.activeInHierarchy)
					bullsEye.SetActive(true);
				if (!lineRenderer.enabled)
					lineRenderer.enabled = true;
			}


			launchPosition = transform.position;

			if (!TargetAvailable())
				SetTargetLocation();

			if (Input.GetMouseButtonUp(0) && !hasTarget && !isBeingThrown)
			{
				bullsEye.SetActive(false);
				hasTarget				= true;
				lineRenderer.enabled	= false;				
				isBeingThrown			= true;
				isFinished				= false;
				playerMovement.disabled = true;

				playerAnimator.SetBool("jumping", true);
				transform.LookAt(target);
				

			}
		}

	}

	private void OnCollisionEnter(Collision collision)
	{
		if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.Jump && isBeingThrown)
		{
			if (collision.gameObject.tag == "Ground")
			{
				Quaternion rotation		= transform.rotation;
				playerAnimator.SetBool("jumping", false);

				targetRigidbody.MoveRotation(rotation);
				isBeingThrown			= false;				
				playerMovement.disabled = false;

				if (!PlayerSkillManager.Singleton.infiniteSkill)
					remainingUsage--;

				remainingUsageText.text					= remainingUsage.ToString();
				if (remainingUsage == 0)
					iconImage.color = Color.grey;

				isFinished = true;
				PlayerSkillManager.Singleton.skillInUse = SkillInUse.None;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (PlayerSkillManager.Singleton.skillInUse == SkillInUse.Jump && isBeingThrown)
		{
			if (other.gameObject.tag == "Chicken")
			{
				Destroy(other.gameObject);
			}
		}
	}

	public void ResetToRespawn()
	{
		Deactivate();
		targetRigidbody.velocity = Vector3.zero;
	}
}
