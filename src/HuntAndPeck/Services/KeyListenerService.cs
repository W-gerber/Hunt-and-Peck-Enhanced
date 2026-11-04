using HuntAndPeck.NativeMethods;
using System;
using System.Windows.Forms;
using HuntAndPeck.Services.Interfaces;

namespace HuntAndPeck.Services
{
    internal class KeyListenerService : Form, IKeyListenerService, IDisposable
    {
        public event EventHandler OnHotKeyActivated;
        public event EventHandler OnTaskbarHotKeyActivated;
        public event EventHandler OnDebugHotKeyActivated;

        private int _hotkeyIdCounter = 0;

        private HotKey _hotKey;
        private HotKey _taskbarHotKey;
        private HotKey _debugHotKey;

        // Swaps a registration: the old combo has to be released or it stays
        // live alongside the new one until the app exits.
        private HotKey Register(HotKey old, HotKey hotKey)
        {
            if (old != null)
            {
                User32.UnregisterHotKey(Handle, old.RegistrationId);
            }

            if (hotKey != null && !_paused)
            {
                hotKey.RegistrationId = ++_hotkeyIdCounter;
                hotKey.Registered = User32.RegisterHotKey(Handle, hotKey.RegistrationId, (uint)hotKey.Modifier, (uint)hotKey.Keys);
            }
            return hotKey;
        }

        private bool _paused;

        public bool Paused
        {
            get { return _paused; }
            set
            {
                _paused = value;
                HotKey = _hotKey;
                TaskbarHotKey = _taskbarHotKey;
                DebugHotKey = _debugHotKey;
            }
        }

        public HotKey HotKey
        {
            get { return _hotKey; }
            set { _hotKey = Register(_hotKey, value); }
        }

        public HotKey TaskbarHotKey
        {
            get { return _taskbarHotKey; }
            set { _taskbarHotKey = Register(_taskbarHotKey, value); }
        }

        public HotKey DebugHotKey
        {
            get { return _debugHotKey; }
            set { _debugHotKey = Register(_debugHotKey, value); }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY)
            {
                var e = new HotKeyEventArgs(m.LParam);

                if (_hotKey != null &&
                    e.Key == _hotKey.Keys &&
                    e.Modifiers == _hotKey.Modifier &&
                    OnHotKeyActivated != null)
                {
                    OnHotKeyActivated(this, new EventArgs());
                }

                if (_taskbarHotKey != null &&
                    e.Key == _taskbarHotKey.Keys &&
                    e.Modifiers == _taskbarHotKey.Modifier &&
                    OnTaskbarHotKeyActivated != null)
                {
                    OnTaskbarHotKeyActivated(this, new EventArgs());
                }

                if (_debugHotKey != null &&
                    e.Key == _debugHotKey.Keys &&
                    e.Modifiers == _debugHotKey.Modifier &&
                    OnDebugHotKeyActivated != null)
                {
                    OnDebugHotKeyActivated(this, new EventArgs());
                }
            }

            base.WndProc(ref m);
        }

        protected override void SetVisibleCore(bool value)
        {

            base.SetVisibleCore(false);
        }
    }
}
