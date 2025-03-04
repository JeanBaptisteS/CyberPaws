using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Energy Interactor")]
	public class EnergyInteractor : MonoBehaviour
	{
		public enum InteractionType { Consume, Gain, Both }

		[SerializeField] private bool interactWithFox = true;
		[SerializeField] private InteractionType interactionType = InteractionType.Both;
		[SerializeField] private bool chargeState = true;
		[ShowIf(nameof(interactionType), InteractionType.Gain)]
		[SerializeField] private bool infiniteCharge = false;
		private EnergyManager energyManager;

		[FoldoutGroup("Events"), SerializeField] private UnityEvent<bool> OnSetChargeState;
		[FoldoutGroup("Events"), SerializeField] private UnityEvent OnInteractionSuccess;
		[FoldoutGroup("Events"), SerializeField] private UnityEvent OnInteractionFailed;

		private void Start()
		{
			if (interactWithFox)
				energyManager = MalbersAnimations.Controller.MAnimal.MainAnimal.GetComponent<EnergyManager>();
			OnSetChargeState?.Invoke(chargeState);
		}

		internal void SetChargeState(bool _state)
		{
			chargeState = _state;
		}

		public void TryInteract(Collider _other)
		{
			EnergyManager em = _other.GetComponentInParent<EnergyManager>();
			if (em == null) return;
			energyManager = em;
			TryInteract();
		}

		public void TryInteract()
		{
			if (energyManager == null) return;
			if (InteractionFail())
			{
				OnInteractionFailed?.Invoke();
				return;
			}
			bool success = false;
			switch (interactionType)
			{
				case InteractionType.Consume:
					success = energyManager.ConsumeEnergy();
					if (success)
						chargeState = true;
					break;
				case InteractionType.Gain:
					success = energyManager.GainEnergy();
					if (success)
						chargeState = infiniteCharge;
					break;
				case InteractionType.Both:
					success = energyManager.HasEnergy ? energyManager.ConsumeEnergy() : energyManager.GainEnergy();
					if (success)
						chargeState = !energyManager.HasEnergy;
					break;
				default:
					break;
			}
			if (success)
			{
				OnInteractionSuccess?.Invoke();
				OnSetChargeState?.Invoke(chargeState);
			}
		}

		private bool InteractionFail()
		{
			switch (interactionType)
			{
				case InteractionType.Consume:
					if (chargeState) return true;
					return !energyManager.HasEnergy;
				case InteractionType.Gain:
					if (!chargeState) return true;
					return energyManager.HasEnergy;
				case InteractionType.Both:
					return energyManager.HasEnergy == chargeState;
				default:
					return true;
			}
		}
	}
}