using BattlePhaze.SettingsManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingSettingModule : SettingsManagerOption
{
	[SerializeField] private Volume ppVolume;

	public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager = null)
	{
		if (ppVolume == null) return;
		if (NameReturn(0, Option))
		{
			if (bool.TryParse(Option.SelectedValue, out bool onState))
				ppVolume.enabled = onState;
		}
	}
}
