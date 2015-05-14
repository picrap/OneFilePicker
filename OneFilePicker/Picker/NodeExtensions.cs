#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.Picker
{
    using System.Collections.Generic;
    using System.Linq;

    public static class NodeExtensions
    {
        /// <summary>
        /// Gets the self and ascendants.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static IEnumerable<INode> GetSelfAndAscendants(this INode node)
        {
            for (var currentNode = node; currentNode != null; currentNode = currentNode.Parent)
                yield return currentNode;
        }

        /// <summary>
        /// Gets the path of the current node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public static string[] GetPath(this INode node)
        {
            return GetSelfAndAscendants(node).Reverse().Select(n => n.Name).ToArray();
        }
    }
}
