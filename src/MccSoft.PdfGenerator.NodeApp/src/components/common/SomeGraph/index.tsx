import React from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import { useTranslation } from 'react-i18next';


export const SomeGraph = () => {

  const i18n = useTranslation();

  const options = {
  chart: {
      type: 'spline',
      scrollablePlotArea: {
          minWidth: 600,
          scrollPositionX: 1,
      },
  },
  title: {
      text: i18n.t('Graph_Title'),
      align: 'left',
  },
  subtitle: {
      text: i18n.t('Graph_Subtitle'),
      align: 'left',
  },
  xAxis: {
      type: 'datetime',
      labels: {
          overflow: 'justify',
      },
  },
  yAxis: {
      title: {
          text: i18n.t('YAxis_Text'),
      },
      minorGridLineWidth: 0,
      gridLineWidth: 0,
      alternateGridColor: null,
      plotBands: [{ // Light air
          from: 0.3,
          to: 1.5,
          color: 'rgba(68, 170, 213, 0.1)',
          label: {
              text: i18n.t('Light_Air'),
              style: {
                  color: '#606060',
              },
          },
      }, { // Light breeze
          from: 1.5,
          to: 3.3,
          color: 'rgba(0, 0, 0, 0)',
          label: {
              text: i18n.t('Light_Breeze'),
              style: {
                  color: '#606060',
              },
          },
      }, { // Gentle breeze
          from: 3.3,
          to: 5.5,
          color: 'rgba(68, 170, 213, 0.1)',
          label: {
              text: i18n.t('Gentle_Breeze'),
              style: {
                  color: '#606060',
              },
          },
      }, { // Moderate breeze
          from: 5.5,
          to: 8,
          color: 'rgba(0, 0, 0, 0)',
          label: {
              text: i18n.t('Moderate_Breeze'),
              style: {
                  color: '#606060',
              },
          },
      }, { // Fresh breeze
          from: 8,
          to: 11,
          color: 'rgba(68, 170, 213, 0.1)',
          label: {
              text: i18n.t('Fresh_Breeze'),
              style: {
                  color: '#606060',
              },
          },
      }, { // Strong breeze
          from: 11,
          to: 14,
          color: 'rgba(0, 0, 0, 0)',
          label: {
              text: i18n.t('Strong_Breeze'),
              style: {
                  color: '#606060',
              },
          },
      }, { // High wind
          from: 14,
          to: 15,
          color: 'rgba(68, 170, 213, 0.1)',
          label: {
              text: i18n.t('High_Wind'),
              style: {
                  color: '#606060',
              },
          },
      }],
  },
  tooltip: {
      valueSuffix: ' m/s',
  },
  plotOptions: {
      spline: {
          lineWidth: 4,
          states: {
              hover: {
                  lineWidth: 5,
              },
          },
          marker: {
              enabled: false,
          },
          pointInterval: 3600000, // one hour
          pointStart: Date.UTC(2018, 1, 13, 0, 0, 0),
      },
  },
  series: [{
      name: 'Hestavollane',
      data: [
          3.7, 3.3, 3.9, 5.1, 3.5, 3.8, 4.0, 5.0, 6.1, 3.7, 3.3, 6.4,
          6.9, 6.0, 6.8, 4.4, 4.0, 3.8, 5.0, 4.9, 9.2, 9.6, 9.5, 6.3,
          9.5, 10.8, 14.0, 11.5, 10.0, 10.2, 10.3, 9.4, 8.9, 10.6, 10.5, 11.1,
          10.4, 10.7, 11.3, 10.2, 9.6, 10.2, 11.1, 10.8, 13.0, 12.5, 12.5, 11.3,
          10.1,
      ],

  }, {
      name: 'Vik',
      data: [
          0.2, 0.1, 0.1, 0.1, 0.3, 0.2, 0.3, 0.1, 0.7, 0.3, 0.2, 0.2,
          0.3, 0.1, 0.3, 0.4, 0.3, 0.2, 0.3, 0.2, 0.4, 0.0, 0.9, 0.3,
          0.7, 1.1, 1.8, 1.2, 1.4, 1.2, 0.9, 0.8, 0.9, 0.2, 0.4, 1.2,
          0.3, 2.3, 1.0, 0.7, 1.0, 0.8, 2.0, 1.2, 1.4, 3.7, 2.1, 2.0,
          1.5,
      ],
  }],
  navigation: {
      menuItemStyle: {
          fontSize: '10px',
      },
  },
};

  return (
        <HighchartsReact
            highcharts={Highcharts}
            options={options}
        />
    );
};
