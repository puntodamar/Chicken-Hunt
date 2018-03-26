using System.Collections;
using System.Collections.Generic;
using Player.Skills;
using UnityEngine;

public enum SkillInUse { None, Jump, ThrowStone, Lure, Run }

[RequireComponent(typeof(RunSkill))]
[RequireComponent(typeof(JumpSkill))]
[RequireComponent(typeof(ThrowSkill))]
[RequireComponent(typeof(ThrowSkill))]
public class PlayerSkillManager : MonoBehaviour
{
	public static PlayerSkillManager Singleton;

	public bool infiniteSkill		= true;
	public SkillInUse skillInUse	= SkillInUse.None;
	public JumpSkill jumpSkill;
	public RunSkill runSkill;
	public ThrowSkill throwStoneSkill;
	public ThrowSkill throwLureSkill;
	public ISkill activatedSkill;

	private void Awake()
	{
		if (Singleton == null)
			Singleton = this;
		else if (Singleton != this)
			Destroy(this);
	}

	private void Update()
	{
		if (PlayerManager.Singleton.IsRespawning) return;

		if (Input.GetKeyDown(KeyCode.Q) && jumpSkill.IsFinished)
		{
			if (jumpSkill.RemainingUsage > 0 && (skillInUse == SkillInUse.None || skillInUse == SkillInUse.Run))
			{
				skillInUse = SkillInUse.Jump;
			}

			else if (skillInUse == SkillInUse.Jump)
			{
				skillInUse = SkillInUse.None;
				jumpSkill.Deactivate();
			}

			else
			{
				skillInUse = SkillInUse.None;
				jumpSkill.Deactivate();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if (throwStoneSkill.RemainingUsage > 0 && (skillInUse == SkillInUse.None || skillInUse == SkillInUse.Run))
			{
				//SwapSkill(jumpSkill);
				skillInUse = SkillInUse.ThrowStone;

			}
			else if (skillInUse == SkillInUse.ThrowStone)
			{
				skillInUse = SkillInUse.None;
				throwStoneSkill.Deactivate();
			}
			else
			{
				skillInUse = SkillInUse.None;
				throwStoneSkill.Deactivate();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if (throwLureSkill.RemainingUsage > 0 && (skillInUse == SkillInUse.None || skillInUse == SkillInUse.Run))
			{
				//SwapSkill(jumpSkill);
				skillInUse = SkillInUse.Lure;

			}
			else if (skillInUse == SkillInUse.Lure)
			{
				skillInUse = SkillInUse.None;
				throwLureSkill.Deactivate();
			}
			else
			{
				skillInUse = SkillInUse.None;
				throwLureSkill.Deactivate();
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			runSkill.Activate();
		}
	}

	void SwapSkill(ISkill skill = null)
	{
		if (skill != null)
		{
			if (activatedSkill != null)
			{
				activatedSkill.Deactivate();
				activatedSkill = skill;
			}
			else activatedSkill = skill;
		}
		else activatedSkill = null;
			
		
	}
}
