using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattlePhaze.SettingsManager;

namespace BattlePhaze.SettingsManager.Intergrations
{
    [AddComponentMenu("BattlePhaze/SettingsManager/Modules/VSPRO")]
    public class SMModuleVegetationStudioPro : SettingsManagerOption
    {
#if VEGETATION_STUDIO_PRO || VEGETATION_STUDIO
        public AwesomeTechnologies.VegetationStudio.QualityManager QualityManager;
        public bool forceRefresh;
        public bool LookUpVegetationStudiosQuality;
#endif

        public override void ReceiveOption(SettingsMenuInput Option, SettingsManager Manager)
        {
            if (NameReturn(0, Option))
            {
                ChangeVegetationQuality(Option.SelectedValue);
            }
        }

        public void ChangeVegetationQuality(string Quality)
        {
#if VEGETATION_STUDIO_PRO || VEGETATION_STUDIO
            if (QualityManager)
            {
                if (FindObjectOfType<AwesomeTechnologies.VegetationStudio.QualityManager>())
                {
                    QualityManager = FindObjectOfType<AwesomeTechnologies.VegetationStudio.QualityManager>();
                    if (LookUpVegetationStudiosQuality)
                    {
                        int index = -1;
                        switch (Quality)
                        {
                            case "very low":
                                index = 0;
                                break;
                            case "low":
                                index = 1;
                                break;
                            case "medium":
                                index = 2;
                                break;
                            case "high":
                                index = 3;
                                break;
                            case "ultra":
                                index = 4;
                                break;
                        }
                        if (index != -1)
                        {
                            QualityManager.SetQualityLevel(index, forceRefresh);
                        }
                    }
                }
            }
            else
            {
                AwesomeTechnologies.VegetationSystem.VegetationSystemPro MainVeg = FindObjectOfType<AwesomeTechnologies.VegetationSystem.VegetationSystemPro>();
                if (MainVeg)
                {
                    Dictionary<string, object> vegetationSettings = new Dictionary<string, object>()
                    {
                        {"LargeObjectShadows", true},
                        {"PlantShadows", true},
                        {"TreeShadows", true},
                        {"BillboardShadows", true},
                        {"AdditionalBillboardDistance", 25},
                        {"AdditionalTreeMeshDistance", 25},
                        {"GrassShadows", true}
                    };
                    switch (Quality)
                    {
                        case "very low":
                            vegetationSettings["PlantDistance"] = 30;
                            vegetationSettings["GrassDensity"] = 0.1f;
                            break;
                        case "low":
                            vegetationSettings["PlantDistance"] = 40;
                            vegetationSettings["GrassDensity"] = 0.3f;
                            break;
                        case "medium":
                            vegetationSettings["PlantDistance"] = 50;
                            vegetationSettings["GrassDensity"] = 0.5f;
                            break;
                        case "high":
                            vegetationSettings["PlantDistance"] = 60;
                            vegetationSettings["GrassDensity"] = 0.7f;
                            break;
                        case "very high":
                            vegetationSettings["PlantDistance"] = 80;
                            vegetationSettings["GrassDensity"] = 1.0f;
                            break;
                    }
                    foreach (KeyValuePair<string, object> pair in vegetationSettings)
                    {
                        MainVeg.VegetationSettings.GetType().GetProperty(pair.Key).SetValue(MainVeg.VegetationSettings, pair.Value);
                    }
                    MainVeg.RefreshVegetationSystem();
                }
#endif
        }
    }
}