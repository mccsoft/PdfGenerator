import { IReportDto } from '~typings/ReportDto';
import { getPropertiesMock } from './mocks/common/PropertiesMock';
import { DateRangeMock } from './mocks/common/DateRangeMock';

export class MockUtils {
    static getMockedReportDto(
        language: string,
    ): IReportDto {
        return {
            properties: getPropertiesMock(),
            reportDateRange: DateRangeMock,
        };
    }
}
