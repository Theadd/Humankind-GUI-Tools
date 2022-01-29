using System;
using Modding.Humankind.DevTools;
using StyledGUI.VirtualGridElements;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class BackScreenToolbox
    {
        public BackScreenWindow Window { get; set; }
        public ConstructiblesGrid ConstructiblesGrid { get; set; }
        public Vector2 ScrollViewPosition { get; set; } = Vector2.zero;

        public void Draw(Rect targetRect)
        {
            return;
            // GUILayout.Label("HELLO WORLD!");

            GUILayout.BeginArea(targetRect);
            {
                GUILayout.Label("HELLO WORLD!");
                Draw();
            }
            GUILayout.EndArea();
        }

        public void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("HELLO WORLD!");

            ScrollViewPosition = GUILayout.BeginScrollView(
                ScrollViewPosition, 
                false, 
                false, 
                null,
                null,
                null,
                new GUILayoutOption[]
                {
                    GUILayout.Height(300f)
                });
            {
                GUILayout.BeginVertical();
                {
                    if (Event.current.type == EventType.MouseDown)
                        ConstructiblesGrid.VirtualGrid.FindCellAtPosition(Event.current.mousePosition, OnClickHandler);

                    ConstructiblesGrid.Render();

                    GUILayout.Space(8f);
                    GUILayout.Label("FOCUSED CONTROL: " + GUI.GetNameOfFocusedControl());
                    if (GUILayout.Button("TESTME"))
                    {
                        Loggr.Log("EVENT TYPE = " + Event.current.type.ToString());
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            
            GUILayout.EndHorizontal();
        }

        public void OnClickHandler(ICell cell)
        {
            Loggr.Log("FOUND CELL FOR CLICK! CELL TYPE = " + cell.GetType().Name, ConsoleColor.DarkCyan);
            Loggr.Log(cell);
        }

        private static bool HitTest(Rect rect, Event evt)
        {
            var offset = (evt.pointerType == PointerType.Pen || evt.pointerType == PointerType.Touch) ? 3 : 0;
            var point = evt.mousePosition;

            return (double) point.x >= (double) rect.xMin - (double) offset &&
                   (double) point.x < (double) rect.xMax + (double) offset &&
                   (double) point.y >= (double) rect.yMin - (double) offset &&
                   (double) point.y < (double) rect.yMax + (double) offset;
        }
    }
}