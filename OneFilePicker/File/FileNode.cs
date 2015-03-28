#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using Picker;

    public class FileNode : INode
    {
        private INode[] _children;
        public INode[] Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new INode[0];
                    if (IsFolder)
                    {
                        try
                        {
                            _children = Directory.EnumerateFileSystemEntries(Path).Where(IsVisible).Select(CreateChild).ToArray();
                        }
                        catch (UnauthorizedAccessException)
                        { }
                    }
                }
                return _children;
            }
        }

        private static bool IsVisible(string path)
        {
            var attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.System))
                return false;
            return true;
        }

        private INode CreateChild(string path)
        {
            return new FileNode(path);
        }

        public INode[] FolderChildren
        {
            get { return Children.Where(c => c.IsFolder).ToArray(); }
        }

        private ImageSource _icon;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                    _icon = ImageLoader.GetImage(Path, false);
                return _icon;
            }
        }

        public bool IsFolder { get; private set; }

        public string DisplayName { get; private set; }

        public string Path { get; private set; }

        public DateTime LastWriteTime { get; private set; }

        public string DisplayType { get; private set; }

        public long? LengthKB { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNode"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="displayName">The display name.</param>
        public FileNode(string path, string displayName = null)
        {
            Path = path;
            DisplayName = displayName ?? System.IO.Path.GetFileName(path);
            DisplayType = ImageLoader.GetFileType(path);
            IsFolder = Directory.Exists(path);
            if (IsFolder)
                LastWriteTime = new DirectoryInfo(path).LastWriteTime;
            else
            {
                var fileInfo = new FileInfo(path);
                LengthKB = fileInfo.Length >> 10;
                LastWriteTime = fileInfo.LastWriteTime;
            }
        }
    }
}
