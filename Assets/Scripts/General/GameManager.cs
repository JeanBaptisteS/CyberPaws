using BattlePhaze.SettingsManager;
using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations;
using UnityEngine.InputSystem;

namespace PxlSpace.Fox
{
	public class GameManager : MonoBehaviour
	{
		[Header("Pause")]
		[SerializeField] private GameObject pauseMenu;
		[SerializeField] private MInputLink playerInput;
		[SerializeField] private Cinemachine.CinemachineInputProvider lookInput;
		[SerializeField] private CinemachineZoomController zoomController;
		private bool _paused;

		private void Start()
		{
			SetPause(false);
			//MRespawner.instance.FindMainAnimal();
		}

		public void OnPause()
		{
			SetPause(!_paused);
		}

		public void TogglePause(bool _state)
		{
			SetPause(_state);
		}

		internal void SetPause(bool _state, bool _toggleMenu = true)
		{
			_paused = _state;
			if (_toggleMenu)
				pauseMenu.SetActive(_paused);
			OpenCloseSettings(false);
			Cursor.visible = _paused;
			Cursor.lockState = _paused ? CursorLockMode.None : CursorLockMode.Locked;
			playerInput.Enable(!_paused);
			lookInput.SetEnable(!_paused);
		}

		public void OpenCloseSettings(bool _state)
		{
			SettingsManager.Instance.OpenCloseSettings(_state);
		}

		public void OnZoom(InputValue input)
		{
			float zoom = input.Get<float>();
			if (zoom == 0) return;
			int direction = -(int)Mathf.Sign(zoom);
			zoomController.Zoom(direction);
		}
	}
}
