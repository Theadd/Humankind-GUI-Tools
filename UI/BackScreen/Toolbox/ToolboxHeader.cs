using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class ConstructiblesToolbox
    {
        private string[] tabNames =
        {
            "<size=14><b> UNITS </b></size>", 
            "<size=14><b> DISTRICTS </b></size>"
        };
        
        private string[] displayModeNames =
        {
            "<size=10><b>LIST</b></size>", 
            "<size=10><b>GRID</b></size>"
        };
        
        private void DrawToolboxHeader()
        {
            GUILayout.Space(12f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10f);
                
                if (GUILayout.Button("<size=10><b>SELECT NONE</b></size>"))
                    ToolboxController.Toolbox.ConstructiblesGrid.VirtualGrid.Cursor.ClearSelection();
                
                GUILayout.FlexibleSpace();
                
                if (GUILayout.Button("<size=10><b>REFRESH</b></size>"))
                {
                    ConstructibleStore.Rebuild();
                    ConstructiblesGrid.Snapshot = new ConstructibleStoreSnapshot()
                    {
                        Districts = ConstructibleStore.Districts,
                        Units = ConstructibleStore.Units,
                    };
                    ConstructiblesGrid.IsDirty = true;
                }
                
                GUILayout.Space(8f);

                GUI.enabled = ToolboxController.Toolbox.ConstructiblesGrid.GridModeChunkSize > 1 && ToolboxController.IsDisplayModeGrid;
                if (GUILayout.Button("<size=15><b> ＋</b></size>", GUILayout.Width(22f), GUILayout.Height(21f)))
                    ToolboxController.Toolbox.ConstructiblesGrid.GridModeChunkSize -= 1;
                
                GUI.enabled = ToolboxController.Toolbox.ConstructiblesGrid.GridModeChunkSize < 12 && ToolboxController.IsDisplayModeGrid;
                if (GUILayout.Button("<size=13><b>—</b></size>", GUILayout.Width(22f), GUILayout.Height(21f)))
                    ToolboxController.Toolbox.ConstructiblesGrid.GridModeChunkSize += 1;

                GUI.enabled = true;
                GUILayout.Space(8f);

                var shouldDisplayAsGrid =
                    (GUILayout.Toolbar(ToolboxController.IsDisplayModeGrid ? 1 : 0, displayModeNames) == 1);

                if (shouldDisplayAsGrid != ToolboxController.IsDisplayModeGrid)
                {
                    ToolboxController.IsDisplayModeGrid = shouldDisplayAsGrid;
                }

                GUILayout.Space(8f);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(12f);
        }
        
        private void DrawTabs()
        {
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(12f);
            
            var activeTab = GUILayout.Toolbar(ActiveTab, tabNames, "TabButton", GUI.ToolbarButtonSize.FitToContents);

            if (activeTab != ActiveTab)
            {
                _storedScrollViewPositions[ActiveTab] = ScrollViewPosition;
                ScrollViewPosition = _storedScrollViewPositions[activeTab];
                ActiveTab = activeTab;
                ConstructiblesGrid.VirtualGrid.VisibleViews = new[] { ActiveTab };
            }
            GUILayout.EndHorizontal();
            Utils.DrawHorizontalLine();
        }
    }
}
