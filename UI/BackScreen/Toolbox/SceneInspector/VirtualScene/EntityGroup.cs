using System.Collections.Generic;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using StyledGUI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{

    public static class EntityGroupEx
    {
        public static void RenderContent(this EntityGroup self)
        {
            GUILayout.BeginVertical();
            {
                if (self.Groups.Count > 0)
                    foreach (var group in self.Groups)
                        group.Render();
                            
                if (self.Entities.Count > 0)
                    foreach (var entity in self.Entities)
                        entity.Render();
            }
            GUILayout.EndVertical();
        }
        
        public static void Render(this EntityGroup self)
        {
            // var title = "" + (self.Collapsed ? "[+] " : "[-] ") + self.Name;
            var title = "" + (self.Collapsed ? "▷ " : "▼ ") + self.Name;
            
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

        public static void Render(this VirtualSceneEntity self)
        {
            if (GUILayout.Button(self.Name, "Link"))
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
