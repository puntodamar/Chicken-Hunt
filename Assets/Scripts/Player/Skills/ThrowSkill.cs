﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowSkill : ThrowObject, ISkill
{
	public int remainingUsage = 2;
	public Text remainingUsageText;
	public Image iconImage;
	public SkillInUse skillInUse;
	public bool applyRandomRotation = false;

	protected override void Start()
	{
		base.Start();
		PlayerManager.OnRespawn += ResetToRespawn;
		remainingUsageText.text = remainingUsage.ToString();
	}

	private void Update()
	{
		if (PlayerSkillManager.Singleton.skillInUse == skillInUse)
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
				hasTarget = true;
				isBeingThrown = true;
				lineRenderer.enabled = false;
				bullsEye.SetActive(false);
				launchPosition = transform.position + Vector3.up * 2;
				targetRigidbody = Instantiate(objectToThrow, launchPosition, Quaternion.identity).GetComponent<Rigidbody>();

				if (applyRandomRotation)
					targetRigidbody.angularVelocity = Random.insideUnitSphere * 5;
					//ApplyRandomRotation(targetRigidbody);

				Throw();
				hasTarget = false;
				if (!PlayerSkillManager.Singleton.infiniteSkill)
					remainingUsage--;

				remainingUsageText.text = remainingUsage.ToString();

				if (remainingUsage == 0)
					iconImage.color = Color.gray;

				PlayerSkillManager.Singleton.skillInUse = SkillInUse.None;
				Deactivate();

			}
		}
	}

	public void Deactivate()
	{
		lineRenderer.enabled = false;
		bullsEye.SetActive(false);
		isBeingThrown = false;
	}

	public void ResetToRespawn()
	{
		Deactivate();
	}
}