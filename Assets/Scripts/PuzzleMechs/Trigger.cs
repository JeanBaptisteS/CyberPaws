using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using MalbersAnimations;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Trigger")]
	public class Trigger : MonoBehaviour
	{
		public enum EventType { FirstEnter, PerGameObject }
		
		[SerializeField] private LayerMask layerMask;
		[SerializeField] private EventType eventType = EventType.FirstEnter;

		[ShowIf(nameof(eventType), EventType.FirstEnter)]
		public UnityEvent<Collider> OnFirstEnter;
		[ShowIf(nameof(eventType), EventType.FirstEnter)]
		public UnityEvent<Collider> OnLastExit;
		[ShowIf(nameof(eventType), EventType.PerGameObject)]
		public UnityEvent<GameObject> OnGameObjectEnter;
		[ShowIf(nameof(eventType), EventType.PerGameObject)]
		public UnityEvent<GameObject> OnGameObjectExit;

		private List<Collider> collidersInTrigger = new List<Collider>();
		private List<GameObject> objectsInTrigger = new List<GameObject>();

		private bool CheckConditions(Collider _other)
		{
			if (!enabled) return false;
			if (_other == null) return false;
			if (!layerMask.ContainsLayer(_other.gameObject.layer)) return false;
			if (transform.SameHierarchy(_other.transform)) return false;

			return true;
		}

		private void OnTriggerEnter(Collider _other)
		{
			if (!CheckConditions(_other)) return;

			if (collidersInTrigger.Count == 0)
				OnFirstEnter?.Invoke(_other);

			var rootGO = FindRootGameObject(_other);
			if (!objectsInTrigger.Contains(rootGO))
			{
				OnGameObjectEnter?.Invoke(rootGO);
				objectsInTrigger.Add(rootGO);
			}

			if (!collidersInTrigger.Contains(_other))
				collidersInTrigger.Add(_other);
		}

		private void OnTriggerExit(Collider _other)
		{
			if (!CheckConditions(_other)) return;

			if (collidersInTrigger.Contains(_other))
				collidersInTrigger.Remove(_other);

			var rootGO = FindRootGameObject(_other);
			if (objectsInTrigger.Contains(rootGO))
			{
				OnGameObjectExit?.Invoke(rootGO);
				objectsInTrigger.Remove(rootGO);
			}

			if (collidersInTrigger.Count == 0)
				OnLastExit?.Invoke(_other);
		}

		private GameObject FindRootGameObject(Collider _collider)
		{
			var tr = _collider.GetComponentInParent<TriggerRoot>();
			if (tr == null) return _collider.transform.root.gameObject;
			return tr.gameObject;
		}
	}
}