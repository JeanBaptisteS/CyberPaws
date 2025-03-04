using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PxlSpace.Fox
{
	[AddComponentMenu("Pxl Space/Load Scene")]
	public class LoadScene : MonoBehaviour
	{
		[SerializeField] private bool loadNextLevel;
		[SerializeField, HideIf(nameof(loadNextLevel))] private int targetSceneIndex;

		public void LoadTargetScene()
		{
			if (loadNextLevel)
				SceneLoader.Instance.LoadNextLevel();
			else
				SceneLoader.Instance.LoadScene(targetSceneIndex);
		}
	}
}