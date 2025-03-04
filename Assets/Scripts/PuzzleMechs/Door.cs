using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class Door : MonoBehaviour
	{
		[SerializeField] private bool startOpen = true;
		[SerializeField] private Move leftDoor;
		[SerializeField] private Move rightDoor;
		[SerializeField] private StudioEventEmitter soundEmitter;
		[SerializeField] private List<DoorTerminal> terminals;

		private bool isOpen = false;

		private void Start()
		{
			SetOpening(startOpen);
		}

		public void Open()
		{
			SetOpening(true);
		}

		public void Close()
		{
			SetOpening(false);
		}

		public void SetOpening(bool _state)
		{
			SetVisuals(_state);
			if (_state == isOpen) return;
			if (_state)
			{
				leftDoor.PlayForward();
				rightDoor.PlayForward();
			}
			else
			{
				leftDoor.PlayReverse();
				rightDoor.PlayReverse();
			}
			soundEmitter.Play();
			isOpen = _state;
		}

		private void SetVisuals(bool _state)
		{
			terminals.ForEach(t => t.SetVisuals(_state));
		}

		[System.Serializable]
		public class DoorTerminal
		{
			[SerializeField] private Renderer rend;
			[SerializeField] private GameObject sparks;

			public void SetVisuals(bool _state)
			{
				if (_state)
					rend.material.EnableKeyword(Constants.SHADER_EMISSION_TOGGLE);
				else
					rend.material.DisableKeyword(Constants.SHADER_EMISSION_TOGGLE);
				sparks.SetActive(!_state);
			}
		}
	}
}