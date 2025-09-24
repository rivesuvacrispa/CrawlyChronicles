using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI.Elements
{
    public class LocaleChangeButton : MonoBehaviour
    {
        private int currentLocale;

        private void Start() => currentLocale = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);

        public void ChangeLocale()
        {
            currentLocale = currentLocale == 0 ? 1 : 0;
            var locale = LocalizationSettings.AvailableLocales.Locales[currentLocale];
            LocalizationSettings.SelectedLocale = locale;
        }
    }
}