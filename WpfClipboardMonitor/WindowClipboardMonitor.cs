using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace WpfClipboardMonitor
{
    public class WindowClipboardMonitor : IDisposable
    {
        public event EventHandler<string> ClipboardTextChanged;

        HwndSource Win32InteropSource;
        IntPtr WindowInteropHandle;
        private bool disposed = false;

        public WindowClipboardMonitor(Window clipboardWindow)
        {
            InitializeInteropSource(clipboardWindow);
            InitializeWindowInteropHandle(clipboardWindow);

            StartHandlingWin32Messages();
            AddListenerForClipboardWin32Messages();
        }

        private void InitializeInteropSource(Window clipboardWindow)
        {
            var presentationSource = PresentationSource.FromVisual(clipboardWindow);
            Win32InteropSource = presentationSource as HwndSource;

            if (Win32InteropSource == null)
            {
                throw new ArgumentException(
                    $"Window must be initialized before using the {nameof(WindowClipboardMonitor)}. Use the window's OnSourceInitialized() handler if possible, or a later point in the window lifecycle."
                    , nameof(clipboardWindow));
            }
        }

        private void InitializeWindowInteropHandle(Window clipboardWindow)
        {
            WindowInteropHandle = new WindowInteropHelper(clipboardWindow).Handle;
            if (WindowInteropHandle == null)
            {
                throw new ArgumentException(
                    $"{nameof(clipboardWindow)} must be initialized before using the {nameof(WindowClipboardMonitor)}. Use the Window's OnSourceInitialized() handler if possible, or a later point in the window lifecycle."
                    , nameof(clipboardWindow));
            }
        }

        private void StartHandlingWin32Messages()
        {
            Win32InteropSource.AddHook(Win32InteropMessageHandler);
        }

        private void StopHandlingWin32Messages()
        {
            Win32InteropSource.RemoveHook(Win32InteropMessageHandler);
        }

        private void AddListenerForClipboardWin32Messages()
        {
            NativeInterop.AddClipboardFormatListener(WindowInteropHandle);
        }

        private void RemoveListenerForClipboardWin32Messages()
        {
            NativeInterop.RemoveClipboardFormatListener(WindowInteropHandle);
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
            ProcessClipboardTextWithRetry();
        }

        private void ProcessClipboardTextWithRetry()
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

        private void SleepUntilNextRetry(int currentAttemptNumber)
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

        public void Dispose()
        {
            ReleaseResources();
            GC.SuppressFinalize(this);
        }

        protected virtual void ReleaseResources()
        {
            if (disposed)
            {
                return;
            }
            else
            {
                disposed = true;
            }

            RemoveListenerForClipboardWin32Messages();
            StopHandlingWin32Messages();

            Win32InteropSource = null;
            WindowInteropHandle = IntPtr.Zero;
        }

        ~WindowClipboardMonitor()
        {
            ReleaseResources();
        }
    }
}
