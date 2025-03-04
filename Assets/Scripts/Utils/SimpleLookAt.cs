using UnityEngine;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Simple Look At")]
	public class SimpleLookAt : MonoBehaviour
	{
		[SerializeField] private Transform pivot;
		[SerializeField] private bool lookAtFox = true;
		[SerializeField, Range(0f, 1f)] private float lerp = 1f;
		[SerializeField] private Transform target;

		private void Start()
		{
			if (lookAtFox)
				target = MalbersAnimations.Controller.MAnimal.MainAnimal.transform;
		}

		public void SetTarget(Transform _t)
		{
			target = _t;
		}

		public void ResetLook()
		{
			pivot.rotation = Quaternion.identity;
		}

		private void Update()
		{
			if (target == null) return;

			Quaternion rot = Quaternion.LookRotation((target.position - pivot.position).normalized);
			pivot.rotation = Quaternion.Slerp(pivot.rotation, rot, lerp);
		}
	}
}