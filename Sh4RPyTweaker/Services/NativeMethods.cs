using System;
using System.Runtime.InteropServices;

namespace Sh4RPyTweaker.Services
{
    public static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHQUERYRBINFO
        {
            public int cbSize;
            public long i64Size;
            public long i64NumItems;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHQueryRecycleBin(string pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;
        public const int TokenUser = 1;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr processHandle, uint desiredAccess, out IntPtr tokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool GetTokenInformation(IntPtr tokenHandle, int tokenInformationClass,
            IntPtr tokenInformation, uint tokenInformationLength, out uint returnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LookupAccountSid(string lpSystemName, byte[] sid,
            System.Text.StringBuilder lpName, ref uint cchName,
            System.Text.StringBuilder lpReferencedDomainName, ref uint cchReferencedDomainName,
            out int peUse);

        [DllImport("advapi32.dll")]
        public static extern uint GetLengthSid(IntPtr pSid);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public static bool TryGetAccountSid(IntPtr processHandle, out byte[] sid)
        {
            sid = null;
            IntPtr token;
            if (!OpenProcessToken(processHandle, PROCESS_QUERY_LIMITED_INFORMATION, out token)) return false;
            try
            {
                uint len = 0;
                GetTokenInformation(token, TokenUser, IntPtr.Zero, 0, out len);
                if (len < IntPtr.Size) return false;

                IntPtr buffer = Marshal.AllocHGlobal((int)len);
                try
                {
                    if (!GetTokenInformation(token, TokenUser, buffer, len, out len)) return false;
                    IntPtr sidPtr = Marshal.ReadIntPtr(buffer);
                    if (sidPtr == IntPtr.Zero) return false;

                    uint sidLen = GetLengthSid(sidPtr);
                    if (sidLen == 0 || sidLen > 256) return false;
                    sid = new byte[sidLen];
                    Marshal.Copy(sidPtr, sid, 0, (int)sidLen);
                    return true;
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            finally
            {
                CloseHandle(token);
            }
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr hWnd, uint Msg, UIntPtr wParam, string lParam,
            uint fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        public const uint WM_SETTINGCHANGE = 0x001A;
        public const uint SMTO_ABORTIFHUNG = 0x0002;

        public const uint SHERB_NOCONFIRMATION = 0x00000001;
        public const uint SHERB_NOPROGRESSUI = 0x00000002;
        public const uint SHERB_NOSOUND = 0x00000004;
    }
}

