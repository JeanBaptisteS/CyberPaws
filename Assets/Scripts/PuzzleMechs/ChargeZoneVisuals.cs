using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class ChargeZoneVisuals : MonoBehaviour
	{
		[SerializeField] private MaterialSwitcher station;
		[SerializeField] private List<MaterialSwitcher> materialSwitchers;
		[SerializeField] private List<TransformAnimator> animators;
		[SerializeField] private ConnectionLine line;

		public void SetEnergyState(bool _state)
		{
			materialSwitchers.ForEach(ms => ms.SetState(_state));
			if (_state)
				animators.ForEach(a => a.PlayForward());
			else
				animators.ForEach(a => a.Pause());
			if (line.gameObject.activeSelf)
				line.SetState(_state);
		}
	}
}