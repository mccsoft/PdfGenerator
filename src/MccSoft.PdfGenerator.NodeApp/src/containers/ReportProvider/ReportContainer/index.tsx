import React, { useContext } from 'react';
import { ReportContext } from '~contexts';
import { InfinitePage } from '~common/InfinitePage/InfinitePage';
import { PageOrientation } from '~utils/PageOrientation';
import { useTranslation, withTranslation } from 'react-i18next';
import { ReportProvider } from '..';
import { SomeGraph } from '~common/SomeGraph';

import './styles.scss';

export const ReportContainer = () => (
    <ReportProvider>
        <InfinitePage pageOrientation={PageOrientation.Portrait}>
            <ReportDateRangeView/>
        </InfinitePage>
    </ReportProvider>
);

const ReportDateRangeView = () => {
    const { reportDateRange } = useContext(ReportContext);
    const i18n = useTranslation();
    return (
        <div>
            <h1>
            {i18n.t('Date_Range')}: {reportDateRange?.startDate} - {reportDateRange?.endDate}
            </h1>
            <SomeGraph/>
        </div>
    );
};

export const Report = withTranslation()(ReportContainer);


