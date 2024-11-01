using System;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;

namespace StyledGUI
{
    public static class KeyboardShortcutEx
    {
        public static string AsDisplayValue(this KeyboardShortcut key)
        {
            return string.Join(" + ", key
                .Serialize()
                .ToUpperInvariant()
                .Replace("CONTROL", "CTRL")
                .Replace("RIGHT", "R")
                .Replace("LEFT", "L")
                .Split(new string[] {" + "}, StringSplitOptions.None)
                .Select(s => s == "LARROW" ? "LEFT" : 
                    s == "RARROW" ? "RIGHT" : 
                    s == "UPARROW" ? "UP" : 
                    s == "DOWNARROW" ? "DOWN" : 
                    s == "MOUSE0" ? "LCLICK" : 
                    s == "MOUSE1" ? "RCLICK" : s)
                .Reverse());
        }
    }
    
    public class KeyboardShortcutField
    {
        public KeyboardShortcut Value { get; private set; } = KeyboardShortcut.Empty;
        public KeyboardShortcut InitialValue { get; private set; }
        public GUIStyle Style { get; set; } = Styles.ToggleCaptureStyle;

        private bool initialValueProvided;
        
        public KeyboardShortcutField() : this(KeyboardShortcut.Empty) {}

        public KeyboardShortcutField(KeyboardShortcut initialValue)
        {
            Value = initialValue;
            InitialValue = initialValue;

            initialValueProvided = !initialValue.Equals(KeyboardShortcut.Empty);
        }

        public bool IsCapturing { get; private set; } = false;

        public string DefaultText { get; set; } = "";

        private CaptureKeyCombo keyComboCapturer;

        public string DisplayTextPrefix { get; set; } = "";
        public string DisplayTextPostfix { get; set; } = "";

        public string DisplayText => DisplayTextPrefix +
                                     (Value.Equals(KeyboardShortcut.Empty)
                                         ? (initialValueProvided && !IsCapturing
                                             ? InitialValue.AsDisplayValue()
                                             : DefaultText)
                                         : Value.AsDisplayValue()) + DisplayTextPostfix;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns>true, only one time, just after capturing is done.</returns>
        public bool Draw(GUIStyle style, params GUILayoutOption[] options)
        {
            GUI.color = Color.white;
            var shouldCapture = GUILayout.Toggle(IsCapturing, DisplayText.ToUpperInvariant(), style ?? Style, options);
            if (shouldCapture && !IsCapturing)
            {
                // Start capturing
                keyComboCapturer = new CaptureKeyCombo();
                IsCapturing = true;
            }

            if (IsCapturing)
            {
                bool isDoneCapturing = keyComboCapturer.Capture();
                
                Value = keyComboCapturer.KeyCombo;
                if (isDoneCapturing)
                {
                    if (Value.Equals(KeyboardShortcut.Empty))
                        Value = InitialValue;

                    IsCapturing = false;

                    return true;
                }
            }

            return false;
        }
    }

    public class CaptureKeyCombo
    {
        public KeyCode[] stack = new KeyCode[0] {};
        public KeyCode lastKeyAddedToStack = KeyCode.None;

        public KeyCode[] CapturedKeys = new KeyCode[0] {};
        public KeyCode CapturedMainKey = KeyCode.None;

        public KeyboardShortcut KeyCombo = KeyboardShortcut.Empty;

        public bool Capture()
        {
            KeyCode[] currentStack = KeyboardShortcut.AllKeyCodes.Where(key => Input.GetKey(key)).ToArray();
            
            if (currentStack.Length == 0)
            {
                if (lastKeyAddedToStack == KeyCode.None)
                    return false;

                // KeyCombo captured
                UpdateKeyCombo();
                stack = new KeyCode[0] {};
                lastKeyAddedToStack = KeyCode.None;

                return true;
            }

            if (currentStack.Length > stack.Length) {
                // adding KeyCodes to the combo
                lastKeyAddedToStack = currentStack.Where(k => !stack.Contains(k)).First();
                stack = currentStack;
                
                CapturedKeys = stack.Select(k => k).ToArray();
                CapturedMainKey = lastKeyAddedToStack;

                UpdateKeyCombo();
            }
            else
            {
                if (currentStack.Length < stack.Length)
                {
                    stack = currentStack;
                }
            }

            return false;
        }

        private void UpdateKeyCombo()
        {
            KeyCombo = new KeyboardShortcut(CapturedMainKey, CapturedKeys.Where(k => k != CapturedMainKey).ToArray());
        }
    }

}
