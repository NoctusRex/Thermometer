import { Measurement } from '@/models/measurement';
import { padStart, round, floor, trimEnd } from 'lodash-es';
import moment from 'moment';

const formatDate = (date: string) => {
  return moment(date).format('YYYY-MM-DD');
};

const formatHour = (date: string) => {
  return moment(date).format('HH');
};

const formatDay = (date: string) => {
  return moment(date).format('DD. dd');
};

const formatMonth = (date: string) => {
  return moment(date).format('MM. MMMM');
};

const formatYear = (date: string) => {
  return moment(date).format('YYYY');
};

const formatDecimal = (decimal: number) => {
  if (!decimal) return '';

  let result = round(decimal, 1).toString();
  if (!result.includes('.')) {
    result += '.0';
  }

  return result;
};

const getTemperature = (measurement: Measurement) => {
  return `${formatDecimal(measurement.temperature?.min)} - ${formatDecimal(measurement.temperature?.average)} - ${formatDecimal(measurement.temperature?.max)} °C`;
};

const getHumidity = (measurement: Measurement) => {
  return `${formatDecimal(measurement.humidity?.min)} - ${formatDecimal(measurement.humidity?.average)} - ${formatDecimal(measurement.humidity?.max)} %`;
};

const formatDateToTime = (date: string) => {
  return moment(date).format('HH:mm');
};

export default { formatYear, formatDateToTime, formatMonth, formatDay, formatHumidity: getHumidity, formatTemperature: getTemperature, formatDecimal, formatHour, formatDate };
