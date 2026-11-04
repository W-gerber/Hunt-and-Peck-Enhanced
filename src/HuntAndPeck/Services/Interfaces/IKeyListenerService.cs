using System;
using System.Windows.Forms;
using HuntAndPeck.NativeMethods;

namespace HuntAndPeck.Services.Interfaces
{
    internal class HotKey
    {
        public KeyModifier Modifier { get; set; }
        public Keys Keys { get; set; }

        public int RegistrationId { get; set; }

        // False when Windows refused the combo, i.e. another app already owns it.
        public bool Registered { get; set; }
    }

    internal interface IKeyListenerService
    {
        event EventHandler OnHotKeyActivated;
        event EventHandler OnTaskbarHotKeyActivated;
        event EventHandler OnDebugHotKeyActivated;

        HotKey TaskbarHotKey { get; set; }
        HotKey HotKey { get; set; }
        HotKey DebugHotKey { get; set; }

        // Releases every hotkey while true, so Options can record a combo
        // without the current hotkey firing.
        bool Paused { get; set; }
    }
}
