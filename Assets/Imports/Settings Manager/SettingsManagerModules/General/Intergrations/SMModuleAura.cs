using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if AURA_IN_PROJECT
using Aura2API;
#endif

namespace BattlePhaze.SettingsManager.Intergrations
{
    /// <summary>
    /// SMMod
    /// </summary>
    [AddComponentMenu("BattlePhaze/SettingsManager/Modules/Aura")]
    public class SMModuleAura : SettingsManagerOption
    {
#if AURA_IN_PROJECT
        public AuraCamera AuraCamera;
#endif

        /// <summary>
        /// Receive Option
        /// </summary>
        /// <param name="Option"></param>
        /// <returns></returns>
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                AuraIntergration(Option.SelectedValue.ToLower());
            }
        }

        /// <summary>
        ///Aura Intergration
        /// </summary>
        public void AuraIntergration(string quality)
        {
#if AURA_IN_PROJECT
            if (AuraCamera && AuraCamera.frustumSettings.qualitySettings)
            {
                switch (quality)
                {
                    case "very low":
                        AuraCamera.frustumSettings.qualitySettings.enablePointLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableDirectionalLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableSpotLightsShadows = false;
                        break;
                    case "low":
                        AuraCamera.frustumSettings.qualitySettings.enablePointLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableDirectionalLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableSpotLightsShadows = false;
                        break;
                    case "medium":
                        AuraCamera.frustumSettings.qualitySettings.enablePointLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableDirectionalLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableSpotLightsShadows = false;
                        break;
                    case "high":
                        AuraCamera.frustumSettings.qualitySettings.enablePointLightsShadows = false;
                        AuraCamera.frustumSettings.qualitySettings.enableDirectionalLightsShadows = true;
                        AuraCamera.frustumSettings.qualitySettings.enableSpotLightsShadows = false;
                        break;
                    case "very high":
                        AuraCamera.frustumSettings.qualitySettings.enablePointLightsShadows = true;
                        AuraCamera.frustumSettings.qualitySettings.enableDirectionalLightsShadows = true;
                        AuraCamera.frustumSettings.qualitySettings.enableSpotLightsShadows = true;
                        break;
                    case "ultra":
                        AuraCamera.frustumSettings.qualitySettings.enablePointLightsShadows = true;
                        AuraCamera.frustumSettings.qualitySettings.enableDirectionalLightsShadows = true;
                        AuraCamera.frustumSettings.qualitySettings.enableSpotLightsShadows = true;
                        break;
                }
            }
            else
            {
                AuraCamera = FindObjectOfType<AuraCamera>();
                if (AuraCamera)
                {
                    AuraIntergration(quality);
                }
            }
#endif
        }
    }
}