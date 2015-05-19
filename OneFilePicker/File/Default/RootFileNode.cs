#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File.Default
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Media;
    using Picker;
    using Services;

    /// <summary>
    /// This is the default when no custom INode is provided to <see cref="FilePicker"/>.
    /// </summary>
    public class RootFileNode : INode
    {
        public INode Parent { get { return null; } }

        public INode[] Children
        {
            get
            {
                return (from driveInfo in DriveInfo.GetDrives()
                        let name = driveInfo.Name.TrimEnd('\\')
                        let displayName = ShellInfo.GetDisplayName(driveInfo.Name)
                        select (INode)new FileNode(this, name, name, displayName))
                        .ToArray();
            }
        }

        public INode[] FolderChildren { get { return Children != null ? Children.Where(c => c.IsFolder).ToArray() : null; } }

        public ImageSource Icon { get; private set; }

        public bool IsFolder { get { return true; } }

        public string Name { get { return ""; } }

        public string DisplayName { get; private set; }

        public string Path { get { return ""; } }

        public DateTime LastWriteTime
        {
            get { return DateTime.MinValue; }
        }

        public string DisplayType
        {
            get { return ""; }
        }

        public long? LengthKB
        {
            get { return null; }
        }

        public RootFileNode()
        {
            Icon = ShellInfo.GetComputerImage(true);
            DisplayName = "Computer";
        }
    }
}
