using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Energy Manager")]
	public class EnergyManager : MonoBehaviour
	{
		public enum VisualChangeType { GameObject, Material }
		public bool HasEnergy => hasEnergy;
		[SerializeField] private bool hasEnergy = false;

		[FoldoutGroup("Visuals")]
		[SerializeField] private VisualChangeType visualChangeType;
		[FoldoutGroup("Visuals"), ShowIf(nameof(visualChangeType), VisualChangeType.GameObject)]
		[SerializeField] private GameObject visualGO;
		[FoldoutGroup("Visuals"), ShowIf(nameof(visualChangeType), VisualChangeType.Material)]
		[SerializeField] private Material visualMatOff;
		[FoldoutGroup("Visuals"), ShowIf(nameof(visualChangeType), VisualChangeType.Material)]
		[SerializeField] private Material visualMatOn;
		[FoldoutGroup("Visuals"), ShowIf(nameof(visualChangeType), VisualChangeType.Material)]
		[SerializeField] private Renderer visualRenderer;

		[FoldoutGroup("Events")]
		public UnityEvent OnEnergyConsumed;
		[FoldoutGroup("Events")]
		public UnityEvent OnEnergyGained;

		private void Start()
		{
			SetEnergyState(hasEnergy);
		}

		[Button(DrawResult = false)]
		public bool GainEnergy()
		{
			bool success = !HasEnergy;
			if (success)
				OnEnergyGained?.Invoke();
			SetEnergyState(true);
			return success;
		}

		[Button(DrawResult = false)]
		public bool ConsumeEnergy()
		{
			bool success = HasEnergy;
			if (success)
				OnEnergyConsumed?.Invoke();
			SetEnergyState(false);
			return success;
		}

		private void SetEnergyState(bool _state)
		{
			hasEnergy = _state;
			switch (visualChangeType)
			{
				case VisualChangeType.GameObject:
					visualGO.SetActive(HasEnergy);
					break;
				case VisualChangeType.Material:
					visualRenderer.material = HasEnergy ? visualMatOn : visualMatOff;
					break;
				default:
					break;
			}
		}
	}
}