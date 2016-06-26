using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace WpfClipboardMonitor
{
    public class WindowClipboard
    {
        public event EventHandler<string> ClipboardTextChanged;

        public WindowClipboard(Window windowSource)
        {
            HwndSource source = PresentationSource.FromVisual(windowSource) as HwndSource;
            if (source == null)
            {
                throw new ArgumentException(
                    "Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler."
                    , nameof(windowSource));
            }

            source.AddHook(WndProc);

            // get window handle for interop
            IntPtr windowHandle = new WindowInteropHelper(windowSource).Handle;

            // register for clipboard events
            NativeMethods.AddClipboardFormatListener(windowHandle);
        }

        private void OnClipboardChanged()
        {
            IDataObject clipboardData = Clipboard.GetDataObject();

            var formats = clipboardData.GetFormats();
            if (!formats.Contains(DataFormats.Text))
            {
                // not text, no handling
                return;
            }
            
            Console.WriteLine("Clipboard contains text.");

            string clipboardText = string.Empty;

            int maxAttempts = 10;
            int sleepDurationMilliseconds = 10;

            int remainingAttempts = maxAttempts;

            while (remainingAttempts > 0)
            {
                remainingAttempts--;

                try
                {
                    clipboardText = Clipboard.GetText();

                    // send text changed
                    ClipboardTextChanged?.Invoke(this, clipboardText);
                    return;
                }
                catch (COMException ex) when (ex.ErrorCode == -2147221040)
                {
                    // this means we were unable to obtain a clipboard lock
                    // mitigated by trying again, up to max retry count.
                    // Additional info is: OpenClipboard Failed (Exception from HRESULT: 0x800401D0 (CLIPBRD_E_CANT_OPEN))
                    // See https://github.com/ColinDabritz/PoeItemAnalyzer/issues/1

                    Console.WriteLine("Encountered clipboard locking issue - CLIPBRD_E_CANT_OPEN (trying again)");

                    // intentionally supressing this error!
                    // try agin after a pause
                    Thread.Sleep(TimeSpan.FromMilliseconds(sleepDurationMilliseconds));
                    continue;
                }
            }

            if (remainingAttempts <= 0)
            {
                throw new InvalidOperationException($"Unable to access clipboard after {maxAttempts} attempts.");
            }
        }

        private static readonly IntPtr WndProcSuccess = IntPtr.Zero;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                handled = true;
            }

            return WndProcSuccess;
        }
    }
}
