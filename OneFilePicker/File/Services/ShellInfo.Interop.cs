#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File.Services
{
    using System;
    using System.Runtime.InteropServices;

    partial class ShellInfo
    {
        // http://support.microsoft.com/kb/319350
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        internal static class Win32
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            public const uint SHGFI_DISPLAYNAME = 0x000000200;

            public const uint SHGFI_SYSICONINDEX = 0x000004000;
            public const int ILD_TRANSPARENT = 0x1;

            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            public const uint SHGFI_SMALLICON = 0x1; // 'Small icon
            public const uint SHGFI_PIDL = 0x0008;    // pszPath is a pidl

            public const uint CSIDL_DRIVES = 0x0011; // My Computer

            [DllImport("shell32", CharSet = CharSet.Auto)]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            public static extern IntPtr SHGetFileInfo(IntPtr ppidl, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            public static extern int ExtractIconEx(string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);

            [DllImport("comctl32", SetLastError = true)]
            public static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, int flags);

            [DllImport("user32")]
            public static extern bool DestroyIcon(IntPtr hIcon);

            [DllImport("shell32")]
            public static extern int SHGetFolderLocation(IntPtr hwndOwner, uint nFolder, IntPtr hToken, uint dwReserved, [In][Out] ref IntPtr ppidl);

            [DllImport("shell32")]
            public static extern void ILFree(IntPtr ppidl);
        }

    }
}
