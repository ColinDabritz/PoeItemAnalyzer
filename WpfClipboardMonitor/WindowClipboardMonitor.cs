using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace WpfClipboardMonitor
{
    public class WindowClipboardMonitor
    {
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
            NativeInterop.AddClipboardFormatListener(windowHandleForInterop);
        }

        private IntPtr Win32InteropMessageHandler(IntPtr windowHandle, int messageCode, IntPtr wParam, IntPtr lParam, ref bool messageHandled)
        {
            if (messageCode == NativeInterop.ClipboardUpdateWindowMessageCode)
            {
                OnClipboardChanged();

                messageHandled = true;
                return NativeInterop.HandledClipboardUpdateReturnCode;
            }

            return NativeInterop.NoMessageHandledReturnCode;
        }

        private void OnClipboardChanged()
        {
            PerformRetryableComOperation();
        }

        private void PerformRetryableComOperation()
        {
            const int maxAttempts = 10;
            int currentAttemptNumber = 1;
            
            while (currentAttemptNumber <= maxAttempts)
            {
                try
                {
                    ProcessClipboardText();
                    return;
                }
                catch (COMException ex) when (ex.ErrorCode == NativeInterop.UnableToOpenClipboardComErrorCode)
                {
                    SleepUntilNextRetry(currentAttemptNumber);
                }
                currentAttemptNumber++;
            }
        }

        private void SleepUntilNextRetry(int attemptNumber)
        {
            const int sleepDurationMilliseconds = 50;
            var timeUntilNextRetry = TimeSpan.FromMilliseconds(sleepDurationMilliseconds);
            Thread.Sleep(timeUntilNextRetry);
        }

        private void ProcessClipboardText()
        {
            if (Clipboard.ContainsText())
            {
                ClipboardTextChanged?.Invoke(this, Clipboard.GetText());
            }
        }
    }
}
