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

        private void ReRegisterHotKey(HotKey hotKey)
        {

            if (hotKey.RegistrationId > 0)
            {
                User32.UnregisterHotKey(Handle, hotKey.RegistrationId);
            }

            hotKey.RegistrationId = _hotkeyIdCounter++;
            User32.RegisterHotKey(Handle, hotKey.RegistrationId, (uint)hotKey.Modifier, (uint)hotKey.Keys);
        }

        public HotKey HotKey
        {
            get
            {
                return _hotKey;
            }
            set
            {
                _hotKey = value;
                ReRegisterHotKey(_hotKey);
            }
        }

        public HotKey TaskbarHotKey
        {
            get
            {
                return _taskbarHotKey;
            }
            set
            {
                _taskbarHotKey = value;
                ReRegisterHotKey(_taskbarHotKey);
            }
        }

        public HotKey DebugHotKey
        {
            get
            {
                return _debugHotKey;
            }
            set
            {
                _debugHotKey = value;
                ReRegisterHotKey(_debugHotKey);
            }
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
                    OnHotKeyActivated != null)
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
