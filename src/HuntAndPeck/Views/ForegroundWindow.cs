using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using HuntAndPeck.NativeMethods;

namespace HuntAndPeck.Views
{

    public class ForegroundWindow : Window
    {
        private bool _closing;
        private bool _initialized;

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!_initialized)
            {

                if (!User32.SetForegroundWindow(new WindowInteropHelper(this).Handle))
                {
                    ForceForeground();
                }
                _initialized = true;
            }
            base.OnRender(drawingContext);
        }

        protected override void OnDeactivated(EventArgs e)
        {

            if (_initialized && !_closing)
            {
                Close();
            }
            base.OnDeactivated(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _closing = true;
            base.OnClosing(e);
        }

        private void ForceForeground()
        {

            var targetThread = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), IntPtr.Zero);
            var appThread = Kernel32.GetCurrentThreadId();
            var attached = false;

            try
            {
                if (targetThread == appThread)
                {

                    return;
                }

                attached = User32.AttachThreadInput(targetThread, appThread, true);

                if (!attached)
                {

                    Close();
                    return;
                }

                var ourHandle = new WindowInteropHelper(this).Handle;

                User32.BringWindowToTop(ourHandle);
                User32.SetFocus(ourHandle);
            }
            finally
            {
                if (attached)
                {

                    User32.AttachThreadInput(targetThread, appThread, false);
                }
            }
        }
    }
}
