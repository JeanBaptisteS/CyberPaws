using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class PressurePlateVisuals : MonoBehaviour
	{
		private const string PRESS_ANIMATION = "PressurePlate_Press";
		private const string RELEASE_ANIMATION = "PressurePlate_Release";

		[SerializeField] private Animator anim;
		[SerializeField] private StudioEventEmitter sound;
		[SerializeField] private MaterialSwitcher plate;
		[SerializeField] private ConnectionLine line;

		private bool pressed = false;

		public void Press()
		{
			Animate(true);
		}

		public void Release()
		{
			Animate(false);
		}

		public void Animate(bool _state)
		{
			if (_state == pressed) return;
			anim.Play(_state ? PRESS_ANIMATION : RELEASE_ANIMATION);
			sound.Play();
			plate.SetState(_state);
			line.SetState(_state);
			pressed = _state;
		}
	}
}