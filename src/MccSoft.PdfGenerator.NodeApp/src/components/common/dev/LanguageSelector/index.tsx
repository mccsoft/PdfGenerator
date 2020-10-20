import React from 'react';
import { DevelopmentUtils } from '~utils/development-utils';
import './styles.scss';

interface Props {
    value: string | undefined;
    onChange: (lng: string) => void;
}

export const LanguageSelector: React.FC<Props> = ({ value, onChange }) => {
    if (!DevelopmentUtils.isDevelopment) {
        return null;
    }

    const languages: string[] = [
        'de',
        'en',
        'fr',
    ];

    return (
        <select
            value={value}
            className={'language-selector'}
            onChange={(e) => onChange(e.target.value)}
        >
            {
                languages.map(lng => (
                    <option key={lng} value={lng}>
                        {lng}
                    </option>
                ))
            }
        </select>
    );
};
