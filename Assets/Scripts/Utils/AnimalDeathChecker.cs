using MalbersAnimations;
using MalbersAnimations.Controller;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	[RequireComponent(typeof(MAnimal), typeof(Stats))]
	public class AnimalDeathChecker : MonoBehaviour
	{
		[SerializeField] private StateID deathState;
		[SerializeField] private StatID healthStat;

		private MAnimal animal;
		private Stats statsManager;

		private void Awake()
		{
			animal = GetComponent<MAnimal>();
			statsManager = GetComponent<Stats>();
		}

		private void Start()
		{
			InvokeRepeating(nameof(StatCheck), 0.5f, 0.5f);
		}

		private void StatCheck()
		{
			var state = animal.ActiveState;
			if (state.ID == deathState) return;
			if (statsManager.Stat_Get(healthStat).IsEmpty)
				ChangeStateToDead();
		}

		private void ChangeStateToDead()
		{
			animal.State_Activate(deathState);
			enabled = false;
		}
	}
}