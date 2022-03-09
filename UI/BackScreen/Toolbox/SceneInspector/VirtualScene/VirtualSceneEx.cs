using System;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.UI;
using Amplitude.UI;
using Amplitude.UI.Interactables;
using Amplitude.UI.Renderers;
using Modding.Humankind.DevTools;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{

    public static class VirtualSceneEx
    {
        private static int _depth = 0;

        public static void RenderTree(this VirtualGameObject self, IVirtualSceneRenderer renderer)
        {
            var prev = _depth;
            _depth = 0;
            GUILayout.BeginVertical();
            self.RenderContent(renderer);
            GUILayout.EndVertical();
            _depth = prev;
        }
        
        public static void RenderContent(this VirtualGameObject self, IVirtualSceneRenderer renderer)
        {
            GUILayout.BeginVertical();
            {
                if (self.Components.Count > 0)
                    foreach (var entity in self.Components)
                    {
                        entity.Render();
                        if (renderer.CaptureOnMouseHover)
                        {
                            var r = GUILayoutTree.LastItemRect;
                            if (Event.current.mousePosition.y < r.y + r.height && r.Contains(Event.current.mousePosition))
                                renderer.OnMouseHoverComponent(entity);
                        }
                    }
                
                if (self.Children.Count > 0)
                    foreach (var group in self.Children)
                        group.Render(renderer);
            }
            GUILayout.EndVertical();
        }
        
        public static void Render(this VirtualGameObject self, IVirtualSceneRenderer renderer)
        {
            var wasCollapsed = self.Collapsed;
            
            GUILayoutTree.BeginRow(_depth);
            {
                GUILayout.Label(self.Collapsed ? "" : "", Styles.UnicodeIconStyle, GUILayout.Height(21f), GUILayout.Width(22f), GUILayout.ExpandWidth(false));
                GUILayout.Label("<size=15><color=#efe5b0></color></size>", Styles.UnicodeIconStyle, GUILayout.Height(25f), GUILayout.Width(22f), GUILayout.ExpandWidth(false));
                // GUILayout.BeginHorizontal("<color=#e2c08d>" + self.Name + "</color>", Styles.TreeInlineTextStyle, GUILayout.Height(25f), GUILayout.ExpandWidth(false));
                GUILayout.BeginHorizontal($"<color={Colors.White}>" + self.Name + "</color>", Styles.TreeInlineTextStyle, GUILayout.Height(25f), GUILayout.ExpandWidth(false));
                // GUILayout.Space(8f);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            if (GUILayoutTree.EndRow())
                self.Collapsed = !self.Collapsed;
            
            if (!self.Collapsed && !wasCollapsed)
            {
                _depth++;
                self.RenderContent(renderer);
                _depth--;
            }
        }

        public static void Render(this VirtualComponent self)
        {
            GUILayoutTree.BeginItem(_depth);
            {
                // GUILayout.Label("❖", Styles.UnicodeIconStyle, GUILayout.Height(25f), GUILayout.Width(25f), GUILayout.ExpandWidth(false));
                // GUILayout.Space(8f);
                GUILayout.BeginHorizontal($"<color={Colors.Aquamarine}>" + self.TypeName + "</color>", Styles.TreeInlineTextStyle, GUILayout.Height(25f), GUILayout.ExpandWidth(false));
                // GUILayout.Space(38f);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                // BEGIN RIGHT SIDE BUTTONS
                if (GUILayout.Button("", Styles.InlineUnicodeButtonStyle, GUILayout.Width(25f), GUILayout.Height(25f),
                    GUILayout.ExpandWidth(false)))
                {
                    Loggr.Log(self.Instance);
                    
                    Loggr.Log("INITIALIZING UniverseLib...", ConsoleColor.DarkMagenta);
                    UniverseLib.Universe.Init();
                    Loggr.Log("UniverseLib initialized.", ConsoleColor.Green);
                    
                    UniverseLib.Utility.MiscUtility.Inspect(self.Instance);
                }
                
                if (self.Instance is UITransform uiTransform)
                {
                    var shouldBeVisible = GUILayout.Toggle(uiTransform.VisibleSelf, "☀",
                        Styles.InlineUnicodeButtonStyle, GUILayout.Width(25f), GUILayout.Height(25f),
                        GUILayout.ExpandWidth(false));

                    if (shouldBeVisible != uiTransform.VisibleSelf)
                        uiTransform.VisibleSelf = shouldBeVisible;
                }
                else
                {
                    //  (Toggle value of (MonoBehaviour).enabled)
                    var shouldBeEnabled = GUILayout.Toggle(self.Instance.enabled, "",
                        Styles.InlineUnicodeButtonStyle, GUILayout.Width(25f), GUILayout.Height(25f),
                        GUILayout.ExpandWidth(false));

                    if (shouldBeEnabled != self.Instance.enabled)
                        self.Instance.enabled = shouldBeEnabled;
                }
                // END RIGHT SIDE BUTTONS

            }
            if (GUILayoutTree.EndItem())
            {
                Loggr.Log(self.Instance);

                if (self.Instance is PresentationEntity entity)
                {
                    HumankindGame.CenterCameraAt(entity.WorldPosition.ToTileIndex());
                }
                else if (self.Instance is PresentationCursorTarget cursorTarget)
                {
                    HumankindGame.CenterCameraAt(cursorTarget.PresentationEntity.WorldPosition.ToTileIndex());
                }
            }

            
        }
        
    }
}
