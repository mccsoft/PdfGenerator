import { IReportDto } from './ReportDto';

interface ReportDtoProps {
    getReportDto: () => IReportDto;
}

export type ExtendedWindow = (typeof window) & ReportDtoProps;
