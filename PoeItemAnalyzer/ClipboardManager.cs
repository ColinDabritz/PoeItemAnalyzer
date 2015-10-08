﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace PoeItemAnalyzer
{
    public class ClipboardManager
    {
        public event EventHandler ClipboardChanged;

        public ClipboardManager(Window windowSource)
        {
            // TODO: after on source initialized only? Maybe check for null and throw here?
            HwndSource source = PresentationSource.FromVisual(windowSource) as HwndSource;
            if(source == null)
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

        // TODO: Bindable list?

        void OnClipboardChanged()
        {
            ClipboardChanged?.Invoke(this, EventArgs.Empty);
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