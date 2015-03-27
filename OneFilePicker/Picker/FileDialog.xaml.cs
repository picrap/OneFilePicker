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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using File;

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
            "FileName", typeof(string), typeof(FileDialog), new PropertyMetadata(default(string)));

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
            "Filters", typeof(Filter[]), typeof(FileDialog), new PropertyMetadata(default(Filter[])));

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
            "Select", typeof(ICommand), typeof(FileDialog), new PropertyMetadata(default(ICommand)));

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
            "Cancel", typeof(ICommand), typeof(FileDialog), new PropertyMetadata(default(ICommand)));

        public ICommand Cancel
        {
            get { return (ICommand)GetValue(CancelProperty); }
            set { SetValue(CancelProperty, value); }
        }

        public static readonly DependencyProperty RootNodesProperty = DependencyProperty.Register(
            "RootNodes", typeof(INode[]), typeof(FileDialog), new PropertyMetadata(new INode[] { new RootNode() }));

        public INode[] RootNodes
        {
            get { return (INode[])GetValue(RootNodesProperty); }
            set { SetValue(RootNodesProperty, value); }
        }

        public static readonly DependencyProperty SelectedFolderProperty = DependencyProperty.Register(
            "SelectedFolder", typeof (INode), typeof (FileDialog), new PropertyMetadata(default(INode)));

        public INode SelectedFolder
        {
            get { return (INode) GetValue(SelectedFolderProperty); }
            set { SetValue(SelectedFolderProperty, value); }
        }

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

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedFolder = (INode) ((TreeView) sender).SelectedItem;
        }
    }
}
