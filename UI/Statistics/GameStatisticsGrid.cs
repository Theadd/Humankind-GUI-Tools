using System;
using System.Linq;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Framework;
using Amplitude.Framework.Networking;
using Amplitude.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class GameStatisticsGrid : DrawableGrid<EmpireSnapshot>
    {

        public GameStatsSnapshot CurrentSnapshot;
        public int[] DisplayOrder { get; set; } = null;

        public float SpaceEmpireColumsBy = 4f;

        public Color CellTintColor = new Color(0.9f, 0.9f, 0.85f, 1f);
        // public Color CellTintColorAlt = new Color(1f, 1f, 1f, 0.35f);
        public Color CellTintColorAlt = new Color(0.6f, 0.6f, 0.6f, 1f);
        public Color IconTintColor = new Color(1f, 1f, 1f, 0.7f);
        public Color CellButtonTintColor = new Color32(85, 136, 254, 230);

        //  public const string SpiralCharacter = "\u0489";
        // public const string SpiralCharacter = "\u2314";
        // public const string TurnCharacter = "Ⓣ";
        public const string PerTurn = "<color=#000000FF><size=8>/</size><size=7> TURN</size></color>";
        public const string ReturnCharacter = "⏎";
        //public const string SpiralCharacter = "<color=#FFFFFFFF><size=12>⌀</size></color>";

        private int CurrentIndex;
        private int CurrentEmpireIndex;

        public void Draw(GameStatsSnapshot snapshot)
        {
            CurrentSnapshot = snapshot;
            if (DisplayOrder == null || DisplayOrder.Length != snapshot.Empires.Length)
                DisplayOrder = snapshot.Empires.Select((e, i) => i).ToArray();

            DrawRow(" ", StaticRowStyle, (empire, index, empireIndex) =>
            {
                var headerText = empireIndex == CurrentSnapshot.LocalEmpireIndex ?
                    "<size=16><color=#FFFFFFFF><b>A C T I V E</b></color></size>" : " ";
                DrawCell(headerText, CenteredStaticRowStyle, Color.white, CellSpan4);
            });
            DrawRow(" ", StaticRowStyle, (empire, index, empireIndex) =>
            {
                GUI.backgroundColor = empire.Color;
                DrawCell("<size=10><color=" + empire.SecondaryColor + "><b>" + empire.UserName + "</b></color></size>", ColorableCellStyle, empire.Color, CellSpan4);
                GUI.backgroundColor = Color.white;
            });

            Space(4f);

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

            DrawSection("<size=12><b>MILITARY STATS</b></size>");
            DrawRow("COMBAT STRENGTH", empire => DrawCell(empire.CombatStrength, CellSpan4));
            DrawRow("ARMIES, TOTAL UNITS", empire =>
                DrawCell(empire.ArmyCount, CellSpan2)
                .DrawCell(empire.UnitCount, CellSpan2));
            DrawRow("MILITARY UPKEEP", empire => DrawCompositeCell(empire.MilitaryUpkeep + " ", Utils.MoneyTexture, PerTurn, CellSpan4));

            DrawSection("<size=12><b>RESEARCH STATS</b></size>");
            DrawRow("SCIENCE", empire => DrawCompositeCell("+" + empire.ResearchNet + " ", Utils.ScienceTexture, PerTurn, CellSpan3)
                                        .CellButton("<size=10>+5K</size>", (e, i, empireIndex) =>
                                        {
                                            HumankindGame.Empires[empireIndex].ResearchStock = 5000;
                                        }, CellSpan1));
            DrawRow("AVAILABLE, UNLOCKED", empire =>
                DrawCell(empire.AvailableTechnologiesCount, CellSpan2)
                .DrawCell(empire.UnlockedTechnologiesCount, CellSpan2));
            DrawRow("TECHNOLOGICAL ERA OFFSET", empire => DrawCell(empire.TechnologicalEraOffset, CellSpan4));

            DrawSection("<size=12><b>ECONOMIC STATS</b></size>");
            DrawRow("MONEY", empire => DrawCompositeCell(empire.MoneyNet + " ", Utils.MoneyTexture, PerTurn, CellSpan4));
            DrawRow("MONEY STOCK", empire => DrawCompositeCell(empire.MoneyStock + " ", Utils.MoneyTexture, null, CellSpan2)
                                        .CellButton("<size=10>-500</size>", (e, i, empireIndex) =>
                                        {
                                            HumankindGame.Empires[empireIndex].MoneyStock -= 500;
                                        }, CellSpan1)
                                        .CellButton("<size=10>+5K</size>", (e, i, empireIndex) =>
                                        {
                                            HumankindGame.Empires[empireIndex].MoneyStock += 5000;
                                        }, CellSpan1));
            DrawRow("INFLUENCE", empire => DrawCompositeCell("+" + empire.InfluenceNet + " ", Utils.InfluenceTexture, PerTurn, CellSpan4));
            DrawRow("INFLUENCE STOCK", empire => DrawCompositeCell(empire.InfluenceStock + " ", Utils.InfluenceTexture, null, CellSpan2)
                                        .CellButton("<size=10>-75</size>", (e, i, empireIndex) =>
                                        {
                                            HumankindGame.Empires[empireIndex].InfluenceStock -= 75;
                                        }, CellSpan1)
                                        .CellButton("<size=10>+250</size>", (e, i, empireIndex) =>
                                        {
                                            HumankindGame.Empires[empireIndex].InfluenceStock += 250;
                                        }, CellSpan1));
            DrawRow("TOTAL TRADE NODES", empire => DrawCell(empire.TradeNodesCount, CellSpan4));
            DrawRow("LUXURY, STRATEGIC ACCESS COUNT", empire =>
                DrawCell(empire.LuxuryResourcesAccessCount, CellSpan2)
                .DrawCell(empire.StrategicResourcesAccessCount, CellSpan2));

            Space(4f);
            DrawRow(" ", StaticRowStyle, (empire, index, empireIndex) =>
            {
                GUI.enabled = (empireIndex != CurrentSnapshot.LocalEmpireIndex);
                CellButton("<size=10>SWITCH EMPIRE</size>", (e, i, eIndex) => 
                {
                    Services.GetService<INetworkingService>()?.CreateMessageSender().SendLocalMessage(
                        (LocalMessage) new SandboxControlMessage(
                            (ISandboxControlInstruction) new ChangeLocalEmpireInstruction(eIndex)
                        )
                    );
                    HumankindGame.Empires.Where(emp => emp.EmpireIndex == eIndex).Settlements().IsCapital().First().CenterToCamera();
                }, CellSpan4);
                GUI.enabled = true;
            });

        }

        public GameStatisticsGrid DrawSection(string title)
        {
            return (GameStatisticsGrid)this
                .Space(16f)
                .Row(StaticRowStyle)
                .VerticalStack()
                .RowHeader(title, CellSpan6)
                .DrawHorizontalLine(0.5f, CellWidth * 6)
                .EndVerticalStack()
                .EndRow();
        }

        public GameStatisticsGrid DrawColumnHeader(string title)
        {
            // TODO:
            return (GameStatisticsGrid)this
                .Space(16f)
                .Row(StaticRowStyle)
                .VerticalStack()
                .RowHeader(title, CellSpan6)
                .DrawHorizontalLine(0.5f, CellWidth * 6)
                .EndVerticalStack()
                .EndRow();
        }

        public GameStatisticsGrid DrawRow(string title, GUIStyle style, Action<EmpireSnapshot, int, int> action)
        {
            return (GameStatisticsGrid)this
                .Row(style)
                .RowHeader(title, CellSpan6)
                .Iterate(action)
                .EndRow();
        }

        public GameStatisticsGrid DrawRow(string title, Action<EmpireSnapshot, int, int> action)
        {
            return (GameStatisticsGrid)this
                .Row()
                .RowHeader(title, CellSpan6)
                .Iterate(action)
                .EndRow();
        }

        public GameStatisticsGrid DrawRow(string title, Action<EmpireSnapshot> action)
        {
            return (GameStatisticsGrid)this
                .Row()
                .RowHeader(title, CellSpan6)
                .Iterate(action)
                .EndRow();
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

            GUILayout.BeginHorizontal(CellStyle, options);
            GUILayout.FlexibleSpace();
            GUILayout.Label("<size=11>" + text + "</size>", InlineCellContentStyle);
            GUI.DrawTexture(GUILayoutUtility.GetRect(14f, 14f),
                image, ScaleMode.StretchToFill, true,
                1f, IconTintColor, 0, 0);
            if (rest != null)
                GUILayout.Label("<size=11>" + rest + "</size>", InlineCellContentStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.backgroundColor = prevTint;

            return this;
        }

        public override DrawableGrid<EmpireSnapshot> Iterate(Action<EmpireSnapshot> action)
        {
            for (var i = 0; i < DisplayOrder.Length; i++)
            {
                if (SpaceEmpireColumsBy != 0 && i != 0)
                    GUILayout.Space(SpaceEmpireColumsBy);
                CurrentIndex = i;
                CurrentEmpireIndex = DisplayOrder[i];
                action.Invoke((EmpireSnapshot)CurrentSnapshot.Empires[DisplayOrder[i]]);
            }

            return this;
        }

        public override DrawableGrid<EmpireSnapshot> Iterate(Action<EmpireSnapshot, int, int> action)
        {
            for (var i = 0; i < DisplayOrder.Length; i++)
            {
                if (SpaceEmpireColumsBy != 0 && i != 0)
                    GUILayout.Space(SpaceEmpireColumsBy);
                CurrentIndex = i;
                CurrentEmpireIndex = DisplayOrder[i];
                action.Invoke((EmpireSnapshot)CurrentSnapshot.Empires[DisplayOrder[i]], i, CurrentEmpireIndex);
            }

            return this;
        }

        public override DrawableGrid<EmpireSnapshot> CellButton(string text, Action<EmpireSnapshot, int, int> action, params GUILayoutOption[] options)
        {
            var empireIndex = CurrentEmpireIndex;
            var index = CurrentIndex;
            var prevBgTint = GUI.backgroundColor;
            // GUI.backgroundColor = CellButtonTintColor; 
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button(text, CellButtonStyle, options))
            {
                action.Invoke((EmpireSnapshot)CurrentSnapshot.Empires[empireIndex], index, empireIndex);
                HumankindGame.Update();
                // CurrentSnapshot.Snapshot();
                GameStatsWindow.ResetLoop();
            }
            GUI.backgroundColor = prevBgTint;

            return this;
        }

    }
}
