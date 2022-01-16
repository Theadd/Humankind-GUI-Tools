using System;
using System.IO;
using System.Linq;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using HarmonyLib;
using Amplitude.Mercury.Presentation;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace StyledGUI
{
    public class KeyboardShortcutField
    {
        public KeyboardShortcut Value { get; private set; } = KeyboardShortcut.Empty;
        public KeyboardShortcut InitialValue { get; private set; }

        private bool initialValueProvided;

        public static GUIStyle DefaultToggleCaptureStyle { get; set; } = new GUIStyle(UIController.DefaultSkin.toggle) {
            fontSize = 10,
            normal = new GUIStyleState() {
                background = UIController.DefaultSkin.toggle.hover.background,
                textColor = new Color32(20, 20, 20, 255)
            },
            hover = new GUIStyleState() {
                background = UIController.DefaultSkin.toggle.active.background,
                textColor = new Color32(20, 20, 20, 255)
            },
            active = new GUIStyleState() {
                background = UIController.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
            onNormal = new GUIStyleState() {
                background = UIController.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
            onHover = new GUIStyleState() {
                background = UIController.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
            onActive = new GUIStyleState() {
                background = UIController.DefaultSkin.textField.onNormal.background,
                textColor = new Color32(250, 250, 250, 255)
            },
        };

        public GUIStyle ToggleCaptureStyle { get; set; } = DefaultToggleCaptureStyle;

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

        public string DisplayText => Value.Equals(KeyboardShortcut.Empty) ? (initialValueProvided && !IsCapturing ? InitialValue.Serialize() : DefaultText) : Value.Serialize();

        public void Draw(params GUILayoutOption[] options)
        {
            GUI.color = Color.white;
            var shouldCapture = GUILayout.Toggle(IsCapturing, DisplayText.ToUpper(), ToggleCaptureStyle, options);
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
                }
            }
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
