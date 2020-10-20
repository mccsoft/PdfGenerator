const { Settings } = require('luxon');
const i18next = require('i18next');
const enzyme = require('enzyme');
const Adapter = require('enzyme-adapter-react-16');

enzyme.configure({ adapter: new Adapter() });

i18next.init({
    interpolation: {
        escapeValue: false,
    },
});

// Monkey patch localization code to use the comma (,) as the decimal separator.
const IntlPolyfill = require('intl');
global.Intl.NumberFormat = IntlPolyfill.NumberFormat;
Number.prototype.toLocaleString = IntlPolyfill.__localeSensitiveProtos.Number.toLocaleString;

i18next.language = 'de';
Settings.defaultLocale = 'de';
