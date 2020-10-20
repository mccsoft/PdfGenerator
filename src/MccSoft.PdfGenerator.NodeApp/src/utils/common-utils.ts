import { Duration } from 'luxon';
import { ExtendedWindow } from '~typings/CommonTypes';
import { IReportDto } from '~typings/ReportDto';
import { MockUtils } from './mock-utils';

export class CommonUtils {
    static getReport(): IReportDto {
        return (window as ExtendedWindow).getReportDto();
    }

    static getReportForDev(
        language: string,
    ): IReportDto {
        return MockUtils.getMockedReportDto(
            language,
        );
    }

    static renderDateRange(startDate: string, endDate: string): string {
        if (startDate === endDate) {
            return startDate;
        }

        return `${startDate} — ${endDate}`;
    }

    static mapMinutesToHours(value: number | undefined): number {
        if (value === undefined || value < 0) {
            return 0;
        }

        return Duration.fromObject({ minutes: value }).as('hours');
    }

    static isValueDefined(value?: number): boolean {
        return value !== null && value !== undefined;
    }
}
