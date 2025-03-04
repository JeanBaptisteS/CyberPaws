using BattlePhaze.SettingsManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PxlSpace.Fox
{
	public class MouseSensitivitySettingModule : SettingsManagerOption
	{
		public CinemachineMouseSensitivityAdjuster mouseSensitivityAdjuster;

		private void Awake()
		{
			SceneLoader.OnSceneLoaded += SceneLoader_OnSceneLoaded;
		}

		private void OnDestroy()
		{
			SceneLoader.OnSceneLoaded -= SceneLoader_OnSceneLoaded;
		}

		private void SceneLoader_OnSceneLoaded()
		{
			mouseSensitivityAdjuster = FindObjectOfType<CinemachineMouseSensitivityAdjuster>();
		}

		public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager = null)
		{
			if (mouseSensitivityAdjuster == null) return;
			if (NameReturn(0, Option))
			{
				if (SliderReadOption(Option, Manager, out float value))
				{
					mouseSensitivityAdjuster.SetSensitivity(value);
				}
			}
		}
	}
}