using System.Linq;
using StyledGUI;
using StyledGUI.VirtualGridElements;
using System;
using System.Collections.Generic;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Framework;
using Amplitude.Framework.Networking;
using Amplitude.Mercury.Data.Simulation;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    
    public class GameOverviewGrid : GridStyles { }
    
    public class GameGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public GameStatsSnapshot Snapshot { get; set; }
        public bool IsDirty { get; set; } = true;
        public const string PerTurn = "<color=#000000FF><size=8>/</size><size=7> TURN</size></color>";

        protected IStyledGrid Grid;
        protected IEnumerable<EraStarDefinition> EraStarDefinitions;

        private static IElement[] _emptyIElementsArray = new IElement[]
        {
            new TextElement() { Text = " " }
        };
        
        private static Color _inactiveEraStarIconTintColor = new Color(0f, 0f, 0f, 0.2f);
        private static Color _eraStarIconTintColor = new Color(0.2f, 0.5f, 1f, 0.7f);
        private static EraStarInfo _dummyEraStarInfo = new EraStarInfo()
        {
            GameplayOrientation = GameplayOrientation.None,
            Level = 0,
            PoolAllocationIndex = -9999999
        };

        public GameGrid() { }
        
        public void Render()
        {
            if (IsDirty)
                Update();

            VirtualGrid.Render();
        }

        public void Update()
        {
            Grid = VirtualGrid.Grid;

            EraStarDefinitions = Databases.GetDatabase<EraStarDefinition>().GetValues();
            ComputeVirtualGrid();

            IsDirty = false;
        }

        private void ComputeVirtualGrid()
        {
            VirtualGrid.Columns = Snapshot.Empires.Select(empire => new Column()
            {
                Name = empire.UserName
            }).ToArray();

            VirtualGrid.Sections = new[]
            {
                new Section()
                {
                    Rows = new []
                    {
                        new Row() { Cells = Snapshot.Empires.Select(empire => new Cell()
                        {
                            Text = (empire.Index == Snapshot.LocalEmpireIndex) ? 
                                "<size=16><color=#FFFFFFFF><b>A C T I V E</b></color></size>" : " ",
                            Style = Styles.CenteredStaticRowStyle
                        }), Style = Styles.StaticRowStyle },
                        new Row() { Cells = Snapshot.Empires.Select(empire => new TintableCell()
                        {
                            Text = "<size=10><b>" + empire.UserName + "</b></size>",
                            BackgroundColor = empire.Color,
                            Color = empire.SecondaryColor
                        }), Style = Styles.StaticRowStyle }
                    },
                    SpaceBefore = 0
                },
                new Section()
                {
                    SpaceBefore = 4f,
                    Rows = new[]
                    {
                        new Row() { Title = "TERRITORIES, CITIES / MAX", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                    new Cell() { Text = e.TerritoryCount, Span = Grid.CellSpan1},
                                    new Cell() { Text = e.CityCount + " / " + e.CityCap, Span = Grid.CellSpan3},
                        }})},
                        new Row() { Title = "STABILITY", 
                            Cells = Snapshot.Empires.Select(e => new CompositeCell() { Elements = new IElement[] {
                                new TextElement() { Text = e.Stability },
                                new ImageElement() { Image = Utils.StabilityTexture },
                            }})},
                        new Row() { Title = "CITIZENS, EMPIRE POPULATION", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.SettlementsPopulation, Span = Grid.CellSpan2},
                                new Cell() { Text = e.EmpirePopulation, Span = Grid.CellSpan2},
                            }})},
                        new Row() { Title = "POLLUTION", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.PollutionStock, Span = Grid.CellSpan2},
                                new Cell() { Text = (int.Parse(e.PollutionNet) >= 0 ? "+" : "") + 
                                                    e.PollutionNet + " " + PerTurn, Span = Grid.CellSpan2},
                            }})}
                    }
                },
                new Section()
                {
                    Title = "<size=12><b>STARS & FAME</b></size>",
                    Rows = new[]
                    {
                        new Row() { Title = "ERA LEVEL, SUM OF ERA STARS", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.EraLevel, Span = Grid.CellSpan1},
                                new Cell() { Text = e.SumOfEraStars, Span = Grid.CellSpan3},
                            }})},
                        new Row() { Title = "FAME", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new CompositeCell() { Span = Grid.CellSpan2, Elements = new IElement[] {
                                    new TextElement() { Text = e.FameStock },
                                    new ImageElement() { Image = Utils.FaithTexture }}},
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>-25</size>", Action = OnRemove25Fame, Enabled = e.Index == Snapshot.LocalEmpireIndex },
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>+300</size>", Action = OnAdd300Fame, Enabled = e.Index == Snapshot.LocalEmpireIndex }
                            }})},
                    }
                        .Concat(EraStarDefinitions
                            .Where(eraStarDef => !eraStarDef.name.Contains("EraStar_Prehistoric_"))
                            .Select(sd => new Row()
                            {
                                Title = UIController.GetLocalizedTitle(sd.Name, sd.GameplayOrientation.ToString()).ToUpper(),
                                Cells = Snapshot.Empires.Select(e =>
                                {
                                    var starInfo = GetRelatedEraStarInfo(e.Index, sd.GameplayOrientation, out var found);
                                    var sdNumLevels = found 
                                        ? starInfo.EraStarDefinitionName == sd.Name 
                                            ? sd.Levels?.Length ?? 0 
                                            : starInfo.Thresholds?.Length ?? 0
                                        : 0;
                                    
                                    return new CellGroup()
                                    {
                                        Cells = new ICell[]
                                        {
                                            new CompositeCell()
                                            {
                                                Span = Grid.CellSpan3, 
                                                Elements = sdNumLevels > 0 
                                                    ? Enumerable
                                                        .Range(0, sdNumLevels)
                                                        .Select(i => new ImageElement() { 
                                                            Image = Utils.InfluenceTexture, 
                                                            UseCustomTintColor = true,
                                                            CustomTintColor = i < starInfo.Level ? _eraStarIconTintColor : _inactiveEraStarIconTintColor
                                                        }).ToArray<IElement>() 
                                                    : _emptyIElementsArray
                                            },
                                            new ClickableCell()
                                            {
                                                Span = Grid.CellSpan1, Text = "<size=10>+1</size>",
                                                Action = eIndex => OnAddEraStar(eIndex, starInfo),
                                                Enabled = e.Index == Snapshot.LocalEmpireIndex && (starInfo.Level < sdNumLevels)
                                            }
                                        }
                                    };
                                })
                            })
                            //.ToArray()
                        )
                        .ToArray()
                },
                new Section()
                {
                    Title = "<size=12><b>MILITARY STATS</b></size>",
                    Rows = new[]
                    {
                        QuickTextRow("COMBAT STRENGTH", e => e.CombatStrength),
                        new Row() { Title = "ARMIES, TOTAL UNITS", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.ArmyCount, Span = Grid.CellSpan2},
                                new Cell() { Text = e.UnitCount, Span = Grid.CellSpan2},
                            }})},
                        new Row() { Title = "MILITARY UPKEEP", 
                            Cells = Snapshot.Empires.Select(e => new CompositeCell() { Elements = new IElement[] {
                                new TextElement() { Text = e.MilitaryUpkeep },
                                new ImageElement() { Image = Utils.MoneyTexture },
                                new TextElement() { Text = PerTurn },
                            }})},
                    }
                },
                new Section()
                {
                    Title = "<size=12><b>RESEARCH STATS</b></size>",
                    Rows = new[]
                    {
                        new Row() { Title = "SCIENCE", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new CompositeCell() { Span = Grid.CellSpan3, Elements = new IElement[] {
                                    new TextElement() { Text = "+" + e.ResearchNet },
                                    new ImageElement() { Image = Utils.ScienceTexture },
                                    new TextElement() { Text = PerTurn }}},
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>+5K</size>", Action = OnAdd5kResearch }
                            }})},
                        new Row() { Title = "AVAILABLE, UNLOCKED", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.AvailableTechnologiesCount, Span = Grid.CellSpan2},
                                new Cell() { Text = e.UnlockedTechnologiesCount, Span = Grid.CellSpan2},
                            }})},
                        QuickTextRow("TECHNOLOGICAL ERA OFFSET", e => e.TechnologicalEraOffset),
                    }
                },
                new Section()
                {
                    Title = "<size=12><b>ECONOMIC STATS</b></size>",
                    Rows = new[]
                    {
                        new Row() { Title = "MONEY", 
                            Cells = Snapshot.Empires.Select(e => new CompositeCell() { Elements = new IElement[] {
                                new TextElement() { Text = e.MoneyNet },
                                new ImageElement() { Image = Utils.MoneyTexture },
                                new TextElement() { Text = PerTurn },
                            }})},
                        new Row() { Title = "MONEY STOCK", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new CompositeCell() { Span = Grid.CellSpan2, Elements = new IElement[] {
                                    new TextElement() { Text = e.MoneyStock },
                                    new ImageElement() { Image = Utils.MoneyTexture }}},
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>-500</size>", Action = OnRemove500Money },
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>+5K</size>", Action = OnAdd5kMoney }
                            }})},
                        new Row() { Title = "INFLUENCE", 
                            Cells = Snapshot.Empires.Select(e => new CompositeCell() { Elements = new IElement[] {
                                new TextElement() { Text = "+" + e.InfluenceNet },
                                new ImageElement() { Image = Utils.InfluenceTexture },
                                new TextElement() { Text = PerTurn },
                            }})},
                        new Row() { Title = "INFLUENCE STOCK", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new CompositeCell() { Span = Grid.CellSpan2, Elements = new IElement[] {
                                    new TextElement() { Text = e.InfluenceStock },
                                    new ImageElement() { Image = Utils.InfluenceTexture }}},
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>-75</size>", Action = OnRemove75Influence },
                                new ClickableCell() { Span = Grid.CellSpan1, Text = "<size=10>+250</size>", Action = OnAdd250Influence }
                            }})},
                        QuickTextRow("TOTAL TRADE NODES", e => e.TradeNodesCount),
                        new Row() { Title = "LUXURY, STRATEGIC ACCESS COUNT", 
                            Cells = Snapshot.Empires.Select(e => new CellGroup() { Cells = new ICell[] {
                                new Cell() { Text = e.LuxuryResourcesAccessCount, Span = Grid.CellSpan2},
                                new Cell() { Text = e.StrategicResourcesAccessCount, Span = Grid.CellSpan2},
                            }})},
                    }
                },
                new Section()
                {
                    SpaceBefore = 4f,
                    Rows = new[]
                    {
                        new Row() { Cells = Snapshot.Empires.Select(e => new ClickableCell() {
                                Text = "<size=10>SWITCH EMPIRE</size>", 
                                Action = OnSwitchEmpire, 
                                Span = Grid.CellSpan4,
                                Enabled = e.Index != Snapshot.LocalEmpireIndex
                            }
                        ), Style = Styles.StaticRowStyle }
                    }
                }
            };


        }

        private void Trigger(Action action)
        {
            // some actions tracking stuff here
            action.Invoke();
            // Trigger update
            GameStatsWindow.ResetLoop();
        }

        private void OnAdd5kMoney(int empireIndex) => 
            Trigger(() => HumankindGame.Empires[empireIndex].MoneyStock += 5000);

        private void OnRemove500Money(int empireIndex) => 
            Trigger(() => HumankindGame.Empires[empireIndex].MoneyStock -= 500);

        private void OnAdd250Influence(int empireIndex) => 
            Trigger(() => HumankindGame.Empires[empireIndex].InfluenceStock += 250);

        private void OnRemove75Influence(int empireIndex) => 
            Trigger(() => HumankindGame.Empires[empireIndex].InfluenceStock -= 75);

        private void OnAdd5kResearch(int empireIndex) => 
            Trigger(() => HumankindGame.Empires[empireIndex].ResearchStock = 5000);
        
        private void OnAdd300Fame(int empireIndex) => 
            Trigger(() => SandboxManager.PostOrder((Order)new OrderForceGainFame()
            {
                Gain = 300
            }));

        private void OnRemove25Fame(int empireIndex) => 
            Trigger(() => SandboxManager.PostOrder((Order)new OrderForceGainFame()
            {
                Gain = -25
            }));
        
        private void OnAddEraStar(int empireIndex, EraStarInfo starInfo) => 
            Trigger(() => SandboxManager.PostOrder((Order)new OrderForceGainEraStarScore()
            {
                EraStarName = starInfo.EraStarDefinitionName,
                Gain = (int) (starInfo.Thresholds[starInfo.Level] - starInfo.Score)
            }));

        private void OnSwitchEmpire(int empireIndex)
        {
            Trigger(() =>
            {
                try
                {
                    Services.GetService<INetworkingService>()?.CreateMessageSender().SendLocalMessage(
                        (LocalMessage) new SandboxControlMessage(
                            (ISandboxControlInstruction) new ChangeLocalEmpireInstruction(empireIndex)
                        )
                    );
                    HumankindGame.Empires
                        .Where(emp => emp.EmpireIndex == empireIndex)
                        .Settlements()
                        .IsCapital()
                        .First()
                        .CenterToCamera();
                }
                catch (Exception)
                {
                    HumankindGame.Empires
                        .Where(emp => emp.EmpireIndex == empireIndex)
                        .Armies()
                        .First()
                        .CenterToCamera();
                }
            });
        }

        private Row QuickTextRow(string title, Func<EmpireSnapshot, string> builder) =>
            new Row()
            {
                Title = title,
                Cells = Snapshot.Empires.Select(empire => new Cell()
                {
                    Text = builder.Invoke(empire)
                })
            };

        private EraStarInfo GetRelatedEraStarInfo(int empireIndex,
            GameplayOrientation relatedTo, out bool found)
        {
            found = true;
            ArrayWithFrame<EraStarInfo> eraStarInfo1 =
                Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].EraStarInfo;
            var length = eraStarInfo1.Length;
            
            for (var index1 = 0; index1 < length; ++index1)
            {
                var eraStarInfo2 = eraStarInfo1[index1];
                if (eraStarInfo2.PoolAllocationIndex >= 0 && eraStarInfo2.GameplayOrientation == relatedTo)
                {
                    return eraStarInfo2;
                }
            }

            found = false;
            
            return _dummyEraStarInfo;
        }
    }
}
