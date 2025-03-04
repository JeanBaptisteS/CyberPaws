using UnityEngine;
using BattlePhaze.SettingsManager;
#if SM_MadGoatSSAA
using MadGoat.SSAA;
#endif
#if UNITY_EDITOR
using BattlePhaze.SettingsManager.EditorDefine;
#endif
namespace BattlePhaze.SettingsManager.Intergrations
{
    [AddComponentMenu("BattlePhaze/SettingsManager/Modules/MadGoatSSAA")]
    public class SMModuleMadGoatSSAA : SettingsManagerOption
    {
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                ChangeAntialiasing(Option.SelectedValue);
            }
        }
        public void ChangeAntialiasing(string Quality)
        {
#if SM_MadGoatSSAA
#else
#if UNITY_EDITOR
            SettingsManagerDefines.IntegrationGenerate("MadGoatSSAA", "SM_MadGoatSSAA");
#endif
#endif
#if SM_MadGoatSSAA
        var madGoatSSAA = Object.FindObjectOfType<MadGoatSSAA>();
        if (madGoatSSAA)
        {
            switch (Quality)
            {
                case "SSAA_HALF":
                    madGoatSSAA.SetAsSSAA(SSAAMode.SSAA_HALF);
                    madGoatSSAA.SetPostAAMode(PostAntiAliasingMode.Off);
                    break;
                case "SSAA_OFF":
                    madGoatSSAA.SetAsSSAA(SSAAMode.SSAA_OFF);
                    madGoatSSAA.SetPostAAMode(PostAntiAliasingMode.Off);
                    break;
                case "SSAA_X2":
                    madGoatSSAA.SetAsSSAA(SSAAMode.SSAA_X2);
                    madGoatSSAA.SetPostAAMode(PostAntiAliasingMode.FSSAA);
                    break;
                case "SSAA_X4 FSSAA":
                    madGoatSSAA.SetAsSSAA(SSAAMode.SSAA_X4);
                    madGoatSSAA.SetPostAAMode(PostAntiAliasingMode.FSSAA);
                    break;
                case "SSAA_X4 TSSAA":
                    madGoatSSAA.SetAsSSAA(SSAAMode.SSAA_X4);
                    madGoatSSAA.SetPostAAMode(PostAntiAliasingMode.TSSAA);
                    break;
            }   
        }
#endif
        }
    }
}