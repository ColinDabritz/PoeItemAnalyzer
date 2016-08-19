﻿using System;
using System.Runtime.InteropServices;

namespace WpfClipboardMonitor
{
    internal static class NativeInterop
    {
        // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
        public const int ClipboardUpdateWindowMessageCode = 0x031D;
        public static readonly IntPtr HandledClipboardUpdateReturnCode = IntPtr.Zero;
        public static readonly IntPtr NoMessageHandledReturnCode = IntPtr.Zero;

        // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr windowHandle);

        // https://msdn.microsoft.com/en-us/library/windows/desktop/dd542643(v=vs.85).aspx
        private static uint CLIPBRD_E_CANT_OPEN = 0x800401D0;
        public static int UnableToOpenClipboardComErrorCode = (int)CLIPBRD_E_CANT_OPEN;
    }
}
