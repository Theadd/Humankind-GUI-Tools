using System.Linq;
using StyledGUI;
using StyledGUI.VirtualGridElements;
using System;
using System.Collections.Generic;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Sandbox;
using Amplitude.Framework;
using Amplitude.Framework.Networking;
using BepInEx.Configuration;
using UnityEngine;


namespace DevTools.Humankind.GUITools.UI
{
    public class KeyMapEntry
    {
        public int Index { get; set; }
        public KeyMap KeyMap { get; set; }
        public KeyboardShortcutField Field { get; set; }
        public bool HasPendingAction { get; set; } = false;
        public bool HasInvalidValue { get; set; } = false;
    }
    
    public class ShortcutsGrid : GridStyles { }
    
    public class KeyboardShortcutsGrid
    {
        public VirtualGrid VirtualGrid { get; set; }
        public bool IsDirty { get; set; } = true;

        protected IStyledGrid Grid;

        public KeyMapEntry[] Entries { get; private set; }

        private List<string> KeyGroups = new List<string>();

        public KeyboardShortcutsGrid()
        {
            ResyncKeyMappings();
        }

        public void ResyncKeyMappings()
        {
            Entries = KeyMappings.Keys.Select((keyMap, i) => new KeyMapEntry() {
                Index = i,
                KeyMap = keyMap,
                Field = CreateShortcutField(keyMap.Key)
            }).ToArray();

            foreach (var keyMap in KeyMappings.Keys)
            {
                if (!KeyGroups.Contains(keyMap.GroupName))
                    KeyGroups.Add(keyMap.GroupName);
            }
        }

        public static KeyboardShortcutField CreateShortcutField(KeyboardShortcut initialValue) =>
            new KeyboardShortcutField(initialValue)
            {
                DefaultText = "<size=10><color=#FFFFFF77>START PRESSING ANY KEY...</color></size>",
                DisplayTextPrefix = "<size=10>",
                DisplayTextPostfix = "</size>"
            };
        
        public void Render()
        {
            if (IsDirty)
                Update();

            VirtualGrid.Render();
        }

        public void Update()
        {
            Grid = VirtualGrid.Grid;
            ComputeVirtualGrid();

            IsDirty = false;
        }

        private void ComputeVirtualGrid()
        {
            VirtualGrid.Columns = new[]
            {
                new Column() { Name = "Keyboard Shortcuts"}
            };

            VirtualGrid.Sections = (new Section[]
            {
                BuildSection(KeyMappings.GlobalKeysGroup),
            })
                .Concat(
                    KeyGroups
                        .Where(groupName => groupName != KeyMappings.GlobalKeysGroup)
                        .Select(group => BuildSection(group)).ToArray()
                    ).ToArray();
        }

        private Color InvalidColor { get; set; } = new Color32(200, 15, 35, 230);

        private Section BuildSection(string sectionName)
        {
            return new Section()
            {
                Title = "<b>" + sectionName + "</b>",
                Rows = Entries.Where(entry => entry.KeyMap.GroupName == sectionName).Select(entry => new Row()
                {
                    Style = Styles.StaticRowStyle,
                    Title = entry.KeyMap.DisplayName.ToUpper(),
                    Cells = new ICell[]
                    {
                        new CellGroup()
                        {
                            Cells = new ICell[]
                            {
                                new KeyboardShortcutCell()
                                {
                                    Span = Grid.CellSpan8,
                                    Field = entry.Field,
                                    Index = entry.Index,
                                    Action = OnDoneCapturing,
                                    Color = entry.HasInvalidValue ? InvalidColor : Color.white,
                                    Enabled = entry.KeyMap.IsEditable
                                },
                                new ClickableCell()
                                {
                                    Span = Grid.CellSpan2,
                                    Enabled = entry.KeyMap.IsEditable && entry.HasPendingAction && !entry.HasInvalidValue,
                                    Text = "<size=10>APPLY</size>",
                                    Index = entry.Index,
                                    Action = OnApply
                                },
                                new ClickableCell()
                                {
                                    Span = Grid.CellSpan2,
                                    Enabled = entry.HasPendingAction || (!entry.HasPendingAction && 
                                                                         entry.KeyMap.IsRemovable &&
                                                                         !entry.KeyMap.Key.Equals(
                                                                             KeyboardShortcut.Empty)),
                                    Text = (entry.HasPendingAction
                                        ? "<size=10>CANCEL</size>"
                                        : "<size=10>REMOVE</size>"),
                                    Index = entry.Index,
                                    Action = OnHandleAction
                                }
                            }
                        }
                    }
                }).ToArray()
            };
        }
        
        private void OnDoneCapturing(int index)
        {
            Entries[index].HasPendingAction = true;

            KeyboardShortcut capturedKey = Entries[index].Field.Value;

            Entries[index].HasInvalidValue = Entries
                .Any(entry => entry.Index != index && entry.KeyMap.Key.Equals(capturedKey));
            IsDirty = true;
        }
        
        private void OnApply(int index)
        {
            Entries[index].HasPendingAction = false;
            Entries[index].KeyMap.Key = Entries[index].Field.Value;
            
            KeyMappings.Apply();
            IsDirty = true;
        }


        private void OnHandleAction(int index)
        {
            if (Entries[index].HasPendingAction)
            {
                // Cancel action
                Entries[index].Field = CreateShortcutField(Entries[index].KeyMap.Key);

                Entries[index].HasInvalidValue = false;
            }
            else
            {
                // Remove action
                Entries[index].KeyMap.Key = KeyboardShortcut.Empty;
                Entries[index].Field = CreateShortcutField(Entries[index].KeyMap.Key);
                
                KeyMappings.Apply();
            }

            Entries[index].HasPendingAction = false;
            IsDirty = true;
        }

    }
}