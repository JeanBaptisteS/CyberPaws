using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;
using FMODUnity;

namespace PxlSpace.Fox
{
	public class DebugTest : MonoBehaviour
	{
#if UNITY_EDITOR

		[SerializeField] private StudioEventEmitter emitter;
		[SerializeField] private LayerMask GroundLayer;

		[Button]
		public void Test()
		{
			
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger) return;
			if (!CollidersLayer(other, GroundLayer)) return;
			emitter.Play();

		}
		public static bool CollidersLayer(Collider collider, LayerMask layerMask) => layerMask == (layerMask | (1 << collider.gameObject.layer));
#endif
	}
}