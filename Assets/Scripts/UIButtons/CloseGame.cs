using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Close Game")]
	public class CloseGame : MonoBehaviour
	{
		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
		}
	}
}