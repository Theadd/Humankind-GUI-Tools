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
    public static partial class DataTypeStore
    {
        public static DefinitionsGroup[] Districts { get; set; }
        public static DefinitionsGroup[] Units { get; set; }
        public static DefinitionsGroup[] Curiosities { get; set; }

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

        private static IDatabase<LootTableDefinition> LootTables { get; set; }

        public static DataTypeDefinition CreateDistrictDataType(DistrictDefinition definition)
        {
            var name = LastName(definition.name);

            return new DataTypeDefinition()
            {
                DefinitionName = definition.Name,
                Title = UIController.GetLocalizedTitle(definition.Name, name),
                Name = name,
                Category = definition.Category.ToString(),
                Era = EraLevel(definition.name),
                Image = AssetHunter.LoadTexture(definition)     // Utils.LoadTexture(definition.name)
            };
        }
        
        public static DataTypeDefinition CreateUnitDataType(UnitDefinition definition)
        {
            var name = UnitLastName(definition.name);

            return new DataTypeDefinition()
            {
                DefinitionName = definition.Name,
                Title = UIController.GetLocalizedTitle(definition.Name, name),
                Name = name,
                Category = UIController.GetLocalizedTitle(definition.UnitClassName),
                Era = UnitEraLevel(definition.name),
                Image = AssetHunter.LoadTexture(definition)     // Utils.LoadTexture(definition.name)
            };
        }
        
        public static DataTypeDefinition CreateCuriosityDataType(CuriosityDefinition definition)
        {
            var name = definition.name;
            string[] lootTypes;
            LootTableDefinition lootTable;

            try
            {
                lootTable = LootTables.GetValue(definition.LootTableReference.ElementName);
                lootTypes = lootTable.Loots.SelectMany(
                    loot => loot.SimulationEventEffects.SelectMany(
                        e => e.GainValues?.Select(
                            g => g.Type.ToString()) ?? new string[] { })).Distinct().ToArray();
            }
            catch (Exception)
            {
                lootTypes = new[] { "" };
            }

            return new DataTypeDefinition()
            {
                DefinitionName = definition.Name,
                Title = UIController.GetLocalizedTitle(definition.Name, name),
                Name = name,
                Category = "" + string.Join(", ", lootTypes),
                Era = definition.EraIndex,
                Image = AssetHunter.LoadTexture(definition)
            };
        }

        public static void Rebuild(DataTypeStoreBuildOptions options = null)
        {
            options = options ?? new DataTypeStoreBuildOptions();
            
            // CONSTRUCTIBLES
            
            var constructibles = Databases
                .GetDatabase<ConstructibleDefinition>(false);

            var districts = constructibles
                .OfType<DistrictDefinition>()
                .ToArray();

            Districts = RebuildDistricts(
                options.ExcludeKnownInvalid
                    ? districts.Where(d => !ExcludeNames.Contains(d.name)).ToArray()
                    : districts, 
                options);
            
            var units = constructibles
                .OfType<UnitDefinition>()
                .ToArray();

            Units = RebuildUnits(
                options.ExcludeKnownInvalid 
                    ? units.Where(u => !ExcludeNames.Contains(u.name)).ToArray()
                    : units, 
                options);
            
            // COLLECTIBLES

            var collectibles = Databases.GetDatabase<CollectibleDefinition>(false);
            
            var curiosities = collectibles
                .OfType<CuriosityDefinition>()
                .ToArray();
                
            LootTables = Databases.GetDatabase<LootTableDefinition>(false);

            Curiosities = RebuildCuriosities(
                options.ExcludeObsolete 
                    ? curiosities.Where(c => !c.IsObsolete) 
                    : curiosities,
                options);
        }

        private static DefinitionsGroup[] RebuildCuriosities(IEnumerable<CuriosityDefinition> curiosities,
            DataTypeStoreBuildOptions options)
        {
            return curiosities
                .Select(c => new { group = c.GetType().Name, definition = c }).ToList()
                .GroupBy(c => c.group).ToList()
                .Select(g => new DefinitionsGroup
                {
                    Title = R.Text.SplitCamelCase(g.First().group).Replace("Definition", "Definitions"),
                    Values = g.OrderBy(i => i.definition.EraIndex)
                        .Select(d => CreateCuriosityDataType(d.definition))
                        .ToArray()
                })
                .OrderBy(gs => gs.Title).ToArray();
        }

        private static DefinitionsGroup[] RebuildDistricts(DistrictDefinition[] districts, DataTypeStoreBuildOptions options)
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
                        .Select(CreateDistrictDataType)
                        .ToArray()
                },
                
            };
            
            if (!options.ExcludeExtractorsAndManufactories)
                result = result.Append(new DefinitionsGroup() 
                    {
                        Title = "Extractors & Manufactories",
                        Values = resourceDistricts
                            .Select(CreateDistrictDataType)
                            .ToArray()
                    });

            result = result.Append(new DefinitionsGroup()
            {
                Title = "Emblematic Districts",
                Values = emblematicDistricts
                    .Select(CreateDistrictDataType)
                    .ToArray()
            });
            
            if (!options.ExcludeOthersGroup && otherDistricts.Length > 0)
                result = result.Append(new DefinitionsGroup()
                {
                    Title = "Other Districts",
                    Values = otherDistricts
                        .Select(CreateDistrictDataType)
                        .ToArray()
                });
            

            return result;
        }

        private static DefinitionsGroup[] RebuildUnits(UnitDefinition[] units, DataTypeStoreBuildOptions options)
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
                        .Select(CreateUnitDataType)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Maritime Units",
                    Values = maritimeUnits
                        .Select(CreateUnitDataType)
                        .ToArray()
                },
                new DefinitionsGroup()
                {
                    Title = "Air & Missile Units",
                    Values = airUnits
                        .Select(CreateUnitDataType)
                        .ToArray()
                }
            };

            if (!options.ExcludeOthersGroup && otherUnits.Length > 0)
                result = result.Append(new DefinitionsGroup()
                {
                    Title = "Other Units",
                    Values = otherUnits
                        .Select(CreateUnitDataType)
                        .ToArray()
                });

            return result;
        }

    }
}
 