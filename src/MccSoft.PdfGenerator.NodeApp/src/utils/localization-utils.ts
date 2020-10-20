import i18next from 'i18next';
import { Settings } from 'luxon';
import { initReactI18next } from 'react-i18next';

export class LocalizationUtils {
  static initialize() {
    const languages = ['en', 'de', 'fr'];
    const defaultLanguage = 'en';

    const initOptions = {
      interpolation: {
        debug: true,
        escapeValue: false,
      },
      load: 'languageOnly' as 'languageOnly',
      lowerCaseLng: true,
      fallbackLng: defaultLanguage,
      whitelist: languages,
      react: {
        useSuspense: false,
      },
    };

    i18next
      .use(initReactI18next)
      .init(initOptions);
    i18next.addResources('de', 'translation', require(`../../public/dictionaries/localization.de.json`));
    i18next.addResources('en', 'translation', require(`../../public/dictionaries/localization.en.json`));
    i18next.addResources('fr', 'translation', require(`../../public/dictionaries/localization.fr.json`));

  }

  static changeLanguage(language: string) {
    try {
      if (i18next.options.whitelist && i18next.options.whitelist.includes(language)) {
        Settings.defaultLocale = language;
      }
      i18next.changeLanguage(language);
    } catch (e) {
      throw e;
    }
  }
}
