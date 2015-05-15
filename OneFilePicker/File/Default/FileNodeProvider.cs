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

    /// <summary>
    /// Default file node provider
    /// </summary>
    public class FileNodeProvider : INodeProvider
    {
        public INode[] Root { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNodeProvider"/> class.
        /// </summary>
        public FileNodeProvider()
        {
            Root = new INode[] { new RootFileNode() };
        }

        /// <summary>
        /// Finds a node by its path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public INode Find(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var pathParts = Parse(fullPath);
            INode[] nodes = Root;
            INode node = null;
            foreach (var pathPart in pathParts)
            {
                node = nodes.SingleOrDefault(n => string.Equals(n.Name, pathPart, StringComparison.InvariantCultureIgnoreCase));
                if (node == null)
                    return null;
                nodes = node.Children;
            }
            return node;
        }

        /// <summary>
        /// Parses the specified full path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public static string[] Parse(string fullPath)
        {
            if (fullPath.StartsWith("@\\?"))
                fullPath = fullPath.Substring(3);
            bool isServer = fullPath.StartsWith(@"\\");
            var parts = fullPath.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (!isServer)
                return new[] { "" }.Concat(parts).ToArray();
            return parts;
        }
    }
}
