using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PxlSpace.Fox
{
	[RequireComponent(typeof(CinemachineFreeLook))]
	public class CinemachineMouseSensitivityAdjuster : MonoBehaviour
	{
		[ShowInInspector, PropertyRange(0f, 1f)]
		public static float CurrentXPercent = 0.1f;
		[ShowInInspector, PropertyRange(0f, 1f)]
		public static float CurrentYPercent = 0.66f;

		private CinemachineFreeLook cam;
		[SerializeField] private float minXSpeed;
		[SerializeField] private float maxXSpeed;
		[SerializeField] private float minYSpeed;
		[SerializeField] private float maxYSpeed;

		private void Awake()
		{
			cam = GetComponent<CinemachineFreeLook>();
		}

		public void SetSensitivity(float _percent)
		{
			cam.m_XAxis.m_MaxSpeed = Mathf.Lerp(minXSpeed, maxXSpeed, _percent);
			cam.m_YAxis.m_MaxSpeed = Mathf.Lerp(minYSpeed, maxYSpeed, _percent);
			CurrentXPercent = _percent;
			CurrentYPercent = _percent;
		}

		public void SetSensitivityX(float _percent)
		{
			CurrentXPercent = _percent;
			cam.m_XAxis.m_MaxSpeed = Mathf.Lerp(minXSpeed, maxXSpeed, CurrentXPercent);
		}

		public void SetSensitivityY(float _percent)
		{
			CurrentYPercent = _percent;
			cam.m_YAxis.m_MaxSpeed = Mathf.Lerp(minYSpeed, maxYSpeed, CurrentYPercent);
		}
	}
}