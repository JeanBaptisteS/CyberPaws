using UnityEngine;

namespace BattlePhaze.SettingsManager.Intergrations
{
    [AddComponentMenu("BattlePhaze/SettingsManager/Modules/EnviroIntergration")]
    public class SMModuleEnviro : SettingsManagerOption
    {
#if ENVIRO_HD
        public EnviroSky Enviro;
#endif
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                EnviroIntergration(Option.SelectedValue);
            }
        }

        public void EnviroIntergration(string Quality)
        {
#if ENVIRO_HD
            if (Enviro)
            {
                switch (Quality.ToLower())
                {
                    case "very low":
                        Enviro.qualitySettings.UpdateInterval = 1f;
                        Enviro.useVolumeClouds = false;
                        Enviro.useVolumeLighting = false;
                        Enviro.useFog = true;
                        break;
                    case "low":
                        Enviro.qualitySettings.UpdateInterval = 0.75f;
                        Enviro.useVolumeClouds = false;
                        Enviro.useVolumeLighting = false;
                        Enviro.useFog = true;
                        break;
                    case "medium":
                        Enviro.qualitySettings.UpdateInterval = 0.5f;
                        Enviro.useVolumeClouds = false;
                        Enviro.useVolumeLighting = false;
                        Enviro.useFog = true;
                        break;
                    case "high":
                        Enviro.qualitySettings.UpdateInterval = 0.4f;
                        Enviro.useVolumeClouds = true;
                        Enviro.useVolumeLighting = true;
                        Enviro.useFog = false;
                        break;
                    case "very high":
                        Enviro.qualitySettings.UpdateInterval = 0.3f;
                        Enviro.useVolumeClouds = true;
                        Enviro.useVolumeLighting = true;
                        Enviro.useFog = false;
                        break;
                    case "ultra":
                        Enviro.qualitySettings.UpdateInterval = 0.2f;
                        Enviro.useVolumeClouds = true;
                        Enviro.useVolumeLighting = true;
                        Enviro.useFog = false;
                        break;
                }
            }
            else
            {
                Enviro = FindObjectOfType<EnviroSky>();
                if (Enviro)
                {
                    EnviroIntergration(Quality);
                }
            }
#endif
        }
    }
}