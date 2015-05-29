#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.File.One
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Annotations;
    using ArxOne.OneFilesystem;

    public class OneNodeProvider : INodeProvider, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly OneFilesystem _filesystem;

        private OneNode[] _root;

        public INode[] Root { get { return _root; } }

        public OneNodeProvider()
        {
            _filesystem = new OneFilesystem();
            _root = _filesystem.GetChildren("file://").Select(CreateNode).ToArray();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var onPropertyChanged = PropertyChanged;
            if (onPropertyChanged != null)
                onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private OneNode CreateNode(OneEntryInformation information)
        {
            return new OneNode(information, null, _filesystem);
        }

        private OneNode GetRoot(Uri uri)
        {
            var existingRoot =
                _root.SingleOrDefault(r => string.Equals(r.OnePath.Protocol, uri.Scheme, StringComparison.InvariantCultureIgnoreCase)
                                           && string.Equals(r.OnePath.Host, uri.Host, StringComparison.InvariantCultureIgnoreCase));
            if (existingRoot != null)
                return existingRoot;

            var information = _filesystem.GetInformation(new Uri(uri, "/"));
            if (information == null)
                return null;

            var newRoot = new OneNode(information, null, _filesystem);
            _root = _root.Concat(new[] { newRoot }).ToArray();
            OnPropertyChanged("Root");
            return newRoot;
        }

        /// <summary>
        /// Finds a node by its path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public INode Find(string path)
        {
            var onePath = new OnePath(path);
            var root = GetRoot(onePath.Uri);
            if (root == null)
                return null;
            INode node = root;
            foreach (var part in onePath.Path)
            {
                node = node.Children.SingleOrDefault(c => string.Equals(c.Name, part, StringComparison.InvariantCultureIgnoreCase));
                if (node == null)
                    break;
            }
            return node;
        }
    }
}
