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
    using ArxOne.OneFilesystem;

    public class OneNodeProvider : INodeProvider
    {
        private readonly OneFilesystem _filesystem;

        public INode[] Root { get; private set; }

        public OneNodeProvider()
        {
            _filesystem = new OneFilesystem();
            Root = _filesystem.GetChildren("file://").Select(CreateNode).ToArray();
        }

        private INode CreateNode(OneEntryInformation information)
        {
            return new OneNode(information, null, _filesystem);
        }

        public INode Find(string path)
        {
            var onePath = new OnePath(path);
            INode[] children = Root;
            INode child = null;
            foreach (var part in onePath.Path)
            {
                child = children.SingleOrDefault(c => c.Name == part);
                if (child == null)
                    break;
                children = child.Children;
            }
            return child;
        }
    }
}
