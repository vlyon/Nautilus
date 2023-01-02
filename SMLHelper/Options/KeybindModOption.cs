﻿namespace SMLHelper.Options
{
    using System;
    using System.Collections;
    using SMLHelper.Utility;
    using UnityEngine;
    using UnityEngine.Events;
    using TMPro;

    /// <summary>
    /// Contains all the information about a keybind changed event.
    /// </summary>
    public class KeybindChangedEventArgs : ModEventArgs<KeyCode>
    {
        /// <summary>
        /// The ID of the <see cref="ModKeybindOption"/> that was changed.
        /// </summary>
        public override string Id { get; }

        /// <summary>
        /// The new value for the <see cref="ModKeybindOption"/>.
        /// </summary>
        public override KeyCode Value { get; }

        /// <summary>
        /// Constructs a new <see cref="KeybindChangedEventArgs"/>.
        /// </summary>
        /// <param name="id">The ID of the <see cref="ModKeybindOption"/> that was changed.</param>
        /// <param name="key">The new value for the <see cref="ModKeybindOption"/>.</param>
        public KeybindChangedEventArgs(string id, KeyCode key)
        {
            this.Id = id;
            this.Value = key;
        }
    }

    /// <summary>
    /// A mod option class for handling an option that is a keybind.
    /// </summary>
    public class ModKeybindOption : ModOption
    {
        /// <summary>
        /// The currently selected <see cref="KeyCode"/> for the <see cref="ModKeybindOption"/>.
        /// </summary>
        public KeyCode Key { get; }

        /// <summary>
        /// The currently select input source device for the <see cref="ModKeybindOption"/>.
        /// </summary>
        public GameInput.Device Device { get; }

        /// <summary>
        /// Instantiates a new <see cref="ModKeybindOption"/> for handling an option that is a keybind.
        /// </summary>
        /// <param name="id">The internal ID of this option.</param>
        /// <param name="label">The display text to show on the in-game menus.</param>
        /// <param name="device">The device name.</param>
        /// <param name="key">The starting keybind value.</param>
        private ModKeybindOption(string id, string label, GameInput.Device device, KeyCode key) : base(label, id, typeof(KeyCode), key)
        {
            this.Device = device;
            this.Key = key;
        }

        /// <summary>
        /// Creates a new <see cref="ModKeybindOption"/> for handling an option that is a keybind.
        /// </summary>
        /// <param name="id">The internal ID for the toggle option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="device">The device name.</param>
        /// <param name="key">The starting keybind value.</param>
        public static ModKeybindOption Factory(string id, string label, GameInput.Device device, KeyCode key)
        {
            return new ModKeybindOption(id, label, device, key);
        }
        /// <summary>
        /// Creates a new <see cref="ModKeybindOption"/> for handling an option that is a keybind.
        /// </summary>
        /// <param name="id">The internal ID for the toggle option.</param>
        /// <param name="label">The display text to use in the in-game menu.</param>
        /// <param name="device">The device name.</param>
        /// <param name="key">The starting keybind value.</param>
        public static ModKeybindOption Factory(string id, string label, GameInput.Device device, string key)
        {
            return Factory(id, label, device, KeyCodeUtils.StringToKeyCode(key));
        }

        internal override void AddToPanel(uGUI_TabbedControlsPanel panel, int tabIndex)
        {
            // Add item
            OptionGameObject = panel.AddItem(tabIndex, panel.bindingOptionPrefab);

            // Update text
            TextMeshProUGUI text = OptionGameObject.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                OptionGameObject.GetComponentInChildren<TranslationLiveUpdate>().translationKey = Label;
                text.text = Language.main.Get(Label);
            }

            // Create bindings
            uGUI_Bindings bindings = OptionGameObject.GetComponentInChildren<uGUI_Bindings>();
            uGUI_Binding binding = bindings.bindings[0];

            // Destroy secondary bindings
            int last = bindings.bindings.Length - 1;
            UnityEngine.Object.Destroy(bindings.bindings[last].gameObject);
            UnityEngine.Object.Destroy(bindings);

            // Update bindings
            binding.device = Device;
            binding.value = KeyCodeUtils.KeyCodeToString(Key);
            binding.gameObject.EnsureComponent<ModBindingTag>();
            binding.bindingSet = GameInput.BindingSet.Primary;
            binding.bindCallback = new Action<GameInput.Device, GameInput.Button, GameInput.BindingSet, string>((_, _1, _2, s) =>
            {
                binding.value = s;
                parentOptions.OnChange<KeybindChangedEventArgs, KeyCode>(Id, KeyCodeUtils.StringToKeyCode(s));
                binding.RefreshValue();
            });

            base.AddToPanel(panel, tabIndex);
        }

        internal class ModBindingTag: MonoBehaviour { };

        private class BindingOptionAdjust: ModOptionAdjust
        {
            private const float spacing = 10f;

            public IEnumerator Start()
            {
                SetCaptionGameObject("Caption");
                yield return null; // skip one frame

                RectTransform rect = gameObject.transform.Find("Bindings") as RectTransform;

                float widthAll = gameObject.GetComponent<RectTransform>().rect.width;
                float widthBinding = rect.rect.width;
                float widthText = CaptionWidth + spacing;

                if (widthText + widthBinding > widthAll)
                {
                    rect.sizeDelta = SetVec2x(rect.sizeDelta, widthAll - widthText - widthBinding);
                }

                Destroy(this);
            }
        }
        internal override Type AdjusterComponent => typeof(BindingOptionAdjust);
    }
}
