using MalbersAnimations;
using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class Poison : MonoBehaviour
	{
		[SerializeField] private StateID deathState;
		private MRespawner respawner;

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(0.5f);
			respawner = FindObjectOfType<MRespawner>();
		}

		public void OnEnter(Collider _collider)
		{
			var root = _collider.GetComponentInParent<TriggerRoot>();
			var animal = root.gameObject.GetComponent<MAnimal>();
			if (animal != null)
			{
				animal.State_Activate(deathState);
			}
			else
			{
				if (respawner != null)
					respawner.Respawn();
					//this.Delay_Action(respawner.RespawnTime, () => respawner.ResetScene());
			}
		}
	}
}