using System.Globalization;

namespace Study_Language
{
    internal class WordPhraseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // parameter — это 'Word' или 'Phrase'
            if (parameter.ToString() == "Word")
            {
                return value.ToString() == "Слова";  // Показывать слово
            }
            else if (parameter.ToString() == "Phrase")
            {
                return value.ToString() == "Предложения";  // Показывать фразу
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
