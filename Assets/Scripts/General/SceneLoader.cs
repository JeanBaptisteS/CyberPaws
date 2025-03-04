using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PxlSpace.Fox
{
	public class SceneLoader : MonoBehaviour
	{
		public static SceneLoader Instance { get; private set; }
		public static int CurrentSceneIndex { get; private set; }
		public static event Action OnSceneLoaded;

		[SerializeField] private int fallBackSceneIndex;
		[SerializeField] private GameObject loadingScreen;
		[SerializeField] private Image progressBar;
		private float targetProgress;
		[SerializeField] private float progressLerpSpeed = 1f;
		[SerializeField] private UnityEvent _onSceneLoaded;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				DestroyImmediate(gameObject);
				return;
			}
			Instance = this;
			transform.parent = null;
			DontDestroyOnLoad(gameObject);
			CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
		}

		public void LoadNextLevel()
		{
			int nextBuildIndex = CurrentSceneIndex + 1;
			if (nextBuildIndex >= SceneManager.sceneCountInBuildSettings)
				nextBuildIndex = fallBackSceneIndex;
			LoadScene(nextBuildIndex);
		}

		public async void LoadScene(int _sceneIndex)
		{
			Debug.Log($"Starting to load scene {_sceneIndex}");
			var loadingTask = SceneManager.LoadSceneAsync(_sceneIndex);

			loadingScreen.SetActive(true);
			progressBar.fillAmount = 0f;
			targetProgress = 0f;

			do
			{
				targetProgress = loadingTask.progress;
				await Task.Yield();
				await Task.Delay(500);
			} while (!loadingTask.isDone);

			loadingScreen.SetActive(false);
			CurrentSceneIndex = _sceneIndex;
			Debug.Log($"Scene {CurrentSceneIndex} loaded.");
			OnSceneLoaded?.Invoke();
			_onSceneLoaded?.Invoke();
		}

		public void ReloadScene()
		{
			var scene = SceneManager.GetActiveScene();
			LoadScene(CurrentSceneIndex);
		}

		private void Update()
		{
			if (!loadingScreen.activeSelf) return;
			progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, targetProgress, progressLerpSpeed * Time.deltaTime);
		}
	}
}
