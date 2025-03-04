using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Respawn Button")]
	public class RespawnButton : MonoBehaviour
	{
		public void Respawn()
		{
			if (MRespawner.instance == null) return;
			MRespawner.instance.Respawn(false);
		}
	}
}