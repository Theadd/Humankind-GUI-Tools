﻿using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Mercury.Data.Simulation;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class DataTypeStore
    {
        
        public static void PrintAllDistricts()
        {
            var constructibles = Databases
                .GetDatabase<ConstructibleDefinition>(false);
            var districts = constructibles
                .OfType<DistrictDefinition>()
                // .Take(6)
                .ToArray();
            
            foreach (var district in districts)
            {
                var tex = AssetHunter.LoadTexture(district);
                if (tex == null)
                {
                    Loggr.Log("%RED% TEXTURE FOR " + district.name + " NOT FOUND.");
                    continue;
                }
                Loggr.Log(tex.name + " %GREEN% " + tex.width + "x" + tex.height);
                // Loggr.Log("" + district.Name.ToString() + " => " + district.Prototype, ConsoleColor.DarkYellow);
            }
        }

        public static void PrintBatchProcess()
        {
            var constructibles = Databases
                .GetDatabase<ConstructibleDefinition>(false);

            var key = new StaticString("Small");
            var key2 = new StaticString("Default_Small");
            List<ConstructibleDefinition> remaining = new List<ConstructibleDefinition>();
            List<string> result = new List<string>();
            int failCount = 0;
            int count = 0;
            string assetPath = "";
            foreach (var definition in constructibles.GetValues())
            {
                count++;
                try
                {
                    var uiTexture = UIController.DataUtils.GetImage(definition.Name, key);
                    assetPath = "" + uiTexture.AssetPath;
                    if (assetPath.Length == 0)
                    {
                        assetPath = "" + UIController.DataUtils.GetImage(definition.Name, key2).AssetPath;
                    }
                    if (assetPath.Length == 0)
                    {
                        remaining.Add(definition);
                        failCount++;
                        continue;
                    }

                    var filename = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
                    result.Add("cp -u " + filename + " ../Small/" + definition.name + ".png");
                }
                catch (Exception)
                {
                    failCount++;
                }
            }
            Loggr.Log(failCount.ToString() + " FAILED OUT OF " + count + ", REMAINING = " + remaining.Count, ConsoleColor.Green);
            
            foreach (var constructibleDefinition in remaining)
            {
                Loggr.Log("" + constructibleDefinition.name.ToString(), ConsoleColor.DarkCyan);
            }
            
            Loggr.Log("\n\n");
            
            foreach (var s in result)
            {
                Loggr.Log(s, ConsoleColor.Green);
            }
            
            Loggr.Log("\n\n");
        }
        
        
        public static void TempPrintAll()
        {
            var constructibles = Databases
                .GetDatabase<ConstructibleDefinition>(false);

            var key = new StaticString("Small");
            var key2 = new StaticString("Default_Small");
            List<ConstructibleDefinition> remaining = new List<ConstructibleDefinition>();
            List<string> result = new List<string>();
            int failCount = 0;
            int count = 0;
            string assetPath = "";
            foreach (var definition in constructibles.GetValues())
            {
                count++;
                try
                {
                    var uiTexture = UIController.DataUtils.GetImage(definition.Name, key);
                    assetPath = "" + uiTexture.AssetPath;
                    if (assetPath.Length == 0)
                    {
                        assetPath = "" + UIController.DataUtils.GetImage(definition.Name, key2).AssetPath;
                    }
                    if (assetPath.Length == 0)
                    {
                        remaining.Add(definition);
                        failCount++;
                        continue;
                    }

                    if (assetPath.Contains("Hawaiians"))
                    {
                        Loggr.Log(uiTexture);
                        Loggr.Log(UIController.DataUtils.GetImage(definition.Name, key2));
                    }

                    // var filename = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
                    result.Add("%GREEN%" + definition.name + " %RED% " + assetPath);
                }
                catch (Exception)
                {
                    failCount++;
                }
            }
            Loggr.Log(failCount.ToString() + " FAILED OUT OF " + count + ", REMAINING = " + remaining.Count, ConsoleColor.Green);
            
            foreach (var constructibleDefinition in remaining)
            {
                Loggr.Log("" + constructibleDefinition.name.ToString(), ConsoleColor.DarkCyan);
            }
            
            Loggr.Log("\n\n");
            
            foreach (var s in result)
            {
                // Loggr.Log(s, ConsoleColor.Green);
            }
            
            Loggr.Log("\n\n");
        }
    }
}


/*
using System.Collections.Generic;
using System.Linq;
using Amplitude.Framework;
using Amplitude.Mercury.Data.Simulation;

namespace Modding.Humankind.DevTools.Core
{
    public static class QuickAccess
    {
        private static UnitDefinition[] _unitDefinitions = null;

        public static UnitDefinition[] UnitDefinitions =>
            _unitDefinitions ?? (_unitDefinitions = Databases
                .GetDatabase<ConstructibleDefinition>(false)
                .OfType<UnitDefinition>()
                .ToArray());
        
        // UITooltipClassDefinition
    }
}

 
 "Extension_ArtificialWonder_Era1_HangingGardensOfBabylon"
"Extension_ArtificialWonder_Era1_PyramidOfGiza"
"Extension_ArtificialWonder_Era1_Stonehenge"
"Extension_ArtificialWonder_Era1_TempleOfArtemis"
"Extension_ArtificialWonder_Era2_ColossusOfRhodes"
"Extension_ArtificialWonder_Era2_LighthouseOfAlexandria"
"Extension_ArtificialWonder_Era2_MausoleumAtHalicarnassus"
"Extension_ArtificialWonder_Era2_StatueOfZeus"
"Extension_ArtificialWonder_Era3_AngkorWat"
"Extension_ArtificialWonder_Era3_ForbiddenCity"
"Extension_ArtificialWonder_Era3_NotreDameDeParis"
"Extension_ArtificialWonder_Era3_TodaiJi"
"Extension_ArtificialWonder_Era4_MachuPicchu"
"Extension_ArtificialWonder_Era4_SaintBasilsCathedral"
"Extension_ArtificialWonder_Era4_TajMahal"
"Extension_ArtificialWonder_Era4_TopkapiPalace"
"Extension_ArtificialWonder_Era5_BigBen"
"Extension_ArtificialWonder_Era5_EiffelTower"
"Extension_ArtificialWonder_Era5_StatueOfLiberty"
"Extension_ArtificialWonder_Era6_ChristTheRedeemer"
"Extension_ArtificialWonder_Era6_EmpireStateBuilding"
"Extension_ArtificialWonder_Era6_SydneyOperaHouse"
"Extension_Base_AdministrativeCenter"
"Extension_Base_Airfield"
"Extension_Base_Airport"
"Extension_Base_CityCenter"
"Extension_Base_Extractor"
"Extension_Base_Food"
"Extension_Base_Harbour"
"Extension_Base_Industry"
"Extension_Base_Luxury"
"Extension_Base_Military"
"Extension_Base_MissileSilo"
"Extension_Base_Money"
"Extension_Base_NaturalReserve"
"Extension_Base_PublicOrder"
"Extension_Base_Science"
"Extension_Base_Strategic01"
"Extension_Base_Strategic02"
"Extension_Base_Strategic03"
"Extension_Base_Strategic04"
"Extension_Base_Strategic05"
"Extension_Base_Strategic06"
"Extension_Base_Strategic07"
"Extension_Base_Strategic08"
"Extension_Base_TrainStation"
"Extension_Base_VillageCenter"
"Extension_Base_WondrousExtractor"
"Extension_Camp_BeforeCamp"
"Extension_Camp_CampCenter"
"Extension_Prototype_Base"
"Extension_Prototype_BaseEconomy"
"Extension_Prototype_BaseMilitary"
"Extension_Prototype_Emblematic"
"Extension_Wondrous_Resource11"
"Extension_Wondrous_Resource12"
"Extension_Wondrous_Resource13"
"Extension_Wondrous_Resource14"
"Extension_Wondrous_Resource15"
"Extension_Wondrous_Resource16"
"Extension_Wondrous_Resource17"
"Extension_Wondrous_Resource18"
"Extension_Wondrous_Resource19"
"Extension_Wondrous_Resource20"
"Extension_Wondrous_Resource21"
"Extension_Wondrous_Resource22"
"Extension_Wondrous_Resource23"
"Extension_Wondrous_Resource24"
"Extension_Wondrous_Resource25"
"Extension_Wondrous_Resource26"
"Extension_Wondrous_Resource27"
"Extension_Wondrous_Resource28"
"Extension_Wondrous_Resource29"
"Extension_Wondrous_Resource30"
"Extension_Wondrous_Resource31"
"Extension_Wondrous_Template"
"Extension_Era1_Assyria"
"Extension_Era1_Babylon"
"Extension_Era1_EgyptianKingdom"
"Extension_Era1_HarappanCivilization"
"Extension_Era1_HittiteEmpire"
"Extension_Era1_MycenaeanCivilization"
"Extension_Era1_Nubia"
"Extension_Era1_OlmecCivilization"
"Extension_Era1_Phoenicia"
"Extension_Era1_ZhouChina"
"Extension_Era2_AksumiteEmpire"
"Extension_Era2_AncientGreece"
"Extension_Era2_Carthage"
"Extension_Era2_CelticCivilization"
"Extension_Era2_Goths"
"Extension_Era2_Huns"
"Extension_Era2_MauryaEmpire"
"Extension_Era2_MayaCivilization"
"Extension_Era2_Persia"
"Extension_Era2_RomanEmpire"
"Extension_Era3_AztecEmpire"
"Extension_Era3_Byzantium"
"Extension_Era3_FrankishKingdom"
"Extension_Era3_GhanaEmpire"
"Extension_Era3_HolyRomanEmpire"
"Extension_Era3_KhmerEmpire"
"Extension_Era3_MedievalEngland"
"Extension_Era3_MongolEmpire"
"Extension_Era3_UmayyadCaliphate"
"Extension_Era3_Vikings"
"Extension_Era4_Holland"
"Extension_Era4_IroquoisConfederacy"
"Extension_Era4_JoseonKorea"
"Extension_Era4_MingChina"
"Extension_Era4_MughalEmpire"
"Extension_Era4_OttomanEmpire"
"Extension_Era4_PolishKingdom"
"Extension_Era4_Spain"
"Extension_Era4_TokugawaShogunate"
"Extension_Era4_VenetianRepublic"
"Extension_Era5_AfsharidPersia"
"Extension_Era5_AustriaHungary"
"Extension_Era5_BritishEmpire"
"Extension_Era5_FrenchRepublic"
"Extension_Era5_Germany"
"Extension_Era5_Italy"
"Extension_Era5_Mexico"
"Extension_Era5_RussianEmpire"
"Extension_Era5_Siam"
"Extension_Era5_ZuluKingdom"
"Extension_Era6_Australia"
"Extension_Era6_Brazil"
"Extension_Era6_China"
"Extension_Era6_Egypt"
"Extension_Era6_India"
"Extension_Era6_Japan"
"Extension_Era6_Sweden"
"Extension_Era6_Turkey"
"Extension_Era6_USA"
"Extension_Era6_USSR"
"Extension_HolySite_Buddhism"
"Extension_HolySite_Christianism"
"Extension_HolySite_Hinduism"
"Extension_HolySite_Islam"
"Extension_HolySite_Judaism"
"Extension_HolySite_Polytheism"
"Extension_HolySite_Shamanism"
"Extension_HolySite_Shintoism"
"Extension_HolySite_Taoism"
"Extension_HolySite_Zoroastrism"
"Extension_NationalProject_NuclearTest_Level1"
"Extension_NationalProject_NuclearTest_Level2"
"Extension_NationalProject_NuclearTest_Level3"
"Extension_NationalProject_SatelliteLaunch_Level1"
"Extension_NationalProject_SatelliteLaunch_Level2"
"Extension_NationalProject_SatelliteLaunch_Level3"
"Extension_NationalProject_SpaceRace_Level1"
"Extension_NationalProject_SpaceRace_Level2"
"Extension_NationalProject_SpaceRace_Level3"
"Extension_ArtificialWonder_Era2_LadyMaryOfZion"
"Extension_ArtificialWonder_Era3_GreatMosqueOfDjenne"
"Extension_ArtificialWonder_Era3_GreatZimbabwe"
"Extension_Era1_Bantu"
"Extension_Era2_Garamantes"
"Extension_Era3_Swahili"
"Extension_Era4_Maasai"
"Extension_Era5_Ethiopia"
"Extension_Era6_Nigeria"


UI_Extension_ArtificialWonder_Era3_GreatZimbabwe_Small.png
Extension_ArtificialWonder_Era3_GreatZimbabwe

UI_ConstructibleInfrastructure_Repeatable_Era3_GreatMosqueOfDjenne_Small.png
UI_ConstructibleExtension_Era5_Siam_Small.png
 */
