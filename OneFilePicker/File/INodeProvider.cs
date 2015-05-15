#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File
{
    public interface INodeProvider
    {
        /// <summary>
        /// Gets the root node.
        /// </summary>
        /// <value>
        /// The root.
        /// </value>
        INode[] Root { get; }
        /// <summary>
        /// Finds a node by its path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        INode Find(string path);
    }
}
