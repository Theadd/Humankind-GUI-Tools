using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude;
using Amplitude.Extensions;
using Amplitude.Framework;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.UI;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class ConstructibleStore
    {
        public static DefinitionsGroup[] Districts { get; set; }
        public static DefinitionsGroup[] Units { get; set; }

        public static string[] ExcludeNames { get; set; } = new[]
        {
            "Extension_Base_WondrousExtractor",
            "Extension_Camp_BeforeCamp",
            "Extension_Camp_CampCenter",
            "Extension_Prototype_Base",
            "Extension_Prototype_BaseEconomy",
            "Extension_Prototype_BaseMilitary",
            "Extension_Prototype_Emblematic",
            "Ruin",
        };
        
        public static Constructible CreateDistrictConstructible(DistrictDefinition definition)
        {
            var name = LastName(definition.name);

            return new Constructible()
            {
                DefinitionName = definition.Name,
                Title = UIController.GetLocalizedTitle(definition.Name, name),
                Name = name,
                Category = definition.Category.ToString(),
                Era = EraLevel(definition.name),
                Image = AssetHunter.LoadTexture(definition)     // Utils.LoadTexture(definition.name)
            };
        }
        
        public static Constructible CreateUnitConstructible(UnitDefinition definition)
        {
            var name = UnitLastName(definition.name);

            return new Constructible()
            {
                DefinitionName = definition.Name,
                Title = UIController.GetLocalizedTitle(definition.Name, name),
                Name = name,
                Category = UIController.GetLocalizedTitle(definition.UnitClassName),
                Era = UnitEraLevel(definition.name),
                Image = AssetHunter.LoadTexture(definition)     // Utils.LoadTexture(definition.name)
            };
        }

        public static void Rebuild()
        {
            var constructibles = Databases
                .GetDatabase<ConstructibleDefinition>(false);

            var districts = constructibles
                .OfType<DistrictDefinition>()
                .ToArray();
            
            var units = constructibles
                .OfType<UnitDefinition>()
                .ToArray();

            Districts = new DefinitionsGroup[]
            {
                new DefinitionsGroup()
                {
                    Title = "Common Districts",
                    Values = districts
                        .Where(d => 
                            d.Prototype == "Extension_Prototype_BaseEconomy" || 
                            d.Prototype == "Extension_Prototype_BaseMilitary" || 
                            d.Prototype == "Extension_Prototype_Base" || 
                            d.name == "Extension_Base_Harbour" || 
                            d.name == "Extension_Base_Extractor" || 
                            d.name == "Extension_Base_Luxury" || 
                            d.name == "Exploitation" || 
                            d.name == "Extension_Base_WondrousExtractor")
                        .Select(CreateDistrictConstructible)
                        .ToArray()
                },
                new DefinitionsGroup() 
                {
                    Title = "Extractors & Manufactories",
                    Values = districts
                        .Where(d => d.Prototype == "Extension_Base_Extractor" || d.Prototype == "Extension_Base_WondrousExtractor")
                        .Select(CreateDistrictConstructible)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Emblematic Districts",
                    Values = districts
                        .Where(d => d.Prototype == "Extension_Prototype_Emblematic")
                        .Select(CreateDistrictConstructible)
                        .ToArray()
                }
            };

            Units = new DefinitionsGroup[]
            {
                
                new DefinitionsGroup()
                {
                    Title = "Maritime Units",
                    Values = units
                        .Where(u => u.SpawnType == UnitSpawnType.Maritime)
                        .Select(CreateUnitConstructible)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Land Units",
                    Values = units
                        .Where(u => u.SpawnType == UnitSpawnType.Land)
                        .Select(CreateUnitConstructible)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Air & Missile Units",
                    Values = units
                        .Where(u => u.SpawnType == UnitSpawnType.Air || u.SpawnType == UnitSpawnType.Missile)
                        .Select(CreateUnitConstructible)
                        .ToArray()
                }
            };
        }

    }
}
 