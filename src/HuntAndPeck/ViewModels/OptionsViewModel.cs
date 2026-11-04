using HuntAndPeck.NativeMethods;
using HuntAndPeck.Properties;
using HuntAndPeck.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Forms;

namespace HuntAndPeck.ViewModels
{
    internal class OptionsViewModel : INotifyPropertyChanged
    {
        private const string InUse = "Another app is already using this shortcut. Pick a different one.";

        private readonly IKeyListenerService _keyListener;

        public OptionsViewModel(IKeyListenerService keyListener)
        {
            _keyListener = keyListener;
            RefreshHotKeyStatus();
        }

        // Every setter writes straight to Settings; ShellViewModel re-registers
        // hotkeys from there, and the overlay reads the rest when it opens.
        public string FontSize
        {
            get { return Settings.Default.FontSize; }
            set { Save("FontSize", value); }
        }

        public string FontFamily
        {
            get { return Settings.Default.FontFamily; }
            set { Save("FontFamily", value); }
        }

        // Only fonts that are actually installed; a missing one silently falls
        // back to the system font. The saved font stays listed so it still shows.
        public IList<string> FontFamilies
        {
            get
            {
                var installed = new HashSet<string>(Fonts.SystemFontFamilies.Select(f => f.Source));
                var list = new[] { "Arial", "Calibri", "Cascadia Code", "Cascadia Mono", "Consolas",
                                   "JetBrains Mono", "JetBrainsMono NF", "Segoe UI", "Tahoma", "Verdana" }
                    .Where(installed.Contains).ToList();
                if (!list.Contains(FontFamily))
                {
                    list.Add(FontFamily);
                }
                return list;
            }
        }

        public string BackgroundColor
        {
            get { return Settings.Default.BackgroundColor; }
            set { Save("BackgroundColor", value); }
        }

        public string InactiveBackgroundColor
        {
            get { return Settings.Default.InactiveBackgroundColor; }
            set { Save("InactiveBackgroundColor", value); }
        }

        public string TextColor
        {
            get { return Settings.Default.TextColor; }
            set { Save("TextColor", value); }
        }

        // Two plain hints and one matching what you typed, as the overlay shows them.
        public object[] PreviewHints
        {
            get
            {
                return new object[]
                {
                    new { Label = "SA", Color = InactiveBackgroundColor },
                    new { Label = "SD", Color = InactiveBackgroundColor },
                    new { Label = "DF", Color = BackgroundColor }
                };
            }
        }

        public IList<string> MainHotKeyParts
        {
            get { return Describe(Settings.Default.MainHotKeyKey, Settings.Default.MainHotKeyModifier); }
        }

        public IList<string> TaskbarHotKeyParts
        {
            get { return Describe(Settings.Default.TaskbarHotKeyKey, Settings.Default.TaskbarHotKeyModifier); }
        }

        public string MainHotKeyError { get; private set; }

        public string TaskbarHotKeyError { get; private set; }

        // True while a shortcut box is listening for keys.
        public bool Recording
        {
            set
            {
                _keyListener.Paused = value;
                if (!value)
                {
                    RefreshHotKeyStatus();
                }
            }
        }

        public void SetHotKey(bool main, Keys key, KeyModifier modifier)
        {
            var prefix = main ? "MainHotKey" : "TaskbarHotKey";
            Settings.Default[prefix + "Modifier"] = modifier.ToString();
            Settings.Default[prefix + "Key"] = key.ToString();
            Settings.Default.Save();
            OnPropertyChanged(prefix + "Parts");
        }

        private void RefreshHotKeyStatus()
        {
            MainHotKeyError = _keyListener.HotKey?.Registered == false ? InUse : "";
            TaskbarHotKeyError = _keyListener.TaskbarHotKey?.Registered == false ? InUse : "";
            OnPropertyChanged("MainHotKeyError");
            OnPropertyChanged("TaskbarHotKeyError");
        }

        private void Save(string name, string value)
        {
            if ((string)Settings.Default[name] != value)
            {
                Settings.Default[name] = value;
                Settings.Default.Save();
                OnPropertyChanged(name);
                OnPropertyChanged("PreviewHints");
            }
        }

        // "Control, Shift" + "OemSemicolon" -> ["Ctrl", "Shift", ";"]
        public static IList<string> Describe(string key, string modifier)
        {
            var parts = new List<string>();
            KeyModifier mod;
            if (Enum.TryParse(modifier, out mod))
            {
                if (mod.HasFlag(KeyModifier.Windows)) parts.Add("Win");
                if (mod.HasFlag(KeyModifier.Control)) parts.Add("Ctrl");
                if (mod.HasFlag(KeyModifier.Alt)) parts.Add("Alt");
                if (mod.HasFlag(KeyModifier.Shift)) parts.Add("Shift");
            }

            Keys k;
            if (!Enum.TryParse(key, out k))
            {
                parts.Add(key);
                return parts;
            }

            switch (k)
            {
                case Keys.OemSemicolon: parts.Add(";"); break;
                case Keys.Oemcomma: parts.Add(","); break;
                case Keys.OemPeriod: parts.Add("."); break;
                case Keys.OemQuestion: parts.Add("/"); break;
                case Keys.OemOpenBrackets: parts.Add("["); break;
                case Keys.OemCloseBrackets: parts.Add("]"); break;
                case Keys.OemPipe: parts.Add("\\"); break;
                case Keys.OemQuotes: parts.Add("'"); break;
                case Keys.Oemtilde: parts.Add("`"); break;
                case Keys.OemMinus: parts.Add("-"); break;
                case Keys.Oemplus: parts.Add("="); break;
                case Keys.Space: parts.Add("Space"); break;
                default:
                    parts.Add(k >= Keys.D0 && k <= Keys.D9 ? ((int)(k - Keys.D0)).ToString() : k.ToString());
                    break;
            }
            return parts;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
