using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using Amplitude.Framework.Overlay;
using DevTools.Humankind.GUITools.UI.BuiltIn;

namespace DevTools.Humankind.GUITools.UI
{
    public class MainToolbar : FloatingToolWindow
    {
        public override bool ShouldBeVisible => HumankindGame.IsGameLoaded;
        public override bool ShouldRestoreLastWindowPosition => true;
        public override string WindowTitle { get; set; } = "TOOLBAR";
        public override Rect WindowRect { get; set; } = new Rect (30, 290, 170, 600);
        
        #region FloatingToolWindow derived classes

        public AutoTurnToolWindow AutoTurnTool { get; set; } = null;
        public TechnologyToolsWindow TechnologyTool { get; set; } = null;
        public MilitaryToolsWindow MilitaryTool { get; set; } = null;
        public ResourceToolsWindow ResourceTool { get; set; } = null;
        
        public FramerateToolWindow FramerateTool { get; set; } = null;

        #endregion FloatingToolWindow derived classes

        public override void OnDrawUI()
        {
            GUILayout.BeginVertical((GUIStyle) "PopupWindow.Sidebar.Highlight"); 
        
                GUILayout.Label("<color=#000000AA>C H E A T I N G</color>   T O O L S", "PopupWindow.Sidebar.Heading");
                

                if (DrawItem<MilitaryToolsWindow>(MilitaryTool, "Military"))
                    Open<MilitaryToolsWindow>(window => MilitaryTool = window);

                if (DrawItem<TechnologyToolsWindow>(TechnologyTool, "Technology"))
                    Open<TechnologyToolsWindow>(window => TechnologyTool = window);

                if (DrawItem<ResourceToolsWindow>(ResourceTool, "Resources"))
                    Open<ResourceToolsWindow>(window => ResourceTool = window);
                
                OnDrawTool<AffinityUtilsWindow>("Cultural Affinity");
                
                GUILayout.Label("<color=#000000AA>P R O F I L I N G</color>   T O O L S", "PopupWindow.Sidebar.Heading");
            
                if (DrawItem<FramerateToolWindow>(FramerateTool, "Framerate"))
                    Open<FramerateToolWindow>(window => FramerateTool = window);
                
                GUILayout.Label("<color=#000000AA>D E V E L O P E R</color>   T O O L S", "PopupWindow.Sidebar.Heading");

                if (DrawItem<AutoTurnToolWindow>(AutoTurnTool, "Auto Turn <color=#00CC00AA>**HOT**</color>"))
                    Open<AutoTurnToolWindow>(window => AutoTurnTool = window);
                    
                OnDrawTool<ArchetypesWindow>("Archetypes");
                
                GUILayout.Label("E X P <color=#000000AA>E R I M E N T A L</color>", "PopupWindow.Sidebar.Heading");

                OnDrawTool<AIWindow>("AI");
                
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

        public override void Close()
        {
            AutoTurnTool?.Close();
            TechnologyTool?.Close();
            MilitaryTool?.Close();
            ResourceTool?.Close();
            base.Close();
        }
    }
}
