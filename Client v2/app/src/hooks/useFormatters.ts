import { Measurement } from '@/models/measurement';
import { padStart, round, floor, trimEnd } from 'lodash-es';
import moment from 'moment';

const formatDate = (date: string) => {
  return moment(date).format('YYYY-MM-DD');
};

const formatHour = (date: string) => {
  return moment(date).format('HH');
};

const formatDecimal = (decimal: number) => {
  if (!decimal) return '';

  const integerPart: number = floor(decimal);
  const decimalPart: number = round((decimal - integerPart) * 10);
  const formattedString = `${padStart(integerPart.toString(), 2, '0')}.${padStart(trimEnd(decimalPart.toString(), '0'), 1, '0')}`;

  return formattedString;
};

const getTemperature = (measurement: Measurement) => {
  return `${formatDecimal(measurement.temperature?.min)} - ${formatDecimal(measurement.temperature?.average)} - ${formatDecimal(measurement.temperature?.max)} °C`;
};

const getHumidity = (measurement: Measurement) => {
  return `${formatDecimal(measurement.humidity?.min)} - ${formatDecimal(measurement.humidity?.average)} - ${formatDecimal(measurement.humidity?.max)} °C`;
};

const formatDateToTime = (date: string) => {
  return moment(date).format('HH:mm');
};

export default { formatDateToTime, formatHumidity: getHumidity, formatTemperature: getTemperature, formatDecimal, formatHour, formatDate };
