using MarkPad.XAML.Converters;

namespace MarkPad.Contracts
{
    public enum SpellingLanguages
    {
        [DisplayString("English (Australia)")]
        Australian,
        [DisplayString("English (Canada)")]
        Canadian,
        [DisplayString("English (United States)")]
        UnitedStates,
        [DisplayString("Spanish (Spain)")]
        Spain
    }

    public interface ISpellingService
    {
        void ClearLanguages();
        void SetLanguage(SpellingLanguages language);

        bool Spell(string word);
    }
}
