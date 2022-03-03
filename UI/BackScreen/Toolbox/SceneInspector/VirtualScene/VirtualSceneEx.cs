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
                            var r = GUILayoutUtility.GetLastRect();
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
            var title = ""
                        + (self.Collapsed ? "   " : "   ") 
                        + " " 
                        + self.Name 
                        + "";
            
            GUILayout.BeginVertical();
            {
                self.Collapsed = GUILayout.Toggle(self.Collapsed, title, Styles.CollapsibleSectionToggleStyle);
                if (!self.Collapsed)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(15f);
                        self.RenderContent(renderer);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        public static void Render(this VirtualComponent self)
        {
            GUILayout.BeginHorizontal();
            {
                self.RenderMainButton();

                if (self.Instance is UITransform uiTransform)
                {
                    var shouldBeVisible = GUILayout.Toggle(uiTransform.VisibleSelf, "☀",
                        Styles.InlineUnicodeButtonStyle, GUILayout.Width(22f), GUILayout.Height(21f),
                        GUILayout.ExpandWidth(false));

                    if (shouldBeVisible != uiTransform.VisibleSelf)
                        uiTransform.VisibleSelf = shouldBeVisible;
                }
                
                //  (Toggle value of (MonoBehaviour).enabled)
                var shouldBeEnabled = GUILayout.Toggle(self.Instance.enabled, "",
                    Styles.InlineUnicodeButtonStyle, GUILayout.Width(22f), GUILayout.Height(21f),
                    GUILayout.ExpandWidth(false));

                if (shouldBeEnabled != self.Instance.enabled)
                    self.Instance.enabled = shouldBeEnabled;
                
                GUILayout.Space(12f);
            }
            GUILayout.EndHorizontal();
        }
        
        private static void RenderMainButton(this VirtualComponent self)
        {
            if (GUILayout.Button("<color=#FFFFFF00> ⮚  </color>"
                                 + "❖ <color=#5588FEF0>" 
                                 + self.TypeName 
                                 + "</color>", 
                Styles.CollapsibleSectionToggleStyle))
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
