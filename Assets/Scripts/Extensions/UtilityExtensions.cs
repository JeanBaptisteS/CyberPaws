using System.Collections;
using UnityEngine;

namespace PxlSpace.Fox
{
	public static class UtilityExtensions
	{
		public static bool ContainsLayer(this LayerMask _layerMask, int _layer) => _layerMask == (_layerMask | (1 << _layer));

		public static Transform SetParentScaleFixer(this Transform transform, Transform parent, Vector3 Position)
		{
			if (parent.lossyScale.x == parent.lossyScale.y && parent.lossyScale.x == parent.lossyScale.z)
			{
				transform.SetParent(parent, true);
				transform.position = Position;
				return null;
			}

			Vector3 NewScale = parent.transform.lossyScale;
			NewScale.x = 1f / Mathf.Max(NewScale.x, Constants.Epsilon);
			NewScale.y = 1f / Mathf.Max(NewScale.y, Constants.Epsilon);
			NewScale.z = 1f / Mathf.Max(NewScale.z, Constants.Epsilon);

			GameObject Hlper = new GameObject { name = transform.name + "Link" };

			Hlper.transform.SetParent(parent);
			Hlper.transform.localScale = NewScale;
			Hlper.transform.position = Position;
			Hlper.transform.localRotation = Quaternion.identity;

			transform.SetParent(Hlper.transform);
			transform.localPosition = Vector3.zero;
			return Hlper.transform;
		}
	}
}