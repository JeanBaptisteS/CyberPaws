using BattlePhaze.SettingsManager;
using UnityEngine;
using UnityEngine.Rendering;
#if SM_Volume
using BeautifyHDRP;
#endif
#if UNITY_EDITOR
using BattlePhaze.SettingsManager.EditorDefine;
#endif
namespace BattlePhaze.SettingsManager.Intergrations
{
    [AddComponentMenu("BattlePhaze/SettingsManager/Modules/Beautify")]
    public class SMModuleBeautify : SettingsManagerOption
    {
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                ChangeBeautify(Option.SelectedValue);
            }
        }

        public void ChangeBeautify(string Quality)
        {
#if Beautify
            Volume volume = FindObjectOfType<Volume>();
            if (volume)
            {
                volume.profile.TryGet<Beautify>(out Beautify beautify);
                if (beautify)
                {
                    switch (Quality.ToLower())
                    {
                        case "off":
                            beautify.active = false;
                            break;
                        case "on":
                            beautify.active = true;
                            break;
                    }
                }
            }
#else
#if UNITY_EDITOR
            SettingsManagerDefines.IntegrationGenerate("volume", "SM_Beautify");
#endif
#endif
        }
    }
}