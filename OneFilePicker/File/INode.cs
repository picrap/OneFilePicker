#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File
{
    using System;
    using System.Windows.Media;

    /// <summary>
    /// Node interface
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        INode Parent { get; }
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        INode[] Children { get; }
        /// <summary>
        /// Gets the directory children.
        /// </summary>
        /// <value>
        /// The directory children.
        /// </value>
        INode[] FolderChildren { get; }
        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        ImageSource Icon { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is directory.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directory; otherwise, <c>false</c>.
        /// </value>
        bool IsFolder { get; }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        string DisplayName { get; }
        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        string Path { get; }
        /// <summary>
        /// Gets the last write time.
        /// </summary>
        /// <value>
        /// The last write time.
        /// </value>
        DateTime LastWriteTime { get; }
        /// <summary>
        /// Gets the display type.
        /// </summary>
        /// <value>
        /// The display type.
        /// </value>
        string DisplayType { get; }
        /// <summary>
        /// Gets the length kb.
        /// </summary>
        /// <value>
        /// The length kb.
        /// </value>
        long? LengthKB { get; }
    }
}
