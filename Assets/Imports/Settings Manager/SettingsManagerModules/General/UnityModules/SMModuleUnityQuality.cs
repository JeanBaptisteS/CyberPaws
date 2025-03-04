using UnityEngine;
namespace BattlePhaze.SettingsManager.Intergrations
{
    public class SMModuleUnityQuality : SettingsManagerOption
    {
        /// <summary>
        /// Recieve option
        /// </summary>
        /// <param name="Option"></param>
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                ChangeUnityQuality(Option.SelectedValue);
            }
        }
        /// <summary>
        /// Changes the quality level components
        /// (does not set the actual quality setting level as this would interfere with other modules)
        /// </summary>
        /// <param name="Quality"></param>
        public void ChangeUnityQuality(string Quality)
        {
            switch (Quality)
            {
                case "very low":
                    QualitySettings.SetQualityLevel(0, true);
                    break;
                case "low":
                    QualitySettings.SetQualityLevel(1, true);
                    break;
                case "medium":
                    QualitySettings.SetQualityLevel(2, true);
                    break;
                case "high":
                    QualitySettings.SetQualityLevel(3, true);
                    break;
                case "ultra":
                    QualitySettings.SetQualityLevel(4, true);
                    break;
            }
        }
    }
}