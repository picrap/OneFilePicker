﻿
namespace OneFilePicker.File
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    internal static partial class ImageLoader
    {
        private static readonly IDictionary<string, ImageSource> Cache = new Dictionary<string, ImageSource>();

        private static ImageSource GetImage(string key, bool large, Func<ImageSource> loadImage)
        {
            lock (Cache)
            {
                if (large)
                    key += "+";
                else
                    key += "-";
                ImageSource image;
                if (!Cache.TryGetValue(key, out image))
                    Cache[key] = image = loadImage();
                return image;
            }
        }

        #region Special Images

        internal static ImageSource GetComputerImage(bool large)
        {
            return GetImage(":computer", large, () => GetImage("shell32.dll", 15, large));
        }

        internal static ImageSource GetFavoritesImage(bool large)
        {
            return GetImage(":favorites", large, () => GetImage("shell32.dll", 43, large));
        }

        internal static ImageSource GetFolderImage(bool large)
        {
            return GetImage(":folder", large, () => GetImage("shell32.dll", 4, large));
        }

        internal static ImageSource GetNetworkNeighborhoodImage(bool large)
        {
            return GetImage(":network", large, () => GetImage("shell32.dll", 17, large));
        }

        internal static ImageSource GetWarningImage(bool large)
        {
            return GetImage(":warning", large, () => GetImage("user32.dll", 1, large));
        }

        internal static ImageSource GetQuestionImage(bool large)
        {
            return GetImage(":question", large, () => GetImage("user32.dll", 2, large));
        }

        internal static ImageSource GetErrorImage(bool large)
        {
            return GetImage(":error", large, () => GetImage("user32.dll", 3, large));
        }

        internal static ImageSource GetInformationImage(bool large)
        {
            return GetImage(":information", large, () => GetImage("user32.dll", 4, large));
        }

        internal static ImageSource GetLibrariesImage(bool large)
        {
            return GetImage(":libraries", large, () => GetImage("imageres.dll", 202, large));
        }

        #endregion

        private static ImageSource GetImage(string file, int index, bool large)
        {
            IntPtr hImgLarge = IntPtr.Zero; //the handle to the system image list
            IntPtr hImgSmall = IntPtr.Zero; //the handle to the system image list
            ImageSource image;
            try
            {
                int count = Win32.ExtractIconEx(file, index, ref hImgLarge, ref hImgSmall, 1);
                if (large)
                {
                    image = GetImage(hImgLarge);
                }
                else
                {
                    image = GetImage(hImgSmall);
                }
            }
            finally
            {
                Win32.DestroyIcon(hImgLarge);
                Win32.DestroyIcon(hImgSmall);
            }

            return image;
        }

        internal static ImageSource GetImage(string filename, bool large)
        {
            IntPtr hImg = IntPtr.Zero;
            try
            {
                var shinfo = new SHFILEINFO();
                var iconSize = large ? Win32.SHGFI_LARGEICON : Win32.SHGFI_SMALLICON;
                hImg = Win32.SHGetFileInfo(filename, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo),
                    Win32.SHGFI_ICON | iconSize );
                var image = GetImage(shinfo.hIcon);
                return image;
            }
            finally
            {
                Win32.DestroyIcon(hImg);
            }
        }

        private static ImageSource GetImage(IntPtr hIcon)
        {
            try
            {
                //The icon is returned in the hIcon member of the shinfo struct
                var icon = System.Drawing.Icon.FromHandle(hIcon);
                var image = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return image;
            }
            catch
            {
                return null;
            }
        }

        internal static string GetFileType(string filename)
        {
            var shinfo = new SHFILEINFO();
            Win32.SHGetFileInfo(filename, Win32.FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_TYPENAME | Win32.SHGFI_USEFILEATTRIBUTES);
            return shinfo.szTypeName;
        }
    }
}