#region OneFilePicker
// OneFilePicker
// Abstract (extensible) file picker
// https://github.com/picrap/OneFilePicker
// Released under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace OneFilePicker.Picker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using File;
    using File.Default;

    public partial class FileDialog
    {
        public static readonly DependencyProperty FileNameLabelProperty = DependencyProperty.Register(
            "FileNameLabel", typeof(string), typeof(FileDialog), new PropertyMetadata("File name:"));

        public string FileNameLabel
        {
            get { return (string)GetValue(FileNameLabelProperty); }
            set { SetValue(FileNameLabelProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName", typeof(string), typeof(FileDialog));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter", typeof(string), typeof(FileDialog), new PropertyMetadata("All files|*.*", OnFilterChanged));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterIndexProperty = DependencyProperty.Register(
            "FilterIndex", typeof(int), typeof(FileDialog), new PropertyMetadata(default(int)));

        public int FilterIndex
        {
            get { return (int)GetValue(FilterIndexProperty); }
            set { SetValue(FilterIndexProperty, value); }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(
            "Filters", typeof(Filter[]), typeof(FileDialog));

        public Filter[] Filters
        {
            get { return (Filter[])GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public static readonly DependencyProperty SelectTextProperty = DependencyProperty.Register(
            "SelectText", typeof(string), typeof(FileDialog), new PropertyMetadata("Open"));

        public string SelectText
        {
            get { return (string)GetValue(SelectTextProperty); }
            set { SetValue(SelectTextProperty, value); }
        }

        public static readonly DependencyProperty SelectProperty = DependencyProperty.Register(
            "Select", typeof(ICommand), typeof(FileDialog));

        public ICommand Select
        {
            get { return (ICommand)GetValue(SelectProperty); }
            set { SetValue(SelectProperty, value); }
        }

        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register(
            "CancelText", typeof(string), typeof(FileDialog), new PropertyMetadata("Cancel"));

        public string CancelText
        {
            get { return (string)GetValue(CancelTextProperty); }
            set { SetValue(CancelTextProperty, value); }
        }

        public static readonly DependencyProperty CancelProperty = DependencyProperty.Register(
            "Cancel", typeof(ICommand), typeof(FileDialog));

        public ICommand Cancel
        {
            get { return (ICommand)GetValue(CancelProperty); }
            set { SetValue(CancelProperty, value); }
        }

        public static readonly DependencyProperty NodeProviderProperty = DependencyProperty.Register(
            "NodeProvider", typeof(INodeProvider), typeof(FileDialog), new PropertyMetadata(new FileNodeProvider()));

        public INodeProvider NodeProvider
        {
            get { return (INodeProvider)GetValue(NodeProviderProperty); }
            set { SetValue(NodeProviderProperty, value); }
        }

        public static readonly DependencyProperty SelectedFolderProperty
            = DependencyProperty.Register("SelectedFolder", typeof(INode), typeof(FileDialog),
                new PropertyMetadata(null, (d, e) => { ((FileDialog)d).OnSelectedFolderChanged(); }));

        public INode SelectedFolder
        {
            get { return (INode)GetValue(SelectedFolderProperty); }
            set { SetValue(SelectedFolderProperty, value); }
        }

        public static readonly DependencyProperty CanNavigateBackProperty
            = DependencyProperty.Register("CanNavigateBack", typeof(bool), typeof(FileDialog), new PropertyMetadata(false));
        public bool CanNavigateBack
        {
            get { return (bool)GetValue(CanNavigateBackProperty); }
            set { SetValue(CanNavigateBackProperty, value); }
        }

        public static readonly DependencyProperty CanNavigateForwardProperty
            = DependencyProperty.Register("CanNavigateForward", typeof(bool), typeof(FileDialog), new PropertyMetadata(false));
        public bool CanNavigateForward
        {
            get { return (bool)GetValue(CanNavigateForwardProperty); }
            set { SetValue(CanNavigateForwardProperty, value); }
        }

        public static readonly DependencyProperty CanNavigateUpProperty
            = DependencyProperty.Register("CanNavigateUp", typeof(bool), typeof(FileDialog), new PropertyMetadata(false));
        public bool CanNavigateUp
        {
            get { return (bool)GetValue(CanNavigateUpProperty); }
            set { SetValue(CanNavigateUpProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDialog"/> class.
        /// </summary>
        public FileDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            OnFilterChanged();
        }

        /// <summary>
        /// Called when [filter changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fileDialog = (FileDialog)d;
            fileDialog.OnFilterChanged();
        }

        /// <summary>
        /// Called when [filter changed].
        /// </summary>
        private void OnFilterChanged()
        {
            var filtersParts = Filter.Split('|').Select(f => f.Trim()).ToArray();
            var filters = new List<Filter>();
            for (int index = 0; index < filtersParts.Length; index += 2)
                filters.Add(Picker.Filter.FromLiteral(filtersParts[index], filtersParts[index + 1]));
            Filters = filters.ToArray();
        }

        // history of selected nodes
        private readonly IList<INode> _history = new List<INode>();
        // index points at the current node
        private int _historyIndex = -1;
        private bool _selectingNode = false;
        private bool _updatingHistory = false;

        // workflow:
        // NodeTree.SelectedItem --> SelectedFolder --> History --> check for NodeTree.SelectedItem --> Binding to list view
        // if origin is history:
        // History --> SelectedFolder --> check for NodeTree.SelectedItem --> Binding to list view
        // so all combined:
        // source NodeTree.SelectedItem or History --> SelectedFolder --> [History] --> check for NodeTree.SelectedItem --> Binding to list view

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if (_historyIndex < 0)
                return;
            NoHistory(() => SelectedFolder = _history[--_historyIndex]);
        }

        private void NavigateForward(object sender, RoutedEventArgs e)
        {
            NoHistory(() => SelectedFolder = _history[++_historyIndex]);
        }

        private void NoHistory(Action action)
        {
            try
            {
                _updatingHistory = true;
                action();
            }
            finally
            {
                _updatingHistory = false;
            }
        }

        private void NavigateUp(object sender, RoutedEventArgs e)
        {
            if (SelectedFolder == null || SelectedFolder.Parent == null)
                return;
            SelectedFolder = SelectedFolder.Parent;
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            var bindingExpression = FilesList.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (bindingExpression != null)
                bindingExpression.UpdateTarget();
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFolder = (INode)NodeTree.SelectedItem;
        }

        private void OnSelectedFolderChanged()
        {
            if (_selectingNode)
                return;
            UpdateHistory();
            CheckSelectedNode();
            UpdateCanNavigate();
        }

        private void CheckSelectedNode()
        {
            if (SelectedFolder == null || (NodeTree.SelectedItem != null && (SelectedFolder.Path == ((INode)NodeTree.SelectedItem).Path)))
                return;
            try
            {
                _selectingNode = true;
                var items = NodeTree.Items;
                var containerGenerator = NodeTree.ItemContainerGenerator;
                var path = SelectedFolder.GetPath();
                for (int partIndex = 0; partIndex < path.Length; partIndex++)
                {
                    bool isLast = partIndex == path.Length - 1;
                    var node = items.Cast<INode>().SingleOrDefault(i => i.Name == path[partIndex]);
                    if (node == null)
                        break;

                    var item = (TreeViewItem)containerGenerator.ContainerFromItem(node);
                    if (!isLast)
                        item.IsExpanded = true;
                    else
                        item.IsSelected = true;

                    containerGenerator = item.ItemContainerGenerator;
                    items = item.Items;
                }
            }
            finally
            {
                _selectingNode = false;
            }
        }

        private void UpdateHistory()
        {
            if (_updatingHistory)
                return;
            while (_history.Count > _historyIndex + 1)
                _history.RemoveAt(_historyIndex + 1);
            _history.Insert(++_historyIndex, SelectedFolder);
        }

        private void UpdateCanNavigate()
        {
            CanNavigateBack = _historyIndex > 0;
            CanNavigateForward = _historyIndex < _history.Count - 1;
            CanNavigateUp = SelectedFolder != null && SelectedFolder.Parent != null;
        }
    }
}
