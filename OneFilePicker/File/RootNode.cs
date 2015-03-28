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

    /// <summary>
    /// This is the default when no custom INode is provided to <see cref="FileDialog"/>.
    /// </summary>
    public class RootNode : INode
    {
        public INode[] Children { get; private set; }

        public INode[] FolderChildren { get { return Children != null ? Children.Where(c => c.IsFolder).ToArray() : null; } }

        public ImageSource Icon { get; private set; }

        public bool IsFolder { get { return true; } }

        public string DisplayName { get; private set; }

        public string Path
        {
            get
            {
                return "";
            }
        }

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

        public RootNode()
        {
            Children = DriveInfo.GetDrives().Select(d => (INode)new FileNode(d.Name, d.Name)).ToArray();
            Icon = ImageLoader.GetComputerImage(true);
            DisplayName = "Computer";
        }
    }
}
