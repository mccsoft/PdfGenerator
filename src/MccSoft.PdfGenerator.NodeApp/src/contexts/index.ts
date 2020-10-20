import React from 'react';
import { IReportDto } from '~typings/ReportDto';

const emptyReportDto: IReportDto = {};

export const ReportContext = React.createContext(emptyReportDto);
