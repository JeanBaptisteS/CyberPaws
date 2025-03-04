using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class LevelExit : MonoBehaviour
	{
		public void OnTriggerExitDoor(Collider _other)
		{
			var animal = _other.GetComponentInParent<MAnimal>();
			if (!animal) return;
			if (animal != MAnimal.MainAnimal) return;

			ExitReached();
		}

		[Sirenix.OdinInspector.Button]
		public void ExitReached()
		{
			SceneLoader.Instance.LoadNextLevel();
		}
	}
}