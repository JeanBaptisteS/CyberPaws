using BattlePhaze.SettingsManager;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FmodMixerSettingModule : SettingsManagerOption
{
	private const string MASTER_BUS_PATH = "bus:/";
	private const string SFX_BUS_PATH = "bus:/SFX";
	private const string MUSIC_BUS_PATH = "bus:/Music";
	private const int MASTER_OPTION = 0;
	private const int SFX_OPTION = 1;
	private const int MUSIC_OPTION = 2;

	private Bus masterBus;
	private Bus sfxBus;
	private Bus musicBus;

	private void Start()
	{
		masterBus = RuntimeManager.GetBus(MASTER_BUS_PATH);
		sfxBus = RuntimeManager.GetBus(SFX_BUS_PATH);
		musicBus = RuntimeManager.GetBus(MUSIC_BUS_PATH);
	}

	public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager = null)
	{
		if (!SliderReadOption(Option, Manager, out float value)) return;
		if (NameReturn(MASTER_OPTION, Option))
		{
			masterBus.setVolume(value);
		}
		if (NameReturn(SFX_OPTION, Option))
		{
			sfxBus.setVolume(value);
		}
		if (NameReturn(MUSIC_OPTION, Option))
		{
			musicBus.setVolume(value);
		}
	}
}
