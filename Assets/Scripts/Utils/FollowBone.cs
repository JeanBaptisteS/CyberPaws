using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class FollowBone : MonoBehaviour
	{
		public enum OffsetType { Position, Rotation, Both }

		[SerializeField] private Transform bone;
		[SerializeField] private Vector3 positionOffset;
		[SerializeField] private Vector3 rotationOffset;

		[Button]
		private void ApplyCurrentOffset(OffsetType _type)
		{
			if (_type != OffsetType.Rotation)
				positionOffset = transform.position - bone.position;
			if (_type != OffsetType.Position)
				rotationOffset = Clamp180(transform.eulerAngles - bone.eulerAngles);
	    }
		[Button]
		private void ZeroOffsets(OffsetType _type)
		{
			if (_type != OffsetType.Rotation)
				positionOffset = Vector3.zero;
			if (_type != OffsetType.Position)
				rotationOffset = Vector3.zero;
		}

		private void FixedUpdate()
		{
			if (bone == null) return;
			transform.position = bone.position + bone.TransformDirection(positionOffset);
			

			transform.eulerAngles = bone.eulerAngles + rotationOffset;
		}

		private Vector3 Clamp180(Vector3 _angles)
		{
			Vector3 result = new Vector3(_angles.x, _angles.y, _angles.z);
			if (Mathf.Abs(result.x) > 180) result.x += 360 * -Mathf.Sign(result.x);
			if (Mathf.Abs(result.y) > 180) result.y += 360 * -Mathf.Sign(result.y);
			if (Mathf.Abs(result.z) > 180) result.z += 360 * -Mathf.Sign(result.z);
			return result;
		}
	}
}