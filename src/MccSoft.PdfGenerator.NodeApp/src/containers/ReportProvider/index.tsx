import React, { useContext, useEffect, useState } from 'react';
import { LanguageSelector } from '~common/dev/LanguageSelector';
import { ReportContext } from '~contexts';
import { IReportDto } from '~typings/ReportDto';
import { CommonUtils } from '~utils/common-utils';
import { DevelopmentUtils } from '~utils/development-utils';
import { LocalizationUtils } from '~utils/localization-utils';
import './bootstrap-grid.min.css';
import './commonStyles.scss';
import './fonts.scss';

const LANGUAGE_KEY = 'language';


const getInitialLanguage = (language: string | undefined): string => {
    if (DevelopmentUtils.isDevelopment) {
      const storedLanguage = localStorage.getItem(LANGUAGE_KEY);
  
      return storedLanguage || 'de';
    }
    return language || 'en';
  };

export const ReportProvider: React.FC = ({ children }) => {
    const [localizationInitialized, setLocalizationInitialized] = useState(false);

    useEffect(() => {
        const init = async () => {
            await LocalizationUtils.initialize();
            setLocalizationInitialized(true);
        };

        init();
    }, []);

    let dto: IReportDto = CommonUtils.getReport();
    const [language, setLanguage] = useState(getInitialLanguage(dto.properties?.language));

    const handleOnLanguageChange = (language: string) => {
        setLanguage(language);

        if (DevelopmentUtils.isDevelopment) {
            localStorage.setItem(LANGUAGE_KEY, language);
        }
    };

    useEffect(() => {
        LocalizationUtils.changeLanguage(language);
      }, [language]);

    if (DevelopmentUtils.isDevelopment && language) {
        dto = CommonUtils.getReportForDev(language);
      }

    if (!localizationInitialized) {
        return null;
    }

    return (
        <ReportContext.Provider value={dto}>
            <ReportProviderInternal
                selectedLanguage={language}
                handleOnLanguageChange={handleOnLanguageChange}
            >
                {children}
            </ReportProviderInternal>
        </ReportContext.Provider>
    );
};

interface ReportProviderInternalState {
    selectedLanguage: string | undefined;
    handleOnLanguageChange: (lng: string) => void;
}

// Extracted because report context values are set above.
// To work with them, they should be already set.
const ReportProviderInternal: React.FC<ReportProviderInternalState> = (props) => {
    const { properties } = useContext(ReportContext);
    const language = properties?.language;

    if (!language) {
        throw new Error('Language must be provided.');
    }

    useEffect(() => {
        LocalizationUtils.changeLanguage(language);
    }, [language]);

    const {
        children,
        handleOnLanguageChange,
        selectedLanguage,
    } = props;

    return (
        <>
            {
                language && (
                    <LanguageSelector value={selectedLanguage} onChange={handleOnLanguageChange}/>
                )
            }
            <div id={'report'}>
                {children}
            </div>
        </>
    );
};
