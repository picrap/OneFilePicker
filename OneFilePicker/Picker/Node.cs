namespace OneFilePicker.Picker
{
    using System.Windows.Media;

    public class Node
    {
        public Node[] Children { get; set; }
        public ImageSource Icon { get; set; }
        public string Name { get; set; }
    }
}
