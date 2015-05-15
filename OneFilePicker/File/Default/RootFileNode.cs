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

    /// <summary>
    /// This is the default when no custom INode is provided to <see cref="FileDialog"/>.
    /// </summary>
    public class RootFileNode : INode
    {
        public INode Parent { get { return null; } }

        public INode[] Children
        {
            get
            {
                return DriveInfo.GetDrives().Select(d => (INode)new FileNode(this, d.Name, d.Name, d.Name)).ToArray();
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
            Icon = ImageLoader.GetComputerImage(true);
            DisplayName = "Computer";
        }
    }
}
