using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Move")]
	public class Move : TransformAnimator
	{
		[FoldoutGroup("Bounds")]
		public Vector3 start = Vector3.zero;
		[FoldoutGroup("Bounds")] 
		public Vector3 end = Vector3.forward;

		public override void Evaluate(float _t)
		{
			float curvedValue = curve.Evaluate(_t);
			Vector3 pos = transform.TransformPoint(Vector3.Lerp(start, end, curvedValue));
			objectToMove.position = pos;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(Move), true)]
	public class MoveEditor : OdinEditor
	{
		void OnSceneGUI()
		{
			var t = target as Move;
			var start = t.transform.TransformPoint(t.start);
			var end = t.transform.TransformPoint(t.end);


			using (var cc = new EditorGUI.ChangeCheckScope())
			{
				start = Handles.PositionHandle(start, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);
				Handles.color = Color.yellow;
				Handles.SphereHandleCap(0, start, t.transform.rotation, 0.1f * t.transform.lossyScale.y, EventType.Repaint);
				Handles.SphereHandleCap(0, end, t.transform.rotation, 0.1f * t.transform.lossyScale.y, EventType.Repaint);

				end = Handles.PositionHandle(end, Quaternion.AngleAxis(180, t.transform.up) * t.transform.rotation);

				if (cc.changed)
				{
					Undo.RecordObject(t, "Move Handles");
					t.start = t.transform.InverseTransformPoint(start);
					t.end = t.transform.InverseTransformPoint(end);
					t.Evaluate(t.previewPosition);
				}
			}
			Handles.DrawDottedLine(start, end, 5);
			Handles.Label(Vector3.Lerp(start, end, 0.5f), "Distance:" + (end - start).magnitude.ToString("F2"));
		}
	}
#endif
}