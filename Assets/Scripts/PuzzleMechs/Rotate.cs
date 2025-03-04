using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Rotate")]
	public class Rotate : TransformAnimator
	{
		[FoldoutGroup("Bounds")]
		[SerializeField] private Vector3 axis = Vector3.up;
		[FoldoutGroup("Bounds")]
		[SerializeField] private float startAngle = 0f;
		[FoldoutGroup("Bounds")]
		[SerializeField] private float endAngle = 360f;


		public override void Evaluate(float _t)
		{
			var curvePosition = curve.Evaluate(_t);
			var q = Quaternion.AngleAxis(Mathf.LerpUnclamped(startAngle, endAngle, curvePosition), axis);
			objectToMove.localRotation = q;
		}
	}
}