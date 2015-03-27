#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File
{
    using System;
    using System.Runtime.InteropServices;

    partial class ImageLoader
    {
        // http://support.microsoft.com/kb/319350
        [StructLayout(LayoutKind.Sequential)]
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

        internal class Win32
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;

            internal const uint SHGFI_SYSICONINDEX = 0x000004000;
            internal const int ILD_TRANSPARENT = 0x1;

            internal const uint SHGFI_ICON = 0x100;
            internal const uint SHGFI_LARGEICON = 0x0; // 'Large icon
            internal const uint SHGFI_SMALLICON = 0x1; // 'Small icon

            [DllImport("shell32")]
            internal static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("shell32", CharSet = CharSet.Auto)]
            internal static extern int ExtractIconEx(string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons);

            [DllImport("comctl32", SetLastError = true)]
            internal static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, int flags);

            [DllImport("user32")]
            internal static extern bool DestroyIcon(IntPtr hIcon);
        }

    }
}
