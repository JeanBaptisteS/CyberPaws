using BattlePhaze.SettingsManager;
using UnityEngine;
using UnityEngine.Rendering;
#if SETTINGS_MANAGER_HD
using UnityEngine.Rendering.HighDefinition;
#endif

#if SETTINGS_MANAGER_LEGACY && UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

namespace BattlePhaze.SettingsManager.Integrations
{
    public class SMModuleBrightness : SettingsManagerOption
    {
        public bool UsePostExposure;
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                if (SliderReadOption(Option, Manager, out float Value))
                {
                    if (UsePostExposure)
                    {
                        BURPBrightness(Value);
                        HDBrightness(Value);
                    }
                    else
                    {
                        RenderSettings.ambientLight = new Color(Value, Value, Value, 1.0f);
                    }
                }
            }
        }
        public void BURPBrightness(float Brightness)
        {
#if SETTINGS_MANAGER_LEGACY && UNITY_POST_PROCESSING_STACK_V2
                PostProcessVolume postProcessVolume = FindObjectOfType<PostProcessVolume>(true);
                if (postProcessVolume != null)
                {
                    postProcessVolume.sharedProfile.TryGetSettings(out  ColorGrading grading);
                    grading.postExposure.value = Brightness;
                }
#endif
        }
        public void HDBrightness(float Brightness)
        {
#if SETTINGS_MANAGER_HD
                if (FindObjectOfType<Volume>())
                {
                    FindObjectOfType<Volume>().sharedProfile.TryGet(out ColorAdjustments adjustment);
                    adjustment.postExposure.value = Brightness;
                }
#endif
        }
    }
}