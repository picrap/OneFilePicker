#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.Picker
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Annotations;
    using File;

    public class TreeViewNode : INotifyPropertyChanged
    {
        private readonly ISet<string> _expanded;

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>
        /// The node.
        /// </value>
        public INode Node { get; set; }

        private TreeViewNode[] _children;

        public TreeViewNode[] Children { get { return _children ?? (_children = Node.Children.Select(CreateChild).ToArray()); } }

        public TreeViewNode[] FolderChildren { get { return Children.Where(c => c.Node.IsFolder).ToArray(); } }

        public bool IsExpanded
        {
            get { return _expanded.Contains(Node.Path); }
            set
            {
                if (value != IsExpanded)
                {
                    if (value)
                        _expanded.Add(Node.Path);
                    else
                        _expanded.Remove(Node.Path);
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewNode"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="expanded">The expaned nodes information.</param>
        public TreeViewNode(INode node, ISet<string> expanded)
        {
            _expanded = expanded;
            Node = node;
        }

        private TreeViewNode CreateChild(INode node)
        {
            return new TreeViewNode(node, _expanded);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var onPropertyChanged = PropertyChanged;
            if (onPropertyChanged != null)
                onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Refresh()
        {
            _children = null;
            OnPropertyChanged("Children");
            OnPropertyChanged("FolderChildren");
        }
    }
}
