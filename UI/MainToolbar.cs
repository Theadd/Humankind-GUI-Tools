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

        #endregion FloatingToolWindow derived classes

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
        
        }

        public override void OnDrawUI()
        {
            GUILayout.BeginVertical((GUIStyle) "PopupWindow.Sidebar.Highlight"); 
        
                if (GlobalSettings.CheatingTools.Value)
                {
                    GUILayout.Label("<color=#000000AA>C H E A T I N G</color>   T O O L S",
                        "PopupWindow.Sidebar.Heading");

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

                    // OnDrawTool<AffinityUtilsWindow>("Cultural Affinity");
                }
                
                if (GlobalSettings.DeveloperTools.Value)
                {
                    GUILayout.Label("<color=#000000AA>D E V E L O P E R</color>   T O O L S",
                        "PopupWindow.Sidebar.Heading");

                    if (GlobalSettings.AutoTurnTool.Value)
                        if (DrawItem<AutoTurnToolWindow>(AutoTurnTool, "Auto Turn <color=#00CC00AA>**HOT**</color>"))
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

                    // OnDrawTool<ArchetypesWindow>("Archetypes");
                }

                if (GlobalSettings.ProfilingTools.Value)
                {
                    GUILayout.Label("<color=#000000AA>P R O F I L I N G</color>   T O O L S",
                        "PopupWindow.Sidebar.Heading");
                    
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
                
                if (GlobalSettings.ExperimentalTools.Value)
                {
                    GUILayout.Label("E X P <color=#000000AA>E R I M E N T A L</color>", "PopupWindow.Sidebar.Heading");

                    if (GlobalSettings.AITool.Value)
                        if (DrawItem<AIToolWindow>(AITool, "AI Tools"))
                            Open<AIToolWindow>(window => AITool = window);
                    
                    if (GlobalSettings.BattleAITool.Value)
                        if (DrawItem<BattleAIToolWindow>(BattleAITool, "Battle AI Tool"))
                            Open<BattleAIToolWindow>(window => BattleAITool = window);
                    
                }
            GUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

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
            // var window = UIManager.GetWindow<T>(false);
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
            var window = UIManager.GetWindow<T>(false);
            var created = window != null;
            var visible = created && window.IsVisible;

            GUILayout.BeginHorizontal();
                // The close tool button (Object.Destroy)
                GUI.enabled = created;
                GUI.color = created ? Color.white : Color.clear;
                if (GUILayout.Button("x", "PopupWindow.Sidebar.InlineButton", new GUILayoutOption[] {
                    GUILayout.Width(28f)
                })) {
                    UIManager.CloseWindow<T>();
                }
                GUI.enabled = true;
                GUI.color = Color.white;

                // The show/hide tool button toggle
                var shouldBeVisible = (GUILayout.Toggle(visible, tool.ToUpper(), "PopupWindow.Sidebar.Toggle"));
                if (visible != shouldBeVisible)
                {
                    UIManager.ShowWindow<T>(shouldBeVisible);
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
            base.Close(false);
        }
    }
}
