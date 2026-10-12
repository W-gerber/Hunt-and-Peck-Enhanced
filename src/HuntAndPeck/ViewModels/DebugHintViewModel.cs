using HuntAndPeck.Models;

namespace HuntAndPeck.ViewModels
{
    public class DebugHintViewModel
    {
        public DebugHintViewModel(DebugHint hint)
        {
            Hint = hint;
            Text = string.Join(", ", hint.SupportedPatterns);

            var vowels = new[] {"a", "e", "i", "o", "u"};
            ShortText = Text.Replace("PatternIdentifiers.Pattern", string.Empty);
            foreach(var vowel in vowels)
            {
                ShortText = ShortText.Replace(vowel, string.Empty);
            }
        }

        public DebugHint Hint { get; set; }
        public string Text { get; set; }
        public string ShortText { get; set; }
    }
}
