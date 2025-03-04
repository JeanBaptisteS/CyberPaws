using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PxlSpace.Fox
{
	public class InteractableBase : MonoBehaviour
	{
		public UnityEvent OnInteractEnter;
		public UnityEvent OnInteractExit;

		public virtual void InteractState(bool _state)
		{
			if (_state)
				InteractEnter();
			else
				InteractExit();
		}

		public virtual void InteractEnter()
		{
			OnInteractEnter?.Invoke();
		}
		public virtual void InteractExit()
		{
			OnInteractExit?.Invoke();
		}
	}
}