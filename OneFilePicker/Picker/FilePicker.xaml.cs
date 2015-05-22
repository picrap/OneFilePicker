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
    using File.One;

    public partial class FilePicker
    {
        public static readonly DependencyProperty FileNameLabelProperty = DependencyProperty.Register(
            "FileNameLabel", typeof(string), typeof(FilePicker), new PropertyMetadata("File name:"));

        public string FileNameLabel
        {
            get { return (string)GetValue(FileNameLabelProperty); }
            set { SetValue(FileNameLabelProperty, value); }
        }

        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
            "FileName", typeof(string), typeof(FilePicker));

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter", typeof(string), typeof(FilePicker), new PropertyMetadata("All files|*.*", OnFilterChanged));

        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterIndexProperty = DependencyProperty.Register(
            "FilterIndex", typeof(int), typeof(FilePicker), new PropertyMetadata(default(int)));

        public int FilterIndex
        {
            get { return (int)GetValue(FilterIndexProperty); }
            set { SetValue(FilterIndexProperty, value); }
        }

        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(
            "Filters", typeof(Filter[]), typeof(FilePicker));

        public Filter[] Filters
        {
            get { return (Filter[])GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public static readonly DependencyProperty SelectTextProperty = DependencyProperty.Register(
            "SelectText", typeof(string), typeof(FilePicker), new PropertyMetadata("Open"));

        public string SelectText
        {
            get { return (string)GetValue(SelectTextProperty); }
            set { SetValue(SelectTextProperty, value); }
        }

        public static readonly DependencyProperty SelectProperty = DependencyProperty.Register(
            "Select", typeof(ICommand), typeof(FilePicker));

        public ICommand Select
        {
            get { return (ICommand)GetValue(SelectProperty); }
            set { SetValue(SelectProperty, value); }
        }

        public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register(
            "CancelText", typeof(string), typeof(FilePicker), new PropertyMetadata("Cancel"));

        public string CancelText
        {
            get { return (string)GetValue(CancelTextProperty); }
            set { SetValue(CancelTextProperty, value); }
        }

        public static readonly DependencyProperty CancelProperty = DependencyProperty.Register(
            "Cancel", typeof(ICommand), typeof(FilePicker));

        public ICommand Cancel
        {
            get { return (ICommand)GetValue(CancelProperty); }
            set { SetValue(CancelProperty, value); }
        }

        public static readonly DependencyProperty NodeProviderProperty = DependencyProperty.Register(
            "NodeProvider", typeof(INodeProvider), typeof(FilePicker),
            new PropertyMetadata(null, (d, e) => ((FilePicker)d).OnNodeProviderChanged()));

        public INodeProvider NodeProvider
        {
            get { return (INodeProvider)GetValue(NodeProviderProperty); }
            set { SetValue(NodeProviderProperty, value); }
        }

        public static readonly DependencyProperty RootNodesProperty = DependencyProperty.Register(
            "RootNodes", typeof(TreeViewNode[]), typeof(FilePicker), new PropertyMetadata(default(TreeViewNode[])));

        public TreeViewNode[] RootNodes
        {
            get { return (TreeViewNode[])GetValue(RootNodesProperty); }
            set { SetValue(RootNodesProperty, value); }
        }

        public static readonly DependencyProperty SelectedFolderProperty
            = DependencyProperty.Register("SelectedFolder", typeof(INode), typeof(FilePicker),
                new PropertyMetadata(null, (d, e) => ((FilePicker)d).OnSelectedFolderChanged()));

        public INode SelectedFolder
        {
            get { return (INode)GetValue(SelectedFolderProperty); }
            set { SetValue(SelectedFolderProperty, value); }
        }

        public static readonly DependencyProperty SelectedFolderPathProperty = DependencyProperty.Register(
            "SelectedFolderPath", typeof(string), typeof(FilePicker),
            new PropertyMetadata(null, (d, e) => ((FilePicker)d).OnSelectedFolderPathChanged()));

        public string SelectedFolderPath
        {
            get { return (string)GetValue(SelectedFolderPathProperty); }
            set { SetValue(SelectedFolderPathProperty, value); }
        }

        public static readonly DependencyProperty CanNavigateBackProperty
            = DependencyProperty.Register("CanNavigateBack", typeof(bool), typeof(FilePicker), new PropertyMetadata(false));

        public bool CanNavigateBack
        {
            get { return (bool)GetValue(CanNavigateBackProperty); }
            set { SetValue(CanNavigateBackProperty, value); }
        }

        public static readonly DependencyProperty CanNavigateForwardProperty
            = DependencyProperty.Register("CanNavigateForward", typeof(bool), typeof(FilePicker), new PropertyMetadata(false));
        public bool CanNavigateForward
        {
            get { return (bool)GetValue(CanNavigateForwardProperty); }
            set { SetValue(CanNavigateForwardProperty, value); }
        }

        public static readonly DependencyProperty CanNavigateUpProperty
            = DependencyProperty.Register("CanNavigateUp", typeof(bool), typeof(FilePicker), new PropertyMetadata(false));

        public bool CanNavigateUp
        {
            get { return (bool)GetValue(CanNavigateUpProperty); }
            set { SetValue(CanNavigateUpProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(FilePickerMode), typeof(FilePicker),
            new PropertyMetadata(FilePickerMode.Default, (d, e) => ((FilePicker)d).OnModeChanged()));

        public FilePickerMode Mode
        {
            get { return (FilePickerMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        private bool ShowFilesList
        {
            get { return Mode == FilePickerMode.Default; }
        }

        private TreeView CurrentFoldersView
        {
            get { return ShowFilesList ? FullViewFoldersTree : FolderViewFoldersTree; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilePicker"/> class.
        /// </summary>
        public FilePicker()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            NodeProvider = new OneNodeProvider();
        }

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            OnModeChanged();
            OnFilterChanged();
            OnSelectedFolderPathChanged();
        }

        /// <summary>
        /// Called when [filter changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fileDialog = (FilePicker)d;
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
        private bool _selectingNode;
        private bool _updatingHistory;

        // workflow:
        // NodeTree.SelectedItem --> SelectedFolder --> History --> check for NodeTree.SelectedItem --> Binding to list view
        // if origin is history:
        // History --> SelectedFolder --> check for NodeTree.SelectedItem --> Binding to list view
        // so all combined:
        // source NodeTree.SelectedItem or History --> SelectedFolder --> [History] --> check for NodeTree.SelectedItem --> Binding to list view

        /// <summary>
        /// Navigates back.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if (_historyIndex < 0)
                return;
            NoHistory(() => SelectedFolder = _history[--_historyIndex]);
        }

        /// <summary>
        /// Navigates forward.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NavigateForward(object sender, RoutedEventArgs e)
        {
            NoHistory(() => SelectedFolder = _history[++_historyIndex]);
        }

        /// <summary>
        /// Invokes the action without updating the history.
        /// </summary>
        /// <param name="action">The action.</param>
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

        /// <summary>
        /// Navigates up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void NavigateUp(object sender, RoutedEventArgs e)
        {
            if (SelectedFolder == null || SelectedFolder.Parent == null)
                return;
            SelectedFolder = SelectedFolder.Parent;
        }

        /// <summary>
        /// Refreshes the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Refresh(object sender, RoutedEventArgs e)
        {
            var treeViewNode = (TreeViewNode)CurrentFoldersView.SelectedItem;
            if (treeViewNode == null)
                return;
            treeViewNode.Refresh();
            //var bindingExpression = FilesList.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            //if (bindingExpression != null)
            //    bindingExpression.UpdateTarget();
        }

        /// <summary>
        /// Called when treeview selected item changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="object"/> instance containing the event data.</param>
        private void OnFullSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!ShowFilesList)
                return;
            OnSelectedItemChanged((TreeView)sender);
        }

        /// <summary>
        /// Called when treeview selected item changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="object"/> instance containing the event data.</param>
        private void OnFolderSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ShowFilesList)
                return;
            OnSelectedItemChanged((TreeView)sender);
        }

        private void OnSelectedItemChanged(TreeView sender)
        {
            var treeViewNode = (TreeViewNode)sender.SelectedItem;
            if (treeViewNode == null)
                return;
            SelectedFolder = treeViewNode.Node;
        }

        /// <summary>
        /// Called when <see cref="SelectedFolder"/> has changed.
        /// </summary>
        private void OnSelectedFolderChanged()
        {
            if (_selectingNode)
                return;
            Location.Text = SelectedFolder.Path;
            SelectedFolderPath = SelectedFolder.Path;
            UpdateHistory();
            CheckSelectedNode();
            UpdateCanNavigate();
        }

        /// <summary>
        /// Checks if tree node matches the <see cref="SelectedFolder"/>.
        /// </summary>
        private void CheckSelectedNode()
        {
            if (SelectedFolder == null ||
                (CurrentFoldersView.SelectedItem != null && (SelectedFolder.Path == ((TreeViewNode)CurrentFoldersView.SelectedItem).Node.Path)))
                return;
            try
            {
                _selectingNode = true;
                var nodes = RootNodes;
                var path = SelectedFolder.GetPath();
                for (int partIndex = 0; partIndex < path.Length; partIndex++)
                {
                    bool isLast = partIndex == path.Length - 1;
                    var node = nodes.SingleOrDefault(i => i.Node.Name == path[partIndex]);
                    if (node == null)
                        break;

                    if (!isLast)
                        node.IsExpanded = true;
                    else
                        node.IsSelected = true;

                    nodes = node.Children;
                }
            }
            finally
            {
                _selectingNode = false;
            }
        }

        /// <summary>
        /// Updates the history (records the current position).
        /// </summary>
        private void UpdateHistory()
        {
            if (_updatingHistory)
                return;
            while (_history.Count > _historyIndex + 1)
                _history.RemoveAt(_historyIndex + 1);
            _history.Insert(++_historyIndex, SelectedFolder);
        }

        /// <summary>
        /// Updates the can navigate flags.
        /// </summary>
        private void UpdateCanNavigate()
        {
            CanNavigateBack = _historyIndex > 0;
            CanNavigateForward = _historyIndex < _history.Count - 1;
            CanNavigateUp = SelectedFolder != null && SelectedFolder.Parent != null;
        }

        /// <summary>
        /// Called when Key Up in Location TextBox.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void OnLocationKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                var node = NodeProvider.Find(Location.Text);
                if (node != null)
                {
                    if (!node.IsFolder)
                        node = node.Parent;
                    SelectedFolder = node;
                }
                Location.SelectAll();
            }
        }

        /// <summary>
        /// Called when selection in FilesList has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnFilesListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedNode = (INode)FilesList.SelectedItem;
            if (selectedNode == null)
                return;
            Selection.Text = selectedNode.Name;
        }

        /// <summary>
        /// Called when double-click occurred in FilesList.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnFilesListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedNode = (INode)FilesList.SelectedItem;
            if (selectedNode == null)
                return;
            if (selectedNode.IsFolder)
                SelectedFolder = selectedNode;
            else
            {
                if (Select != null && Select.CanExecute(selectedNode))
                    Select.Execute(selectedNode);
            }
        }

        /// <summary>
        /// Called when SelectedFolderPath changed.
        /// </summary>
        private void OnSelectedFolderPathChanged()
        {
            if (!IsLoaded)
                return;

            // when set to null, we don't go further
            if (SelectedFolderPath == null)
                return;

            // if the selected folder already matches this path, there is nothing to do
            if (SelectedFolder != null && SelectedFolder.Path == SelectedFolderPath)
                return;

            SelectedFolder = NodeProvider.Find(SelectedFolderPath);
        }

        private void OnModeChanged()
        {
            var fullViewVisibility = ShowFilesList ? Visibility.Visible : Visibility.Collapsed;
            var foldersViewVisibility = ShowFilesList ? Visibility.Collapsed : Visibility.Visible;
            FullView.Visibility = fullViewVisibility;
            FolderView.Visibility = foldersViewVisibility;
            SelectionView.Visibility = fullViewVisibility;
        }

        private readonly ISet<string> _isExpanded = new HashSet<string>();

        private void OnNodeProviderChanged()
        {
            RootNodes = NodeProvider.Root.Select(n => new TreeViewNode(n, _isExpanded)).ToArray();
        }
    }
}
