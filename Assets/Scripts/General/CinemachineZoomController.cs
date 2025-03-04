using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class CinemachineZoomController : MonoBehaviour
	{
		[SerializeField] private Cinemachine.CinemachineFreeLook camController;
		[SerializeField, Range(0f, 1f)] private float zoomPercent;
		[SerializeField] private Vector2 zoomLimits;
		[SerializeField] private float zoomSpeed = 1f;

#if UNITY_EDITOR
		private void OnValidate()
		{
			SetZoomPercent(zoomPercent);
		}
#endif

		public void Zoom(int direction)
		{
			SetZoomPercent(Mathf.Clamp01(zoomPercent + zoomSpeed * direction * Time.deltaTime));
		}

		public void SetZoomPercent(float percent)
		{
			zoomPercent = percent;
			float zoom = Mathf.Lerp(zoomLimits.x, zoomLimits.y, zoomPercent);
			for (int i = 0; i < camController.m_Orbits.Length; i++)
			{
				camController.m_Orbits[i].m_Radius = zoom;
			}
		}
	}
}