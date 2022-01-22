using System;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using Amplitude.Framework.Overlay;

namespace DevTools.Humankind.GUITools.UI
{
    public class MainToolbar : FloatingToolWindow
    {
        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools && 
            HumankindGame.IsGameLoaded && !GlobalSettings.HideToolbarWindow.Value;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "TOOLBAR";
        public override Rect WindowRect { get; set; } = new Rect (30, 290, 170, 600);
        public static Color ActiveToolBackgroundTint { get; set; } = new Color(0.8f, 0.7f, 0.9f, 0.7f);
        
        #region FloatingToolWindow derived classes

        public AutoTurnToolWindow AutoTurnTool { get; set; } = null;
        public TechnologyToolsWindow TechnologyTool { get; set; } = null;
        public MilitaryToolsWindow MilitaryTool { get; set; } = null;
        public ResourceToolsWindow ResourceTool { get; set; } = null;
        public FramerateToolWindow FramerateTool { get; set; } = null;
        // public ProfilerToolWindow ProfilerTool { get; set; } = null;
        // public GPUProfilerToolWindow GPUProfilerTool { get; set; } = null;
        public MemoryProfilerToolWindow MemoryProfilerTool { get; set; } = null;
        public GraphicsToolsWindow GraphicsTool { get; set; } = null;
        public ArmyToolsWindow ArmyTools { get; set; } = null;
        public AIToolWindow AITool { get; set; } = null;
        public BattleAIToolWindow BattleAITool { get; set; } = null;
        public CivicsToolWindow CivicsTool { get; set; } = null;
        public CollectiblesToolWindow CollectiblesTool { get; set; } = null;
        public DiplomacyToolWindow DiplomacyTool { get; set; } = null;
        public TerrainPickingToolWindow TerrainPickingTool { get; set; }
        public GameInfoToolWindow GameInfoTool { get; set; }
        public DistrictPainterToolWindow DistrictPainterTool { get; set; }
        public SettlementToolsWindow SettlementTools { get; set; }
        public StatisticsAndAchievementsToolWindow StatisticsAndAchievementsTool { get; set; }
        public FameToolWindow FameTool { get; set; }
        public EndGameToolWindow EndGameTool { get; set; }

        #endregion FloatingToolWindow derived classes

        public HexOverlay HexPainter { get; set; } = new HexOverlay();

        public void RestoreVisibleWindows()
        {
            if (GlobalSettings.AutoTurnTool.Value && WasVisible<AutoTurnToolWindow>()) Open<AutoTurnToolWindow>(window => AutoTurnTool = window);
            if (GlobalSettings.TechnologyTool.Value && WasVisible<TechnologyToolsWindow>()) Open<TechnologyToolsWindow>(window => TechnologyTool = window);
            if (GlobalSettings.MilitaryTool.Value && WasVisible<MilitaryToolsWindow>()) Open<MilitaryToolsWindow>(window => MilitaryTool = window);
            if (GlobalSettings.ResourcesTool.Value && WasVisible<ResourceToolsWindow>()) Open<ResourceToolsWindow>(window => ResourceTool = window);
            if (GlobalSettings.FramerateTool.Value && WasVisible<FramerateToolWindow>()) Open<FramerateToolWindow>(window => FramerateTool = window);
            if (GlobalSettings.MemoryProfilerTool.Value && WasVisible<MemoryProfilerToolWindow>()) Open<MemoryProfilerToolWindow>(window => MemoryProfilerTool = window);
            if (GlobalSettings.GraphicsTool.Value && WasVisible<GraphicsToolsWindow>()) Open<GraphicsToolsWindow>(window => GraphicsTool = window);
            if (GlobalSettings.ArmyTool.Value && WasVisible<ArmyToolsWindow>()) Open<ArmyToolsWindow>(window => ArmyTools = window);
            if (GlobalSettings.AITool.Value && WasVisible<AIToolWindow>()) Open<AIToolWindow>(window => AITool = window);
            if (GlobalSettings.BattleAITool.Value && WasVisible<BattleAIToolWindow>()) Open<BattleAIToolWindow>(window => BattleAITool = window);
            if (GlobalSettings.CivicsTool.Value && WasVisible<CivicsToolWindow>()) Open<CivicsToolWindow>(window => CivicsTool = window);
            if (GlobalSettings.CollectiblesTool.Value && WasVisible<CollectiblesToolWindow>()) Open<CollectiblesToolWindow>(window => CollectiblesTool = window);
            if (GlobalSettings.DiplomacyTool.Value && WasVisible<DiplomacyToolWindow>()) Open<DiplomacyToolWindow>(window => DiplomacyTool = window);
            if (GlobalSettings.TerrainPickingTool.Value && WasVisible<TerrainPickingToolWindow>()) Open<TerrainPickingToolWindow>(window => TerrainPickingTool = window);
            if (GlobalSettings.GameInfoTool.Value && WasVisible<GameInfoToolWindow>()) Open<GameInfoToolWindow>(window => GameInfoTool = window);
            if (GlobalSettings.DistrictPainterTool.Value && WasVisible<DistrictPainterToolWindow>()) Open<DistrictPainterToolWindow>(window => DistrictPainterTool = window);
            if (GlobalSettings.SettlementTools.Value && WasVisible<SettlementToolsWindow>()) Open<SettlementToolsWindow>(window => SettlementTools = window);
            if (GlobalSettings.StatisticsAndAchievementsTool.Value && WasVisible<StatisticsAndAchievementsToolWindow>()) Open<StatisticsAndAchievementsToolWindow>(window => StatisticsAndAchievementsTool = window);
            if (GlobalSettings.FameTool.Value && WasVisible<FameToolWindow>()) Open<FameToolWindow>(window => FameTool = window);
            if (GlobalSettings.EndGameTool.Value && WasVisible<EndGameToolWindow>()) Open<EndGameToolWindow>(window => EndGameTool = window);
        
        }

        public static Texture2D TintableWhiteTexture { get; set; } =
            Utils.CreateSinglePixelTexture2D(new Color(1f, 1f, 1f, 0.8f));

        public GUIStyle TintableBackgroundStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Row"))
        {
            padding = new RectOffset(0, 0, 0, 0),
            margin = new RectOffset(0, 0, 0, 0),
            border = new RectOffset(0, 0, 0, 0),
            overflow = new RectOffset(0, 0, 0, 0),
            normal = new GUIStyleState() {
                background = TintableWhiteTexture,
                textColor = Color.white
            },
            hover = new GUIStyleState() {
                background = TintableWhiteTexture,
                textColor = Color.white
            },
            active = new GUIStyleState() {
                background = TintableWhiteTexture,
                textColor = Color.white
            },
            onNormal = new GUIStyleState() {
                background = TintableWhiteTexture,
                textColor = Color.white
            }
        };

        public GUIStyle BackgroundContainerStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Sidebar.Heading"))
        {
            margin = new RectOffset(0, 0, 0, 0),
        };

        public static Color BackgroundTintColor { get; set; } = new Color32(205, 196, 174, 244);

        public override void OnDrawUI()
        {
            GUI.backgroundColor = BackgroundTintColor;
            GUILayout.BeginHorizontal(TintableBackgroundStyle);
            
            GUI.backgroundColor = Color.black;
            GUILayout.BeginVertical(BackgroundContainerStyle);

            HexPainter.Draw();    
            
                OnDrawCheatingTools();
                GUI.backgroundColor = Color.black;
                OnDrawDeveloperTools();
                GUI.backgroundColor = Color.black;
                OnDrawProfilingTools();
                GUI.backgroundColor = Color.black;
                OnDrawExperimentalTools();
                
                
            GUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();

        }

        private void OnDrawCheatingTools()
        {
            if (GlobalSettings.CheatingTools.Value)
            {
                GUILayout.Label("<color=#000000AA>C H E A T I N G</color>   T O O L S",
                    "PopupWindow.Sidebar.Heading");

                GUI.backgroundColor = ActiveToolBackgroundTint;
                if (GlobalSettings.MilitaryTool.Value)
                    if (DrawItem<MilitaryToolsWindow>(MilitaryTool, "Military"))
                        Open<MilitaryToolsWindow>(window => MilitaryTool = window);

                if (GlobalSettings.ArmyTool.Value)
                    if (DrawItem<ArmyToolsWindow>(ArmyTools, "Army Tools"))
                        Open<ArmyToolsWindow>(window => ArmyTools = window);

                if (GlobalSettings.TechnologyTool.Value)
                    if (DrawItem<TechnologyToolsWindow>(TechnologyTool, "Technology"))
                        Open<TechnologyToolsWindow>(window => TechnologyTool = window);

                if (GlobalSettings.ResourcesTool.Value)
                    if (DrawItem<ResourceToolsWindow>(ResourceTool, "Resources"))
                        Open<ResourceToolsWindow>(window => ResourceTool = window);
                    
                if (GlobalSettings.StatisticsAndAchievementsTool.Value)
                    if (DrawItem<StatisticsAndAchievementsToolWindow>(StatisticsAndAchievementsTool, "Achievements"))
                        Open<StatisticsAndAchievementsToolWindow>(window => StatisticsAndAchievementsTool = window);

                // OnDrawTool<AffinityUtilsWindow>("Cultural Affinity");
            }
        }

        private void OnDrawDeveloperTools()
        {
            if (GlobalSettings.DeveloperTools.Value)
            {
                GUILayout.Label("<color=#000000AA>D E V E L O P E R</color>   T O O L S",
                    "PopupWindow.Sidebar.Heading");

                GUI.backgroundColor = ActiveToolBackgroundTint;
                if (GlobalSettings.AutoTurnTool.Value)
                    if (DrawItem<AutoTurnToolWindow>(AutoTurnTool, "Auto Turn"))
                        Open<AutoTurnToolWindow>(window => AutoTurnTool = window);

                if (GlobalSettings.CivicsTool.Value)
                    if (DrawItem<CivicsToolWindow>(CivicsTool, "Civics Tool"))
                        Open<CivicsToolWindow>(window => CivicsTool = window);
                    
                if (GlobalSettings.CollectiblesTool.Value)
                    if (DrawItem<CollectiblesToolWindow>(CollectiblesTool, "Collectibles"))
                        Open<CollectiblesToolWindow>(window => CollectiblesTool = window);
                    
                if (GlobalSettings.DiplomacyTool.Value)
                    if (DrawItem<DiplomacyToolWindow>(DiplomacyTool, "Diplomacy"))
                        Open<DiplomacyToolWindow>(window => DiplomacyTool = window);

                if (GlobalSettings.TerrainPickingTool.Value)
                    if (DrawItem<TerrainPickingToolWindow>(TerrainPickingTool, "Terrain Info"))
                        Open<TerrainPickingToolWindow>(window => TerrainPickingTool = window);

                if (GlobalSettings.GameInfoTool.Value)
                    if (DrawItem<GameInfoToolWindow>(GameInfoTool, "Game Info"))
                        Open<GameInfoToolWindow>(window => GameInfoTool = window);

                // OnDrawTool<ArchetypesWindow>("Archetypes");
            }
        }

        private void OnDrawProfilingTools()
        {
            if (GlobalSettings.ProfilingTools.Value)
            {
                GUILayout.Label("<color=#000000AA>P R O F I L I N G</color>   T O O L S",
                    "PopupWindow.Sidebar.Heading");
                
                GUI.backgroundColor = ActiveToolBackgroundTint; 
                if (GlobalSettings.FramerateTool.Value)
                    if (DrawItem<FramerateToolWindow>(FramerateTool, "Framerate"))
                        Open<FramerateToolWindow>(window => FramerateTool = window);

                // if (GlobalSettings.GPUProfilerTool.Value)
                //     if (DrawItem<GPUProfilerToolWindow>(GPUProfilerTool, "GPU Profiler"))
                //         Open<GPUProfilerToolWindow>(window => GPUProfilerTool = window);

                if (GlobalSettings.GraphicsTool.Value)
                    if (DrawItem<GraphicsToolsWindow>(GraphicsTool, "Graphics Tool"))
                        Open<GraphicsToolsWindow>(window => GraphicsTool = window);

                // if (DrawItem<ProfilerToolWindow>(ProfilerTool, "Profiler"))
                //     Open<ProfilerToolWindow>(window => ProfilerTool = window);
                
                if (GlobalSettings.MemoryProfilerTool.Value)
                    if (DrawItem<MemoryProfilerToolWindow>(MemoryProfilerTool, "Memory Profiler"))
                        Open<MemoryProfilerToolWindow>(window => MemoryProfilerTool = window);
            }
        }

        private void OnDrawExperimentalTools()
        {
            if (GlobalSettings.ExperimentalTools.Value)
            {
                GUILayout.Label("E X P <color=#000000AA>E R I M E N T A L</color>", "PopupWindow.Sidebar.Heading");

                GUI.backgroundColor = ActiveToolBackgroundTint;
                if (GlobalSettings.AITool.Value)
                    if (DrawItem<AIToolWindow>(AITool, "AI Tools"))
                        Open<AIToolWindow>(window => AITool = window);
                
                if (GlobalSettings.BattleAITool.Value)
                    if (DrawItem<BattleAIToolWindow>(BattleAITool, "Battle AI Tool"))
                        Open<BattleAIToolWindow>(window => BattleAITool = window);

                if (GlobalSettings.DistrictPainterTool.Value)
                    if (DrawItem<DistrictPainterToolWindow>(DistrictPainterTool, "District Painter"))
                        Open<DistrictPainterToolWindow>(window => DistrictPainterTool = window);
                        
                if (GlobalSettings.SettlementTools.Value)
                    if (DrawItem<SettlementToolsWindow>(SettlementTools, "Settlements"))
                        Open<SettlementToolsWindow>(window => SettlementTools = window);
                        
                if (GlobalSettings.FameTool.Value)
                    if (DrawItem<FameToolWindow>(FameTool, "Fame Tool"))
                        Open<FameToolWindow>(window => FameTool = window);
                        
                if (GlobalSettings.EndGameTool.Value)
                    if (DrawItem<EndGameToolWindow>(EndGameTool, "End Game Tool"))
                        Open<EndGameToolWindow>(window => EndGameTool = window);
                
            }
        }
        
        /// <summary>
        ///     Draws respective buttons to open/close/hide a FloatingToolWindow derived class to the side toolbar.
        /// </summary>
        /// <param name="window">reference to the declared window, which will be null unless it is created.</param>
        /// <param name="displayName">toolbar item display name</param>
        /// <typeparam name="T">FloatingToolWindow derived class</typeparam>
        /// <returns>boolean to indicate that the window needs to be created and shown.</returns>
        protected bool DrawItem<T>(T window, string displayName) where T : FloatingToolWindow
        {
            // var window = UIController.GetWindow<T>(false);
            var created = window != null;
            var visible = created && window.IsVisible;

            GUILayout.BeginHorizontal();
            // The close tool button (Object.Destroy)
            GUI.enabled = created;
            GUI.color = created ? Color.white : Color.clear;
            if (GUILayout.Button("x", "PopupWindow.Sidebar.InlineButton", GUILayout.Width(28f)))
            {
                window.Close();
            }
            GUI.enabled = true;
            GUI.color = Color.white;

            // The show/hide tool button toggle
            var shouldBeVisible = (GUILayout.Toggle(visible, displayName.ToUpper(), "PopupWindow.Sidebar.Toggle"));
            if (created && visible != shouldBeVisible)
                window.ShowWindow(shouldBeVisible);
            
            GUILayout.EndHorizontal();

            return !created && shouldBeVisible;
        }

        protected void OnDrawTool<T>(string tool) where T : PopupWindow
        {
            var window = UIController.GetWindow<T>(false);
            var created = window != null;
            var visible = created && window.IsVisible;

            GUILayout.BeginHorizontal();
                // The close tool button (Object.Destroy)
                GUI.enabled = created;
                GUI.color = created ? Color.white : Color.clear;
                if (GUILayout.Button("x", "PopupWindow.Sidebar.InlineButton", new GUILayoutOption[] {
                    GUILayout.Width(28f)
                })) {
                    UIController.CloseWindow<T>();
                }
                GUI.enabled = true;
                GUI.color = Color.white;

                // The show/hide tool button toggle
                var shouldBeVisible = (GUILayout.Toggle(visible, tool.ToUpper(), "PopupWindow.Sidebar.Toggle"));
                if (visible != shouldBeVisible)
                {
                    UIController.ShowWindow<T>(shouldBeVisible);
                }
            GUILayout.EndHorizontal();
        }

        public override void Close(bool saveVisibilityStateBeforeClosing = false)
        {
            var s = saveVisibilityStateBeforeClosing;

            AutoTurnTool?.Close(s);
            TechnologyTool?.Close(s);
            MilitaryTool?.Close(s);
            ResourceTool?.Close(s);
            FramerateTool?.Close(s);
            MemoryProfilerTool?.Close(s);
            GraphicsTool?.Close(s);
            ArmyTools?.Close(s);
            AITool?.Close(s);
            BattleAITool?.Close(s);
            CivicsTool?.Close(s);
            CollectiblesTool?.Close(s);
            DiplomacyTool?.Close(s);
            TerrainPickingTool?.Close(s);
            base.Close(false);
        }
    }
}
