using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.ViewModels;
using Keys = System.Windows.Forms.Keys;

namespace HuntAndPeck.Views
{

    public partial class OptionsView : Window
    {
        public OptionsView()
        {
            InitializeComponent();
            SourceInitialized += (s, e) => UseDarkTitleBar();
            Closed += (s, e) => Vm.Recording = false;
        }

        private OptionsViewModel Vm
        {
            get { return (OptionsViewModel)DataContext; }
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        // A white system title bar over a dark window is the main thing that
        // made it look unfinished. Both calls are no-ops on Windows versions
        // that don't support them.
        private void UseDarkTitleBar()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            var on = 1;
            DwmSetWindowAttribute(hwnd, 20, ref on, sizeof(int));        // DWMWA_USE_IMMERSIVE_DARK_MODE
            var caption = 0x00251818;                                     // #181825 as 0x00BBGGRR
            DwmSetWindowAttribute(hwnd, 35, ref caption, sizeof(int));   // DWMWA_CAPTION_COLOR (Windows 11)
        }

        // Hotkeys are released while a box has focus, or pressing the current
        // shortcut would pop the overlay instead of being recorded.
        private void HotKeyBox_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Vm.Recording = true;
        }

        private void HotKeyBox_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Vm.Recording = false;
        }

        private void HotKeyBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            switch (key)
            {
                case Key.Escape:
                    Tabs.Focus();
                    return;
                case Key.LeftCtrl: case Key.RightCtrl:
                case Key.LeftAlt: case Key.RightAlt:
                case Key.LeftShift: case Key.RightShift:
                case Key.LWin: case Key.RWin:
                    return; // wait for the actual key
            }

            // A bare key would fire every time you typed it.
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                return;
            }

            Vm.SetHotKey(
                (string)((FrameworkElement)sender).Tag == "Main",
                (Keys)KeyInterop.VirtualKeyFromKey(key),
                (KeyModifier)(int)Keyboard.Modifiers);
            Tabs.Focus();
        }
    }
}
