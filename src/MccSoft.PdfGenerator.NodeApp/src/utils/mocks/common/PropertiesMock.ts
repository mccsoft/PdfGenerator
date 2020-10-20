import { IReportProperties } from '~typings/ReportDto';

export const getPropertiesMock = (
    language: string = 'de',
): IReportProperties => ({
    language: language,
    createdAt: '2018-06-01T00:00:00',
});
