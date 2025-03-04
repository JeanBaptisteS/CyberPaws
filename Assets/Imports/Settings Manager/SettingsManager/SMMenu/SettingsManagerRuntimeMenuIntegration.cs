using System.Collections.Generic;
using UnityEngine;
namespace BattlePhaze.SettingsManager
{
    public class SettingsManagerRuntimeMenuIntegration : MonoBehaviour
    {
        [SerializeField]
        public List<OptionConfiguration> ToModifyOption = new List<OptionConfiguration>();
        [System.Serializable]
        public struct OptionConfiguration
        {
            public string Option;
            public UnityEngine.Object TextDescription;
            public UnityEngine.Object ObjectInput;
        }
        public void Start()
        {
            SettingsManager Manager = SettingsManager.Instance;
            if (Manager != null)
            {
                for (int ToModifyOptionIndex = 0; ToModifyOptionIndex < ToModifyOption.Count; ToModifyOptionIndex++)
                {
                    int Output = SettingsManagerLoop(ToModifyOption[ToModifyOptionIndex].Option, Manager);
                    if (Output != int.MinValue)
                    {
                        Manager.Options[Output].ObjectInput = ToModifyOption[ToModifyOptionIndex].ObjectInput;
                        Manager.Options[Output].TextDescription = ToModifyOption[ToModifyOptionIndex].TextDescription;
                        SettingsManagerDescriptionSystem.TxtDescriptionSetText(Manager, Output);
                    }
                }
                Manager.Initalize(true);
                SettingsManagerDescriptionSystem.ExplanationSetup(Manager);
            }
        }
        public int SettingsManagerLoop(string LookUpName, SettingsManager Manager)
        {
            for (int SettingsOptionsIndex = 0; SettingsOptionsIndex < Manager.Options.Count; SettingsOptionsIndex++)
            {
                if (Manager.Options[SettingsOptionsIndex].Name == LookUpName)
                {
                    return SettingsOptionsIndex;
                }
            }
            return int.MinValue;
        }
        public void OnDestroy()
        {
            SettingsManager Manager = SettingsManager.Instance;
            if (Manager != null)
            {
                for (int ToModifyOptionIndex = 0; ToModifyOptionIndex < ToModifyOption.Count; ToModifyOptionIndex++)
                {
                    int Output = SettingsManagerLoop(ToModifyOption[ToModifyOptionIndex].Option, Manager);
                    if (Output != int.MinValue)
                    {
                        Manager.Options[Output].ObjectInput = null;
                        Manager.Options[Output].TextDescription = null;
                    }
                }
            }
        }
    }
}