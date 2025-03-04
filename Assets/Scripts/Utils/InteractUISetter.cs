using MalbersAnimations.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class InteractUISetter : MonoBehaviour
	{
		[SerializeField] private MEvent interactEvent;
		[SerializeField] private string message;
		[SerializeField] private bool withKey = true;
		[SerializeField] private Transform where;

		public void SpawnInteract(bool _state)
		{
			interactEvent.Invoke(_state);
			if (!_state) return;
			interactEvent.Invoke(where);
			interactEvent.Invoke(message);
			interactEvent.Invoke(withKey ? 1 : 0);
		}
	}
}