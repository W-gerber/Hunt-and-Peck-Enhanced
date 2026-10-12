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
    }

    internal interface IKeyListenerService
    {
        event EventHandler OnHotKeyActivated;
        event EventHandler OnTaskbarHotKeyActivated;
        event EventHandler OnDebugHotKeyActivated;

        HotKey TaskbarHotKey { get; set; }
        HotKey HotKey { get; set; }
        HotKey DebugHotKey { get; set; }
    }
}
