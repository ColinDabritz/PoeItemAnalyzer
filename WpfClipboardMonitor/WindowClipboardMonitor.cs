using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace WpfClipboardMonitor
{
    public class WindowClipboardMonitor
    {
        private static readonly IntPtr Win32MessageHandlerSuccess = IntPtr.Zero;

        public event EventHandler<string> ClipboardTextChanged;

        public WindowClipboardMonitor(Window window)
        {
            RegisterWindowToRecieveWin32Messages(window);
            RegisterForClipboardWin32Messages(window);
        }

        private void RegisterWindowToRecieveWin32Messages(Window window)
        {
            var presentationSource = PresentationSource.FromVisual(window);
            HwndSource Win32InteropSource = presentationSource as HwndSource;

            if (Win32InteropSource == null)
            {
                throw new ArgumentException(
                    $"{nameof(window)} must be initialized before using the {nameof(WindowClipboardMonitor)}. Use the Window's OnSourceInitialized() handler if possible, or a later point in the window lifecycle."
                    , nameof(window));
            }

            RegisterForAllWin32Messages(Win32InteropSource);
        }

        private void RegisterForAllWin32Messages(HwndSource win32InteropSource)
        {
            win32InteropSource.AddHook(Win32InteropMessageHandler);
        }

        private void RegisterForClipboardWin32Messages(Window windowSource)
        {
            IntPtr windowHandleForInterop = new WindowInteropHelper(windowSource).Handle;
            NativeMethods.AddClipboardFormatListener(windowHandleForInterop);
        }

        private IntPtr Win32InteropMessageHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool messageHandled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                messageHandled = true;
            }

            return Win32MessageHandlerSuccess;
        }

        private void OnClipboardChanged()
        {
            PerformActionWithRetryAndSupressComExceptions(ProcessClipboardText);
        }

        private void ProcessClipboardText()
        {
            if (Clipboard.ContainsText())
            {
                ClipboardTextChanged?.Invoke(this, Clipboard.GetText());
            }
        }

        private void PerformActionWithRetryAndSupressComExceptions(Action action)
        {
            const int maxAttempts = 10;
            const int baseSleepDurationMilliseconds = 2;

            int sleepDurationMilliseconds = baseSleepDurationMilliseconds;
            int remainingAttempts = maxAttempts;

            while (remainingAttempts > 0)
            {
                try
                {
                    action();
                    return;
                }
                catch (COMException)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(sleepDurationMilliseconds));
                    sleepDurationMilliseconds *= 2;
                    remainingAttempts--;
                }
            }
        }

    }
}
