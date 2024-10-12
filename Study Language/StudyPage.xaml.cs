using Microsoft.Maui.Media;

namespace Study_Language;

public partial class StudyPage : ContentPage
{
    private DatabaseService _databaseService;
    private List<WordEntry> _words;
    private List<PhraseEntry> _phrases;
    private bool _isWordsMode = true;
    private int _currentIndex = 0;
    private SpeechOptions _speechOptions;
    private string toRead;

    public StudyPage()
	{
		InitializeComponent();
        _databaseService = App.DatabaseService;
        _words = App.Words;
        _phrases = App.Phrases;
        _speechOptions = App.SpeechOptions;
        dataPicker.SelectedIndex = 0;
        LoadData();
    }

    private async void LoadData()
    {
        _words = await _databaseService.GetWordsAsync();
        _phrases = await _databaseService.GetPhrasesAsync();
        DisplayNextTranslation();
    }

    private void OnDataPickerChanged(object sender, EventArgs e)
    {
        if (dataPicker.SelectedItem != null)
        {
            // Определяем режим работы
            _isWordsMode = dataPicker.SelectedItem.ToString() == "Слова";
            _currentIndex = 0;
            DisplayNextTranslation();
        }
    }

    private void DisplayNextTranslation()
    {
        // Показать перевод первого элемента из списка в зависимости от режима
        if (_isWordsMode && _words.Count > 0)
        {
            var word = _words[_currentIndex];
            translationLabel.Text = word.Translation;
            toRead = word.Word;
        }
        else if (!_isWordsMode && _phrases.Count > 0)
        {
            var phrase = _phrases[_currentIndex];
            translationLabel.Text = phrase.Translation;
            toRead = phrase.Phrase;
        }

        // Очистить поле ввода и результат
        answerEntry.Text = string.Empty;
        Task.Delay(100);
        answerEntry.Focus();
        resultLabel.Text = string.Empty;
    }

    private async void OnCheckAnswerClicked(object sender, EventArgs e)
    {
        string userAnswer = answerEntry.Text?.Trim();

        if (_isWordsMode && _words.Count > 0)
        {
            var word = _words[_currentIndex];
            if (string.Equals(userAnswer, word.Word, StringComparison.OrdinalIgnoreCase))
            {
                resultLabel.Text = "Правильно!";
            }
            else
            {
                resultLabel.Text = $"Неправильно. Правильный ответ: {word.Word}";
            }
        }
        else if (!_isWordsMode && _phrases.Count > 0)
        {
            var phrase = _phrases[_currentIndex];
            if (string.Equals(userAnswer, phrase.Phrase, StringComparison.OrdinalIgnoreCase))
            {
                resultLabel.Text = "Правильно!";
            }
            else
            {
                resultLabel.Text = $"Неправильно. Правильный ответ: {phrase.Phrase}";
            }
        }
    }

    private void OnNextClicked(object sender, EventArgs e)
    {
        // Переход к следующему слову/фразе
        _currentIndex++;
        if (_isWordsMode && _currentIndex >= _words.Count)
        {
            _currentIndex = 0; // Возврат к первому слову, если дошли до конца списка
        }
        else if (!_isWordsMode && _currentIndex >= _phrases.Count)
        {
            _currentIndex = 0; // Возврат к первой фразе, если дошли до конца списка
        }

        DisplayNextTranslation();
    }

    // Озвучивание слова-предложения
    private async void OnSpeakButtonClicked(object sender, EventArgs e)
    { 
        await TextToSpeech.SpeakAsync(toRead, _speechOptions);
    }
}