using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class ExitDoor2 : MonoBehaviour
	{
		[SerializeField] private Move leftDoor;
		[SerializeField] private Move rightDoor;
		[SerializeField] private Renderer terminalRenderer;
		[SerializeField] private GameObject terminalSparks;
		[SerializeField] private AudioSource openAS;
		[SerializeField] private bool working = true;
		private bool isOpen = false;

		private void Start()
		{
			SetWorkingState(working);
			if (working)
				Open();
		}

		public void SetWorkingState(bool _state)
		{
			working = _state;
			if (working)
				terminalRenderer.material.EnableKeyword(Constants.SHADER_EMISSION_TOGGLE);
			else
				terminalRenderer.material.DisableKeyword(Constants.SHADER_EMISSION_TOGGLE);
			terminalSparks.SetActive(!working);
			if (working != isOpen)
				OpenClose(working);
		}

		public void Open()
		{
			OpenClose(true);
		}

		public void Close()
		{
			OpenClose(false);
		}

		public void OpenClose(bool _state)
		{
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
			openAS.Play();
			isOpen = _state;
		}
	}
}