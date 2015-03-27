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
                            _children = Directory.EnumerateFileSystemEntries(Path).Select(CreateChild).ToArray();
                        }
                        catch (UnauthorizedAccessException)
                        { }
                    }
                }
                return _children;
            }
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

        private bool? _isDirectory;
        public bool IsFolder
        {
            get
            {
                if (!_isDirectory.HasValue)
                    _isDirectory = Directory.Exists(Path);
                return _isDirectory.Value;
            }
        }

        public string DisplayName { get; private set; }

        public string Path { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNode"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="displayName">The display name.</param>
        public FileNode(string path, string displayName = null)
        {
            Path = path;
            DisplayName = displayName ?? System.IO.Path.GetFileName(path);
        }
    }
}
