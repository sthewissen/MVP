using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using MVP.Resources;
using Xamarin.Essentials;

namespace MVP.Helpers
{
    public class LocalizationResourceManager : INotifyPropertyChanged
    {
        const string languageKey = nameof(languageKey);

        public static LocalizationResourceManager Current { get; } = new LocalizationResourceManager();

        LocalizationResourceManager()
            => SetCulture(new CultureInfo(Preferences.Get(languageKey, CurrentCulture.TwoLetterISOLanguageName)));

        public string this[string text]
            => Translations.ResourceManager.GetString(text, Translations.Culture);

        public void SetCulture(CultureInfo language)
        {
            CultureInfo.DefaultThreadCurrentCulture = language;
            CultureInfo.DefaultThreadCurrentUICulture = language;
            CultureInfo.CurrentCulture = language;
            CultureInfo.CurrentUICulture = language;
            Thread.CurrentThread.CurrentCulture = language;
            Thread.CurrentThread.CurrentUICulture = language;
            Translations.Culture = language;
            Invalidate();
        }

        public string GetValue(string text, string ResourceId)
        {
            var resourceManager = new ResourceManager(ResourceId, typeof(TranslateExtension).GetTypeInfo().Assembly);
            return resourceManager.GetString(text, CultureInfo.CurrentCulture);
        }

        public CultureInfo CurrentCulture
            => Translations.Culture ?? Thread.CurrentThread.CurrentUICulture;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Invalidate()
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}