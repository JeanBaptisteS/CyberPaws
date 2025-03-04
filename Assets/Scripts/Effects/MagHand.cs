using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class MagHand : MonoBehaviour
	{
		private const string GRAB_ANIM_PARAM = "Grabbing";

		[SerializeField] private Animator anim;
		[SerializeField] private ParticleSystem particles;

		public void Open()
		{
			anim.SetBool(GRAB_ANIM_PARAM, true);
			particles.Play();
		}

		public void Close()
		{
			anim.SetBool(GRAB_ANIM_PARAM, false);
			particles.Stop();
		}
	}
}