namespace Study_Language
{
    public partial class MainPage : ContentPage
    {
        private DatabaseService _databaseService;
        private List<WordEntry> _words;
        private List<PhraseEntry> _phrases;
        private List<object> _currentItems;
        private WordEntry _selectedWord;
        private PhraseEntry _selectedPhrase;
        private SpeechOptions _speechOptions;
        private bool _isWordsMode = true;
        private int _currentIndex = 0;

        public MainPage()
        {
            InitializeComponent();
            _databaseService = App.DatabaseService;
            _words = App.Words;
            _phrases = App.Phrases;
            _speechOptions = App.SpeechOptions;
            dataPicker.SelectedIndex = 0;
            LoadData();
            CheckLocale();
        }

        // Проверка локали ситрезатора речи
        private async void CheckLocale() 
        {
            if (App.SpeechOptions == null) await DisplayAlert("Ошибка", "Норвежская локаль не найдена!", "ОК");
        }

        // Выбор режима "Слова/Предложения"
        private void OnDataPickerChanged(object sender, EventArgs e)
        {
            if (dataPicker.SelectedItem != null)
            {
                // Определяем режим работы
                _isWordsMode = dataPicker.SelectedItem.ToString() == "Слова";
                LoadData(); // Перезагружаем данные для выбранного режима
            }
        }

        // Загрузка данных в зависимости от режима
        private async void LoadData()
        {
            if (_isWordsMode)
            {
                _words = await _databaseService.GetWordsAsync();  // Загрузить слова
                wordsCollectionView.ItemsSource = _words; // Привязать данные к CollectionView
            }
            else
            {
                _phrases = await _databaseService.GetPhrasesAsync();  // Загрузить предложения
                wordsCollectionView.ItemsSource = _phrases;  // Привязать предложения к CollectionView
            }
        }

        // Добавление новой записи (слова или предложения)
        private async void OnAddWordClicked(object sender, EventArgs e)
        {
            if (_isWordsMode)
            {
                await ShowAddWordDialog();
            }
            else
            {
                await ShowAddPhraseDialog(); // Для предложений
            }
        }

        // Диалоговое окно для добавления слова
        private async Task ShowAddWordDialog()
        {
            string word = string.Empty;
            string translation = string.Empty;

            var wordEntry = new Entry { Placeholder = "Введите слово" };
            var translationEntry = new Entry { Placeholder = "Введите перевод" };

            var addButton = new Button { Text = "Добавить", BackgroundColor = Colors.LightBlue };
            var cancelButton = new Button { Text = "Отмена", BackgroundColor = Colors.LightGray };

            var layout = new StackLayout
            {
                Padding = 20,
                Children =
        {
            wordEntry,
            translationEntry,
            new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = { addButton, cancelButton }
            }
        }
            };

            var dialog = new ContentPage { Content = layout };

            // Открываем диалог
            await Navigation.PushModalAsync(dialog);

            // Задержка перед установкой фокуса
            await Task.Delay(100); // Задержка на 100 миллисекунд

            // Устанавливаем фокус на wordEntry
            Device.BeginInvokeOnMainThread(() =>
            {
                wordEntry.Focus();
            });

            addButton.Clicked += async (s, e) =>
            {
                if (!string.IsNullOrEmpty(wordEntry.Text) && !string.IsNullOrEmpty(translationEntry.Text))
                {
                    word = wordEntry.Text;
                    translation = translationEntry.Text;

                    // Проверяем, существует ли слово в базе данных
                    var existingWord = await _databaseService.GetWordByTextAsync(wordEntry.Text);
                    if (existingWord != null)
                    {
                        // Если слово уже есть в базе данных, показываем сообщение
                        await DisplayAlert("Ошибка", $"Слово '{word}' уже существует в базе данных.", "ОК");
                    }
                    else
                    {
                        // Если слова нет, добавляем его
                        var newWord = new WordEntry
                        {
                            Id = Guid.NewGuid().ToString(),
                            Word = word,
                            Translation = translation
                        };

                        await _databaseService.SaveWordAsync(newWord);
                        LoadData();
                        await Navigation.PopModalAsync();
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Заполните оба поля", "ОК");
                }
            };

            cancelButton.Clicked += async (s, e) =>
            {
                await Navigation.PopModalAsync();
            };
        }

        // Диалоговое окно для добавления предложения (аналогично добавлению слова)
        private async Task ShowAddPhraseDialog()
        {
            string phrase = string.Empty;
            string translation = string.Empty;

            var phraseEntry = new Entry { Placeholder = "Введите предложение" };
            var translationEntry = new Entry { Placeholder = "Введите перевод" };

            var addButton = new Button { Text = "Добавить", BackgroundColor = Colors.LightBlue };
            var cancelButton = new Button { Text = "Отмена", BackgroundColor = Colors.LightGray };

            var layout = new StackLayout
            {
                Padding = 20,
                Children =
                {
                    phraseEntry,
                    translationEntry,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Children = { addButton, cancelButton }
                    }
                }
            };

            var dialog = new ContentPage { Content = layout };

            // Открываем диалог
            await Navigation.PushModalAsync(dialog);

            // Задержка перед установкой фокуса
            await Task.Delay(100); // Задержка на 100 миллисекунд

            // Устанавливаем фокус на phraseEntry
            Device.BeginInvokeOnMainThread(() =>
            {
                phraseEntry.Focus();
            });

            addButton.Clicked += async (s, e) =>
            {
                if (!string.IsNullOrEmpty(phraseEntry.Text) && !string.IsNullOrEmpty(translationEntry.Text))
                {
                    phrase = phraseEntry.Text;
                    translation = translationEntry.Text;

                    // Проверяем, существует ли предложение в базе данных
                    var existingPhrase = await _databaseService.GetPhraseByTextAsync(phraseEntry.Text);
                    if (existingPhrase != null)
                    {
                        // Если предложение уже есть в базе данных, показываем сообщение
                        await DisplayAlert("Ошибка", $"Предложение '{phrase}' уже существует в базе данных.", "ОК");
                    }
                    else
                    {
                        // Если предложения нет, добавляем его
                        var newPhrase = new PhraseEntry
                        {
                            Id = Guid.NewGuid().ToString(),
                            Phrase = phrase,
                            Translation = translation
                        };

                        await _databaseService.SavePhraseAsync(newPhrase);
                        LoadData();
                        await Navigation.PopModalAsync();
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Заполните оба поля", "ОК");
                }
            };

            cancelButton.Clicked += async (s, e) =>
            {
                await Navigation.PopModalAsync();
            };
        }

        // Получение выбранной записи (слово или фраза)
        private void OnWordTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
            {
                if (_isWordsMode)
                {
                    _selectedWord = (WordEntry)e.CurrentSelection[0];
                    _selectedPhrase = null; // Очистить выбранную фразу
                }
                else
                {
                    _selectedPhrase = (PhraseEntry)e.CurrentSelection[0];
                    _selectedWord = null; // Очистить выбранное слово
                }
            }
        }

        // Удаление записи (слова или фразы)
        private async void OnDeleteWordClicked(object sender, EventArgs e)
        {
            if (_isWordsMode && _selectedWord != null)
            {
                bool answer = await DisplayAlert("Подтверждение удаления",
                    $"Вы уверены, что хотите удалить слово '{_selectedWord.Word}'?",
                    "Удалить", "Отмена");

                if (answer)
                {
                    await _databaseService.DeleteWordAsync(_selectedWord.Id);
                    LoadData();
                    _selectedWord = null;
                }
            }
            else if (!_isWordsMode && _selectedPhrase != null)
            {
                bool answer = await DisplayAlert("Подтверждение удаления",
                    $"Вы уверены, что хотите удалить фразу '{_selectedPhrase.Phrase}'?",
                    "Удалить", "Отмена");

                if (answer)
                {
                    await _databaseService.DeletePhraseAsync(_selectedPhrase.Id);
                    LoadData();
                    _selectedPhrase = null;
                }
            }
            else
            {
                await DisplayAlert("Ошибка", "Выберите элемент для удаления.", "ОК");
            }
        }

        // Озвучивание слова-предложения
        private async void OnSpeakButtonClicked(object sender, EventArgs e)
        {
            // Определяем, какой элемент был нажат (если необходимо)
            var button = (Image)sender;
            var tappedWordOrPhrase = button.BindingContext;

            // Здесь вы можете определить, какое слово или предложение нужно озвучить.
            if (tappedWordOrPhrase is WordEntry wordEntry)
            {
                await TextToSpeech.SpeakAsync(wordEntry.Word, _speechOptions);
            }
            else if (tappedWordOrPhrase is PhraseEntry phraseEntry)
            {
                await TextToSpeech.SpeakAsync(phraseEntry.Phrase, _speechOptions);
            }
        }
    }
}
