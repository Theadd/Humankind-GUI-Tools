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
            "Exploitation",
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

        public static void Rebuild(ConstructibleStoreBuildOptions options = null)
        {
            options = options ?? new ConstructibleStoreBuildOptions();
            
            var constructibles = Databases
                .GetDatabase<ConstructibleDefinition>(false);

            var districts = constructibles
                .OfType<DistrictDefinition>()
                .ToArray();

            Districts = RebuildDistricts(
                options.ExcludeKnownInvalid
                    ? districts.Where(d => !ExcludeNames.Contains(d.name)).ToArray()
                    : districts, options);
            
            var units = constructibles
                .OfType<UnitDefinition>()
                .ToArray();

            Units = RebuildUnits(
                options.ExcludeKnownInvalid 
                    ? units.Where(u => !ExcludeNames.Contains(u.name)).ToArray()
                    : units, options);
        }

        private static DefinitionsGroup[] RebuildDistricts(DistrictDefinition[] districts, ConstructibleStoreBuildOptions options)
        {
            var commonDistricts = districts.Where(CommonDistrictsClause);
            var remaining = districts.Where(d => !CommonDistrictsClause(d)).ToArray();
            var resourceDistricts = remaining.Where(ResourceDistrictsClause);
            remaining = remaining.Where(d => !ResourceDistrictsClause(d)).ToArray();
            var emblematicDistricts = remaining.Where(EmblematicDistrictsClause);
            var otherDistricts = remaining.Where(d => !EmblematicDistrictsClause(d)).ToArray();

            var result = new DefinitionsGroup[]
            {
                new DefinitionsGroup()
                {
                    Title = "Common Districts",
                    Values = commonDistricts
                        .Select(CreateDistrictConstructible)
                        .ToArray()
                },
                
            };
            
            if (!options.ExcludeExtractorsAndManufactories)
                result = result.Append(new DefinitionsGroup() 
                    {
                        Title = "Extractors & Manufactories",
                        Values = resourceDistricts
                            .Select(CreateDistrictConstructible)
                            .ToArray()
                    });

            result = result.Append(new DefinitionsGroup()
            {
                Title = "Emblematic Districts",
                Values = emblematicDistricts
                    .Select(CreateDistrictConstructible)
                    .ToArray()
            });
            
            if (!options.ExcludeOthersGroup && otherDistricts.Length > 0)
                result = result.Append(new DefinitionsGroup()
                {
                    Title = "Other Districts",
                    Values = otherDistricts
                        .Select(CreateDistrictConstructible)
                        .ToArray()
                });
            

            return result;
        }

        private static DefinitionsGroup[] RebuildUnits(UnitDefinition[] units, ConstructibleStoreBuildOptions options)
        {
            var landUnits = units
                .Where(u => u.SpawnType == UnitSpawnType.Land && !(u is NavalTransportDefinition))
                .OrderBy(u => UnitEraLevel(u.name));
            var remaining = units.Where(u => !(u.SpawnType == UnitSpawnType.Land && !(u is NavalTransportDefinition))).ToArray();
            var maritimeUnits = remaining
                .Where(u => u.SpawnType == UnitSpawnType.Maritime || u is NavalTransportDefinition)
                .OrderBy(u => UnitEraLevel(u.name));
            remaining = remaining.Where(u => !(u.SpawnType == UnitSpawnType.Maritime || u is NavalTransportDefinition)).ToArray();
            var airUnits = remaining
                .Where(u => u.SpawnType == UnitSpawnType.Air || u.SpawnType == UnitSpawnType.Missile)
                .OrderBy(u => UnitEraLevel(u.name));
            var otherUnits = remaining.Where(u => u.SpawnType != UnitSpawnType.Air && u.SpawnType != UnitSpawnType.Missile).ToArray();
            
            var result = new DefinitionsGroup[]
            {
                new DefinitionsGroup()
                {
                    Title = "Land Units",
                    Values = landUnits
                        .Select(CreateUnitConstructible)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Maritime Units",
                    Values = maritimeUnits
                        .Select(CreateUnitConstructible)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Air & Missile Units",
                    Values = airUnits
                        .Select(CreateUnitConstructible)
                        .ToArray()
                }
            };

            if (!options.ExcludeOthersGroup && otherUnits.Length > 0)
                result = result.Append(new DefinitionsGroup()
                {
                    Title = "Other Units",
                    Values = otherUnits
                        .Select(CreateUnitConstructible)
                        .ToArray()
                });

            return result;
        }

    }
}
 