using System.Collections.Generic;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{

    public static class VirtualSceneEx
    {
        public static void RenderContent(this VirtualGameObject self)
        {
            GUILayout.BeginVertical();
            {
                if (self.Children.Count > 0)
                    foreach (var group in self.Children)
                        group.Render();
                            
                if (self.Components.Count > 0)
                    foreach (var entity in self.Components)
                        entity.Render();
            }
            GUILayout.EndVertical();
        }
        
        public static void Render(this VirtualGameObject self)
        {
            // var title = "" + (self.Collapsed ? "[+] " : "[-] ") + self.Name;
            var title = "" + (self.Collapsed ? " ⮚  " : " ⮟  ") + "" + self.Name + "";
            
            GUILayout.BeginVertical();
            {
                self.Collapsed = GUILayout.Toggle(self.Collapsed, title, Styles.CollapsibleSectionToggleStyle);
                if (!self.Collapsed)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(15f);
                        self.RenderContent();
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        public static void Render(this VirtualComponent self)
        {
            if (GUILayout.Button("<color=#FFFFFF00> ⮚  </color>" 
                                 + self.Name 
                                 + " <color=#5588FEF0>" 
                                 + self.TypeName 
                                 + "</color>", 
                Styles.CollapsibleSectionToggleStyle))
            {
                Loggr.LogAll(self.Instance);

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
