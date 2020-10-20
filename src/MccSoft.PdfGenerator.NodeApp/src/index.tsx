import React from 'react';
import ReactDOM from 'react-dom';
import { DevelopmentUtils } from '~utils/development-utils';
import { Report } from '~containers/ReportProvider/ReportContainer';

DevelopmentUtils.initialize();

ReactDOM.render(<Report/>, document.getElementById('root'));
