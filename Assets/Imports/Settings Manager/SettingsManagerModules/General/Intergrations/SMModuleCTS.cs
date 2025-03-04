using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if CTS_PRESENT
using CTS;
#endif
namespace BattlePhaze.SettingsManager.Intergrations
{
    /// <summary>
    /// SMModuleCTS
    /// </summary>
    [AddComponentMenu("BattlePhaze/SettingsManager/Modules/CTS")]
    public class SMModuleCTS : SettingsManagerOption
    {
#if CTS_PRESENT
public CompleteTerrainShader CTS;
#endif
        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                CTSIntergration(Option.SelectedValue);
            }
        }
        public void CTSIntergration(string Quality)
        {
#if CTS_PRESENT
if (CTS && CTS.Profile)
{
switch (Quality.ToLower())
{
case "very low":
CTS.Profile.ShaderType = CTSConstants.ShaderType.Lite;
CTS.Profile.m_globalBasemapDistance = 35;
break;
case "low":
CTS.Profile.ShaderType = CTSConstants.ShaderType.Basic;
CTS.Profile.m_globalBasemapDistance = 100;
break;
case "medium":
CTS.Profile.ShaderType = CTSConstants.ShaderType.Advanced;
CTS.Profile.m_globalBasemapDistance = 200;
break;
case "high":
CTS.Profile.ShaderType = CTSConstants.ShaderType.Advanced;
CTS.Profile.m_globalBasemapDistance = 300;
break;
case "very high":
CTS.Profile.ShaderType = CTSConstants.ShaderType.Advanced;
CTS.Profile.m_globalBasemapDistance = 400;
break;
case "ultra":
CTS.Profile.ShaderType = CTSConstants.ShaderType.Tesselation;
CTS.Profile.m_globalBasemapDistance = 400;
break;
}
}
else
{
CTS = FindObjectOfType<CompleteTerrainShader>();
if (CTS)
{
CTSIntergration(Quality);
}
}
#endif
        }
    }
}