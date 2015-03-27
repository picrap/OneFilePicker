namespace OneFilePicker.Picker
{
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{Display}")]
    public class Filter
    {
        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string Display { get; private set; }

        /// <summary>
        /// Gets the mask.
        /// </summary>
        /// <value>
        /// The mask.
        /// </value>
        public string[] Mask { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="mask">The mask.</param>
        public Filter(string displayName, string[] mask)
        {
            Display = displayName;
            Mask = mask;
        }

        /// <summary>
        /// Creates a filter from a literal string, such as "All files (*.*)" or "Office files (*.docx, *.xlsx, *.pptx)".
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <returns></returns>
        public static Filter FromLiteral(string literal)
        {
            var maskIndex = literal.IndexOf('(');
            var maskPart = maskIndex < 0 ? literal : literal.Substring(maskIndex + 1).TrimEnd(')');
            var maskParts = maskPart.Split(',').Select(s => s.Trim());
            return new Filter(literal, maskParts.ToArray());
        }

        /// <summary>
        /// Create a Filter from a literal string and mask, such as "All files", "*.*" or "Office files", "*.docx, *.xlsx, *.pptx).
        /// </summary>
        /// <param name="literal">The literal.</param>
        /// <param name="maskPart">The mask part.</param>
        /// <returns></returns>
        public static Filter FromLiteral(string literal, string maskPart)
        {
            var maskParts = maskPart.Split(',').Select(s => s.Trim());
            return new Filter(literal, maskParts.ToArray());
        }
    }
}
