using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class LaserBox : MonoBehaviour
	{
		[SerializeField] private List<Laser> lasers;
		[SerializeField] private MaterialSwitcher box;
		//[SerializeField] private Renderer box;
		//[SerializeField] private Material onMat;
		//[SerializeField] private Material offMat;
		[OnValueChanged(nameof(SetWorking))]
		[SerializeField] private bool working = true;

		private void Start()
		{
			SetWorking(working);
		}

		public void SetWorkingInverted(bool _state)
		{
			SetWorking(!_state);
		}

		public void SetWorking(bool _state)
		{
			working = _state;
			lasers.ForEach(l => l.SetWorking(working));
			box.SetState(working);
			//box.material = working ? onMat : offMat;
		}
	}
}