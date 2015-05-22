#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File.One
{
    using System;
    using System.Linq;
    using System.Windows.Media;
    using ArxOne.OneFilesystem;
    using Services;

    public class OneNode : INode
    {
        private readonly OneFilesystem _filesystem;

        public INode Parent { get; private set; }

        public INode[] Children
        {
            get { return _filesystem.GetChildren(Path).Select(CreateNode).ToArray(); }
        }

        public INode[] FolderChildren
        {
            get { return Children.Where(c => c.IsFolder).ToArray(); }
        }

        public ImageSource Icon { get; private set; }

        public bool IsFolder { get; private set; }

        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Path { get; private set; }

        public DateTime LastWriteTime { get; private set; }

        public string DisplayType { get; private set; }

        public long? LengthKB { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneNode" /> class.
        /// </summary>
        /// <param name="information">The information.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="filesystem">The filesystem.</param>
        public OneNode(OneEntryInformation information, INode parent, OneFilesystem filesystem)
        {
            _filesystem = filesystem;
            Parent = parent;
            var fileNameOrPath = GetShellFileNameOrPath(information);
            if (information.Path.Count == 0)
            {
                Icon = ShellInfo.GetComputerImage(false);
                if (string.Equals(information.Host, "localhost", StringComparison.InvariantCultureIgnoreCase))
                    DisplayName = ShellInfo.GetComputerDisplayName();
                else
                    DisplayName = information.Host;
            }
            else
            {
                Icon = ShellInfo.GetIcon(fileNameOrPath, false);
                DisplayName = ShellInfo.GetDisplayName(fileNameOrPath) ?? information.Name;
                DisplayType = ShellInfo.GetFileType(fileNameOrPath);
            }
            IsFolder = information.IsDirectory;
            Name = information.Name;
            Path = information.Uri.ToString();
            LastWriteTime = information.LastWriteTimeUtc ?? DateTime.MinValue;
            if (!information.IsDirectory)
                LengthKB = information.Length >> 10;
        }

        /// <summary>
        /// Gets the shell file name or path (path is better).
        /// </summary>
        /// <param name="information">The information.</param>
        /// <returns></returns>
        private static string GetShellFileNameOrPath(OneEntryInformation information)
        {
            if (string.IsNullOrEmpty(information.Uri.LocalPath))
                return information.Name;
            return information.Uri.LocalPath;
        }

        /// <summary>
        /// Creates an <see cref="INode"/> from a <see cref="OneEntryInformation"/>.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private INode CreateNode(OneEntryInformation c)
        {
            return new OneNode(c, this, _filesystem);
        }
    }
}
