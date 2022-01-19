// using System.Drawing;
using System;
using System.Reflection;
using System.Linq;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using Amplitude.Mercury;

namespace DevTools.Humankind.GUITools.UI
{
    public interface IDataType
    {
        
    }
    
    public interface IEmpireSnapshotDataType : IDataType
    {
        
    }

    public class GameStatsSnapshot
    {
        public int Turn { get; set; }
        public int GameID { get; set; }
        public int GameSpeedLevel { get; set; }
        public EmpireSnapshot[] Empires;
        public int LocalEmpireIndex { get; set; }

        public GameStatsSnapshot()
        {
            Snapshot();
        }

        public GameStatsSnapshot Snapshot()
        {
            Turn = HumankindGame.Turn;
            GameSpeedLevel = HumankindGame.GameSpeedLevel;
            if (GameID != HumankindGame.GameID)
            {
                GameID = HumankindGame.GameID;
                Empires = HumankindGame.Empires.Select(empire => new EmpireSnapshot(empire)).ToArray();
            }
            else
            {
                var empires = HumankindGame.Empires;
                for (var i = 0; i < Empires.Length; i++)
                {
                    Empires[i].Snapshot(empires[i]);
                }
            }
            

            return this;
        }

        public GameStatsSnapshot SetLocalEmpireIndex(int localEmpireIndex)
        {
            LocalEmpireIndex = localEmpireIndex;

            return this;
        }
        
    }

    public class EmpireSnapshot : IEmpireSnapshotDataType
    {
        public string[] Values;
        private string primaryColor = "#FFFFFFFF";
        private Color color = Color.white;
        private string secondaryColor = "#000000FF";
        private Color contrastColor = Color.black;
        public string PrimaryColor => primaryColor;
        public Color Color => color;
        public string SecondaryColor => secondaryColor;
        public Color ContrastColor => contrastColor;
        public string UserName = string.Empty;

        public EmpireSnapshot(HumankindEmpire empire)
        {
            Snapshot(empire);
        }

        public EmpireSnapshot Snapshot(HumankindEmpire empire)
        {
            Values = EmpireSnapshotUtils.MakeEmpireSnapshotValues(empire);
            
            if (R.Text.NormalizeColor(empire.PrimaryColor) != primaryColor)
            {
                primaryColor = R.Text.NormalizeColor(empire.PrimaryColor);
                if (ColorUtility.TryParseHtmlString(primaryColor, out Color parsedColor))
                {
                    color = parsedColor;
                    
                    if (hasBetterContrastWithBlackColor((Color32)parsedColor))
                    {
                        secondaryColor = "#000000FF";
                        contrastColor = Color.black;
                    }
                    else
                    {
                        secondaryColor = "#FFFFFFFF";
                        contrastColor = Color.white;
                    }
                }
            }

            UserName = GetPlayerIdentifierUserName(empire) ?? string.Empty;
            if (UserName.Length == 0)
                UserName = PersonaName;

            return this;
        }


        public bool hasBetterContrastWithBlackColor(Color32 bg)
        {
            int nThreshold = 105;
            int bgDelta = Convert.ToInt32((bg.r * 0.299) + (bg.g * 0.587) + (bg.b * 0.114));

            return (255 - bgDelta < nThreshold);
        }

        private static FieldInfo PlayerIdentifierField = R.GetField<Amplitude.Mercury.Simulation.Empire>("PlayerIdentifier", R.NonPublicInstance);
        private static string GetPlayerIdentifierUserName(HumankindEmpire empire)
        {
            PlayerIdentifier playerIdentifier = (PlayerIdentifier)PlayerIdentifierField.GetValue((Amplitude.Mercury.Simulation.Empire)empire.Simulation);
            
            return playerIdentifier.UserName;
        }

        public string EmpireIndex => Values[0];
        public string PersonaName => Values[1];
        public string TerritoryCount => Values[2];
        public string CityCount => Values[3];
        public string CityCap => Values[4];
        public string Stability => Values[5];
        public string EraLevel => Values[6];
        public string SumOfEraStars => Values[7];
        public string SettlementsPopulation => Values[8];
        public string EmpirePopulation => Values[9];
        public string CombatStrength => Values[10];

        public string ArmyCount => Values[11];
        public string UnitCount => Values[12];
        public string MilitaryUpkeep => Values[13];
        public string CompletedTechnologiesCount => Values[14];
        public string ResearchNet => Values[15];
        public string TechnologicalEraOffset => Values[16];
        public string AvailableTechnologiesCount => Values[17];
        public string UnlockedTechnologiesCount => Values[18];

        public string MoneyStock => Values[19];
        public string MoneyNet => Values[20];
        public string InfluenceStock => Values[21];
        public string InfluenceNet => Values[22];
        public string LuxuryResourcesAccessCount => Values[23];
        public string StrategicResourcesAccessCount => Values[24];
        public string TradeNodesCount => Values[25];
    }

    public static class EmpireSnapshotUtils
    {
        public static string[] MakeEmpireSnapshotValues(HumankindEmpire empire)
        {
            return new[] {
                $"{empire.EmpireIndex}",
                $"{empire.PersonaName}",
                $"{empire.TerritoryCount}",
                $"{empire.CityCount}",
                $"{empire.CityCap}",
                $"{empire.Stability}",
                $"{empire.EraLevel}",
                $"{empire.SumOfEraStars}",
                $"{empire.SettlementsPopulation}",
                $"{empire.EmpirePopulation}",
                $"{empire.CombatStrength}",

                $"{empire.ArmyCount}",
                $"{empire.UnitCount}",
                $"{empire.MilitaryUpkeep}",
                $"{empire.CompletedTechnologiesCount}",
                $"{empire.ResearchNet}",
                $"{empire.TechnologicalEraOffset}",
                $"{empire.AvailableTechnologiesCount}",
                $"{empire.UnlockedTechnologiesCount}",

                $"{empire.MoneyStock}",
                empire.MoneyNet > 0 ? $"+{empire.MoneyNet}" : $"{empire.MoneyNet}",
                $"{empire.InfluenceStock}",
                $"{empire.InfluenceNet}",
                $"{empire.LuxuryResourcesAccessCount}",
                $"{empire.StrategicResourcesAccessCount}",
                $"{empire.TradeNodesCount}",
            };
        }
    }
}
