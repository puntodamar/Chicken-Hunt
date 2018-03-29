using System.Collections;
using System.Collections.Generic;
using Managers;
using NPCs.Chicken;
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

	public bool InfiniteSkill		= true;
	public SkillInUse SkillInUse	= SkillInUse.None;
	public JumpSkill JumpSkill;
	public RunSkill RunSkill;
	public ThrowSkill ThrowStoneSkill;
	public ThrowSkill ThrowLureSkill;
	public ISkill ActivatedSkill;
	public EatSkill EatSkill;

	public float EatRate = 1f;
	private bool _canEatChicken = false;
	private float _timeToEat = 0;
	
	private void Awake()
	{
		if (Singleton == null)
			Singleton = this;
		else if (Singleton != this)
			Destroy(this);

		Health.OnPlayerDied += OnPlayerDied;
		GameManager.OnGameOver += Disable;
		EatSkill.ChickenIsInRange += OnChickenIsInRange;
		Chicken.OnChickenDead += OnChickenDead;
		Chicken.OnChickenIsNotInRange += OnChickenDead;
	}

	private void Update()
	{
		if (PlayerManager.Singleton.IsRespawning) return;

		if (Input.GetKeyDown(KeyCode.Q) && JumpSkill.IsFinished)
		{
			if (JumpSkill.RemainingUsage > 0 && (SkillInUse == SkillInUse.None || SkillInUse == SkillInUse.Run))
			{
				SkillInUse = SkillInUse.Jump;
			}

			else if (SkillInUse == SkillInUse.Jump)
			{
				SkillInUse = SkillInUse.None;
				JumpSkill.Deactivate();
			}

			else
			{
				SkillInUse = SkillInUse.None;
				JumpSkill.Deactivate();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			if (ThrowStoneSkill.RemainingUsage > 0 && (SkillInUse == SkillInUse.None || SkillInUse == SkillInUse.Run))
			{
				//SwapSkill(jumpSkill);
				SkillInUse = SkillInUse.ThrowStone;

			}
			else if (SkillInUse == SkillInUse.ThrowStone)
			{
				SkillInUse = SkillInUse.None;
				ThrowStoneSkill.Deactivate();
			}
			else
			{
				SkillInUse = SkillInUse.None;
				ThrowStoneSkill.Deactivate();
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			if (ThrowLureSkill.RemainingUsage > 0 && (SkillInUse == SkillInUse.None || SkillInUse == SkillInUse.Run))
			{
				//SwapSkill(jumpSkill);
				SkillInUse = SkillInUse.Lure;

			}
			else if (SkillInUse == SkillInUse.Lure)
			{
				SkillInUse = SkillInUse.None;
				ThrowLureSkill.Deactivate();
			}
			else
			{
				SkillInUse = SkillInUse.None;
				ThrowLureSkill.Deactivate();
			}
		}
		else if (Input.GetKey(KeyCode.Space))
		{
			if (_canEatChicken)
			{
				_timeToEat += Time.deltaTime;
				if (_timeToEat >= EatRate)
				{
					_timeToEat = 0;
					EatSkill.Eat();
				}
				
			}
				
		}
		
		else if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			RunSkill.Activate();
		}
		
		
	}

	void SwapSkill(ISkill skill = null)
	{
		if (skill != null)
		{
			if (ActivatedSkill != null)
			{
				ActivatedSkill.Deactivate();
				ActivatedSkill = skill;
			}
			else ActivatedSkill = skill;
		}
		else ActivatedSkill = null;
			
		
	}

	void OnPlayerDied()
	{
		SkillInUse = SkillInUse.None;
		RunSkill.Deactivate();
		JumpSkill.Deactivate();
		ThrowLureSkill.Deactivate();
		ThrowStoneSkill.Deactivate();
	}
	
	void Disable()
	{
		this.enabled = false;
		OnPlayerDied();
	}

	void OnChickenIsInRange()
	{
		_canEatChicken = true;
	}

	void OnChickenDead()
	{
		_canEatChicken = false;
	}
}
