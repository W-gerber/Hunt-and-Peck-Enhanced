namespace HuntAndPeck.Models
{
    /// <summary>
    /// A named colour choice shown in the options window colour pickers.
    /// </summary>
    /// <remarks>
    /// The options window used to declare each colour as a ComboBoxItem whose
    /// content was a StackPanel holding a swatch and a label. That renders in
    /// the drop-down but leaves the closed ComboBox blank, because a WPF visual
    /// can only ever have one parent -- once the StackPanel is realised inside
    /// the drop-down it cannot simultaneously be drawn in the selection box.
    ///
    /// Binding to plain data and letting an ItemTemplate build the visuals
    /// fixes that: the template is instantiated separately for the drop-down
    /// row and for the selection box, so both render.
    /// </remarks>
    public class ColorOption
    {
        /// <summary>
        /// Human readable name, e.g. "Gold".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value persisted to settings and parsed into a brush, e.g.
        /// "#F9CD33". Must stay byte-for-byte compatible with the strings the
        /// old ComboBoxItem Tags used, so existing saved settings still match.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Optional trailing note, e.g. "default".
        /// </summary>
        public string Note { get; set; }
    }
}
