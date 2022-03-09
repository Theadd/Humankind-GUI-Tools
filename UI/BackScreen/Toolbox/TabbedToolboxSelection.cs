using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using StyledGUI;
using StyledGUI.VirtualGridElements;

namespace DevTools.Humankind.GUITools.UI
{
    public partial class TabbedToolbox
    {
        private bool _drawSelectionPreview = false;
        private ICell _selectedCell = null;
        private Texture _selectedImage;
        private string _selectedTitle;
        private string _selectedCategory;
        private string _selectedSubtitle;
        private string _selectedTags;
        private string _selectedType;

        private GUIStyle SelectionPreviewLabel { get; set; } = new GUIStyle(Styles.Fixed20pxHeightTextStyle)
        {
            fontSize = 16,
            fixedHeight = 0,
            wordWrap = true,
            stretchWidth = true,
            
        };
        
        private GUIStyle SelectionPreviewCell { get; set; } = new GUIStyle(Styles.ColorableCellStyle)
        {
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            hover = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            }
        };
        
        private Color SelectionPreviewTintColor { get; set; } = new Color32(85, 136, 254, 230);
        private Color SelectedUniqueNameColor { get; set; } = new Color32(0, 0, 0, 155);
        // private Color SelectionPreviewTintColor { get; set; } = new Color32(40, 86, 240, 255);

        private void DrawSelectionPreview()
        {
            if (_drawSelectionPreview)
            {
                // Utils.DrawHorizontalLine();
                var prevTint = GUI.backgroundColor;
                GUI.backgroundColor = SelectionPreviewTintColor;
                using (var cellScope = new GUILayout.HorizontalScope(SelectionPreviewCell, GUILayout.Height(128f)))
                {
                    GUILayout.BeginVertical(GUILayout.Width(128f), GUILayout.Height(128f));
                    {
                        var r = GUILayoutUtility.GetRect(128f, 128f);
                        GUI.DrawTexture(
                            new Rect(r.x + 16f, r.y + 16f, 96f, 96f), 
                            // StyledGUI.Graphics.BlackTexture,
                            _selectedImage ? _selectedImage : StyledGUI.Graphics.TransparentTexture, 
                            ScaleMode.ScaleToFit, 
                            true, 
                            0,
                            Color.white, 
                            0, 
                            0
                        );
                    }
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                    {
                        GUI.contentColor = Color.white;
                        GUILayout.Space(8f);
                        GUILayout.Label(_selectedTitle, SelectionPreviewLabel);
                        GUILayout.Space(8f);
                        GUI.contentColor = SelectedUniqueNameColor;
                        GUILayout.Label(_selectedSubtitle, SelectionPreviewLabel);
                        GUILayout.Label(_selectedType, SelectionPreviewLabel);
                        GUILayout.FlexibleSpace();
                        GUI.contentColor = Color.yellow;// Styles.GoldTextColor;
                        GUILayout.Label(_selectedCategory, SelectionPreviewLabel);
                        GUI.contentColor = Color.white;
                        GUILayout.Label(_selectedTags, SelectionPreviewLabel);
                        GUILayout.Space(8f);
                    }
                    GUILayout.EndVertical();
                }

                GUI.backgroundColor = prevTint;
            }
            
            if (Event.current.type == EventType.Repaint)
            {
                if (((TypeDefinitionsGrid.VirtualGrid.Cursor.IsSelectionActive || IsMouseHoverCell) && ToolboxController.IsDisplayModeGrid) != _drawSelectionPreview ||
                    (((TypeDefinitionsGrid.VirtualGrid.Cursor.IsSelectionActive || IsMouseHoverCell) && ToolboxController.IsDisplayModeGrid) &&
                     _selectedCell != (IsMouseHoverCell ? CellWithMouseHover : TypeDefinitionsGrid.VirtualGrid.Cursor.SelectedCell.Cell)))
                {
                    _drawSelectionPreview = (TypeDefinitionsGrid.VirtualGrid.Cursor.IsSelectionActive || IsMouseHoverCell) &&
                                            ToolboxController.IsDisplayModeGrid;
                    
                    if (_drawSelectionPreview)
                    {
                        _selectedCell = IsMouseHoverCell ? CellWithMouseHover : TypeDefinitionsGrid.VirtualGrid.Cursor.SelectedCell.Cell;
                        // Loggr.Log(SelectionPreviewCell);
                        // Loggr.Log(UIController.DefaultSkin.FindStyle("Text"));
                        if (_selectedCell is IClickableImageCell cell)
                        {
                            // if (cell.Image != null)
                            //     Loggr.Log("" + cell.Image.width + " x " + cell.Image.height);
                            _selectedImage = cell.Image;
                            _selectedTitle = "<size=16><b>" + cell.Title + "</b></size>";
                            _selectedSubtitle = "<size=12>" + cell.UniqueName + "</size>";
                            _selectedType = "<size=12>" + (IsMouseHoverCell ? "" : (LiveEditorMode.ActivePaintBrush?.GetType().Name ?? "")) + "</size>";
                            _selectedCategory = "<size=10>" + cell.Category + "</size>";
                            _selectedTags = cell.Tags;
                        }
                        else
                        {
                            _drawSelectionPreview = false;
                        }
                    }
                }
            }
        }
    }
}
