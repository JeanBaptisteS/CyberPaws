using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class ConnectionLine : MonoBehaviour
	{
		[SerializeField] private Gradient onLineColor;
		[SerializeField] private Gradient offLineColor;

		private LineRenderer[] lines;

		private void OnEnable()
		{
			lines = GetComponentsInChildren<LineRenderer>(true);
		}

		public void SetState(bool _state)
		{
			Gradient lineGradient = _state ? onLineColor : offLineColor;
			for (int i = 0; i < lines.Length; i++)
			{
				lines[i].colorGradient = lineGradient;
			}
		}
	}
}