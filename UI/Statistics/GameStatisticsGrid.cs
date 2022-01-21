using System;
using System.Linq;
using UnityEngine;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Framework;
using Amplitude.Framework.Networking;
using StyledGUI;

namespace DevTools.Humankind.GUITools.UI
{
    public interface IGameStatisticsGrid : IStyledGrid
    {
        GameStatsSnapshot CurrentSnapshot { get; set; }
        int CurrentIndex { get; set; }
        int CurrentEmpireIndex { get; set; }
        int[] DisplayOrder { get; set; }
        float SpaceEmpireColumsBy { get; set; }
    }
    
    public class GameStatisticsGrid : GridStyles, IGameStatisticsGrid
    {

        public GameStatsSnapshot CurrentSnapshot { get; set; }
        public int[] DisplayOrder { get; set; } = null;
        public float SpaceEmpireColumsBy { get; set; } = 4f;
        public const string PerTurn = "<color=#000000FF><size=8>/</size><size=7> TURN</size></color>";
        public const string ReturnCharacter = "⏎";
        public int CurrentIndex { get; set; }
        public int CurrentEmpireIndex { get; set; }

        public void SetSnapshot(GameStatsSnapshot snapshot)
        {
            CurrentSnapshot = snapshot;
            if (DisplayOrder == null || DisplayOrder.Length != snapshot.Empires.Length)
                DisplayOrder = snapshot.Empires.Select((e, i) => i).ToArray();
        }
        
        public void DrawCommonHeader()
        {
            this.Row(Styles.StaticRowStyle);
            this.RowHeader(" ", CellSpan6);

            this.RowHeader("TURN", CellSpan1);
            DrawCell(CurrentSnapshot.Turn.ToString(), CellSpan1);
            
            this.RowHeader("ATMOSPHERE POLLUTION", CellSpan5);
            DrawCell("<size=10>LEVEL " + CurrentSnapshot.AtmospherePollutionLevel + "</size>", CellSpan2);
            DrawCell("" + 
                     "<b>" + CurrentSnapshot.AtmospherePollutionStock + "</b>" + 
                     (CurrentSnapshot.AtmospherePollutionNet >= 0 ? "  ( +" : "  ( ") + 
                     CurrentSnapshot.AtmospherePollutionNet + " " + PerTurn + " )", 
                CellSpan4);
            CellButton("<size=10>-2K</size>", () =>
            {
                SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
                {
                    Delta = -2000
                }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex);
            }, CellSpan1);
            CellButton("<size=10>-250</size>", () =>
            {
                SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
                {
                    Delta = -250
                }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex);
            }, CellSpan1);
            CellButton("<size=10>+250</size>", () =>
            {
                SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
                {
                    Delta = 250
                }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex);
            }, CellSpan1);
            CellButton("<size=10>+2K</size>", () =>
            {
                SandboxManager.PostOrder((Order)new EditorOrderAddOrRemoveAtmospherePollution()
                {
                    Delta = 2000
                }, (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex);
            }, CellSpan1);
            
            this.EndRow();
        }

        public void Draw()
        {
            DrawRow(" ", Styles.StaticRowStyle, (empire, index, empireIndex) =>
            {
                var headerText = empireIndex == CurrentSnapshot.LocalEmpireIndex ?
                    "<size=16><color=#FFFFFFFF><b>A C T I V E</b></color></size>" : " ";
                DrawCell(headerText, Styles.CenteredStaticRowStyle, Color.white, CellSpan4);
            });
            DrawRow(" ", Styles.StaticRowStyle, (empire, index, empireIndex) =>
            {
                GUI.backgroundColor = empire.Color;
                DrawCell("<size=10><color=" + empire.SecondaryColor + "><b>" + empire.UserName + "</b></color></size>", Styles.ColorableCellStyle, empire.Color, CellSpan4);
                GUI.backgroundColor = Color.white;
            });

            this.Space(4f);

            DrawRow("TERRITORIES, CITIES / MAX", empire =>
                DrawCell(empire.TerritoryCount, CellSpan1)
                .DrawCell(empire.CityCount + " / " + empire.CityCap, CellSpan3));
            DrawRow("STABILITY", empire => DrawCompositeCell(empire.Stability + " ", Utils.StabilityTexture, null, CellSpan4));
            DrawRow("ERA LEVEL, SUM OF ERA STARS", empire =>
                DrawCell(empire.EraLevel, CellSpan1)
                .DrawCell(empire.SumOfEraStars, CellSpan3));
            DrawRow("CITIZENS, EMPIRE POPULATION", empire =>
                DrawCell(empire.SettlementsPopulation, CellSpan2)
                .DrawCell(empire.EmpirePopulation, CellSpan2));
            DrawRow("POLLUTION", empire =>
                DrawCell("" + empire.PollutionStock, CellSpan2)
                .DrawCell((int.Parse(empire.PollutionNet) >= 0 ? "+" : "") + 
                              empire.PollutionNet + " " + PerTurn, CellSpan2));

            DrawSection("<size=12><b>MILITARY STATS</b></size>");
            DrawRow("COMBAT STRENGTH", empire => DrawCell(empire.CombatStrength, CellSpan4));
            DrawRow("ARMIES, TOTAL UNITS", empire =>
                DrawCell(empire.ArmyCount, CellSpan2)
                .DrawCell(empire.UnitCount, CellSpan2));
            DrawRow("MILITARY UPKEEP", empire => DrawCompositeCell(empire.MilitaryUpkeep + " ", Utils.MoneyTexture, PerTurn, CellSpan4));

            DrawSection("<size=12><b>RESEARCH STATS</b></size>");
            DrawRow("SCIENCE", empire => DrawCompositeCell("+" + empire.ResearchNet + " ", Utils.ScienceTexture, PerTurn, CellSpan3)
                                        .CellButton<EmpireSnapshot>("<size=10>+5K</size>", (e, i, empireIndex) =>
                                        {
                                            HumankindGame.Empires[empireIndex].ResearchStock = 5000;
                                        }, CellSpan1));
            DrawRow("AVAILABLE, UNLOCKED", empire =>
                DrawCell(empire.AvailableTechnologiesCount, CellSpan2)
                .DrawCell(empire.UnlockedTechnologiesCount, CellSpan2));
            DrawRow("TECHNOLOGICAL ERA OFFSET", empire => DrawCell(empire.TechnologicalEraOffset, CellSpan4));

            DrawSection("<size=12><b>ECONOMIC STATS</b></size>");
            DrawRow("MONEY", empire => DrawCompositeCell(empire.MoneyNet + " ", Utils.MoneyTexture, PerTurn, CellSpan4));
            DrawRow("MONEY STOCK", empire => {
                this.DrawCompositeCell(empire.MoneyStock + " ", Utils.MoneyTexture, null, CellSpan2);
                this.CellButton<EmpireSnapshot>("<size=10>-500</size>", (e, i, empireIndex) =>
                {
                    HumankindGame.Empires[empireIndex].MoneyStock -= 500;
                }, CellSpan1);
                this.CellButton<EmpireSnapshot>("<size=10>+5K</size>", (e, i, empireIndex) =>
                {
                    HumankindGame.Empires[empireIndex].MoneyStock += 5000;
                }, CellSpan1);
            });
            DrawRow("INFLUENCE", empire => DrawCompositeCell("+" + empire.InfluenceNet + " ", Utils.InfluenceTexture, PerTurn, CellSpan4));
            DrawRow("INFLUENCE STOCK", empire => {
                this.DrawCompositeCell(empire.InfluenceStock + " ", Utils.InfluenceTexture, null, CellSpan2);
                this.CellButton<EmpireSnapshot>("<size=10>-75</size>", (e, i, empireIndex) =>
                {
                    HumankindGame.Empires[empireIndex].InfluenceStock -= 75;
                }, CellSpan1);
                this.CellButton<EmpireSnapshot>("<size=10>+250</size>", (e, i, empireIndex) =>
                {
                    HumankindGame.Empires[empireIndex].InfluenceStock += 250;
                }, CellSpan1);
            });
            DrawRow("TOTAL TRADE NODES", empire => DrawCell(empire.TradeNodesCount, CellSpan4));
            DrawRow("LUXURY, STRATEGIC ACCESS COUNT", empire =>
                DrawCell(empire.LuxuryResourcesAccessCount, CellSpan2)
                .DrawCell(empire.StrategicResourcesAccessCount, CellSpan2));

            this.Space(4f);
            DrawRow(" ", Styles.StaticRowStyle, (empire, index, empireIndex) =>
            {
                GUI.enabled = (empireIndex != CurrentSnapshot.LocalEmpireIndex);
                this.CellButton<EmpireSnapshot>("<size=10>SWITCH EMPIRE</size>", (e, i, eIndex) => 
                {
                    try
                    {
                        Services.GetService<INetworkingService>()?.CreateMessageSender().SendLocalMessage(
                            (LocalMessage) new SandboxControlMessage(
                                (ISandboxControlInstruction) new ChangeLocalEmpireInstruction(eIndex)
                            )
                        );
                        HumankindGame.Empires.Where(emp => emp.EmpireIndex == eIndex).Settlements().IsCapital().First()
                            .CenterToCamera();
                    }
                    catch (Exception)
                    {
                        HumankindGame.Empires.Where(emp => emp.EmpireIndex == eIndex).Armies().First().CenterToCamera();
                    }
                }, CellSpan4);
                GUI.enabled = true;
            });

        }

        public void CellButton(string text, Action action, params GUILayoutOption[] options)
        {
            var prevBgTint = GUI.backgroundColor;
            // GUI.backgroundColor = CellButtonTintColor; 
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(text, Styles.CellButtonStyle, options))
            {
                action.Invoke();
                HumankindGame.Update();
                // CurrentSnapshot.Snapshot();
                GameStatsWindow.ResetLoop();
            }
            GUI.backgroundColor = prevBgTint;
        }
        
        public GameStatisticsGrid DrawSection(string title)
        {
            return (GameStatisticsGrid)this
                .Space(16f)
                .Row(Styles.StaticRowStyle)
                .VerticalStack()
                .RowHeader(title, CellSpan6)
                .DrawHorizontalLine(0.5f, CellWidth * 6)
                .EndVerticalStack()
                .EndRow();
        }

        public GameStatisticsGrid DrawRow(string title, GUIStyle style, Action<EmpireSnapshot, int, int> action)
        {
            this.Row(style);
            this.RowHeader(title, CellSpan6);
            this.Iterate<EmpireSnapshot>(action);
            this.EndRow();

            return this;
        }

        public GameStatisticsGrid DrawRow(string title, Action<EmpireSnapshot> action)
        {
            this.Row();
            this.RowHeader(title, CellSpan6);
            this.Iterate<EmpireSnapshot>(action);
            this.EndRow();

            return this;
        }

        public GameStatisticsGrid DrawCell(string text, params GUILayoutOption[] options)
        {
            return (GameStatisticsGrid)this
                .Cell("<size=11>" + text + "</size>", CurrentIndex % 2 != 0 ? CellTintColorAlt : CellTintColor, options);
        }

        public GameStatisticsGrid DrawCell(string text, GUIStyle style, Color tintColor, params GUILayoutOption[] options)
        {
            return (GameStatisticsGrid)this
                .Cell("<size=11>" + text + "</size>", style, tintColor, options);
        }


        public GameStatisticsGrid DrawCompositeCell(string text, Texture image, string rest = null, params GUILayoutOption[] options)
        {
            var prevTint = GUI.backgroundColor;
            GUI.backgroundColor = CurrentIndex % 2 != 0 ? CellTintColorAlt : CellTintColor;

            GUILayout.BeginHorizontal(Styles.CellStyle, options);
            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=11>" + text + "</size>", Styles.InlineCellContentStyle);
            GUI.DrawTexture(GUILayoutUtility.GetRect(14f, 14f),
                image, ScaleMode.StretchToFill, true,
                1f, IconTintColor, 0, 0);
            if (rest != null)
                GUILayout.Label("<size=11>" + rest + "</size>", Styles.InlineCellContentStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.backgroundColor = prevTint;

            return this;
        }





    }
}
