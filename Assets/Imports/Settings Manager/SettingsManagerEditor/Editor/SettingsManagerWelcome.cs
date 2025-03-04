using UnityEditor;
namespace BattlePhaze.SettingsManager.SMUnityEditor
{
    [InitializeOnLoad]
    public class SettingsManagerWelcome
    {
        /// <summary>
        /// On First Time Display Dialog about Settings Manager
        /// </summary>
        static SettingsManagerWelcome()
        {
            if (!EditorPrefs.HasKey("SM.Installed"))
            {
                EditorPrefs.SetInt("SM.Installed", 1);
                EditorUtility.DisplayDialog("Settings Manager", "Thank you for using Settings Manager! Please read the documentation found in the Package directory of settings manager.", "Ok");
            }
        }
    }
}