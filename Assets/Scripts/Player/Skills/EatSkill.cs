using System.Collections;
using System.Collections.Generic;
using NPCs.Chicken;
using Player.Skills;
using UnityEngine;

public class EatSkill : MonoBehaviour, ISkill
{
	
	public static System.Action ChickenIsInRange;
		
	private bool _canEat = false;
	private Chicken _chicken = null;

	private void Start()
	{
		Chicken.OnChickenDead += Deactivate;
		Chicken.OnChickenIsNotInRange += Deactivate;
	}

	public void CanEatChicken(bool value, Chicken chicken)
	{
		_canEat = value;
		_chicken = chicken;
		
		if (ChickenIsInRange != null)
			ChickenIsInRange();
	}

	public void Eat()
	{
		if(_canEat)
			_chicken.Eaten();
	}

	public void Deactivate()
	{
		_canEat = false;
		_chicken = null;
	}

	public void ResetToRespawn(){}
}
