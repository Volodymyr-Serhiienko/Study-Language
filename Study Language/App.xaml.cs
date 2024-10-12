namespace Study_Language
{
    public partial class App : Application
    {
        public static DatabaseService DatabaseService { get; private set; }
        public static List<WordEntry> Words {  get; private set; }
        public static List<PhraseEntry> Phrases { get; private set; }
        public static SpeechOptions SpeechOptions { get; private set; }

        public App()
        {
            InitializeComponent();

            DatabaseService = new DatabaseService(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "languagelearning.db"));
            Words = new List<WordEntry>();
            Phrases = new List<PhraseEntry>();
            InitializeTextToSpeech();

            MainPage = new AppShell();
        }

        // Метод инициализации озвучки
        private async void InitializeTextToSpeech()
        {
            var locales = await TextToSpeech.GetLocalesAsync();
            var norwegianLocale = locales.FirstOrDefault(l => l.Language == "nb-NO");

            if (norwegianLocale != null)
            {
                SpeechOptions = new SpeechOptions
                {
                    Locale = norwegianLocale
                };
            }
        }
    }
}
