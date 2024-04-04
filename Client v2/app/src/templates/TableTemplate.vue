<template>
  <div>
    <div class="header">
      <div>{{ props.device }}</div>
      <div>{{ getTimeStamp() }}</div>
    </div>
    <table>
      <tr>
        <th v-if="props.time !== 'current'" class="border-right width-small">{{ getTimeHeader() }}</th>
        <th>Temperature<br>Min</th>
        <th>Temperature<br>Average</th>
        <th class="border-right">Temperature<br>Max</th>
        <th>Humidity<br>Min</th>
        <th>Humidity<br>Average</th>
        <th>Humidity<br>Max</th>
      </tr>
      <tr v-for="(measurement, index) in props.measurements" :key="index">
        <td v-if="props.time !== 'current'" class="border-right width-small">{{ getTime(measurement.timeStamp) }}</td>
        <td>{{ formatters.formatDecimal(measurement.temperature?.min) }}</td>
        <td>{{ formatters.formatDecimal(measurement.temperature?.average) }}</td>
        <td class="border-right">{{ formatters.formatDecimal(measurement.temperature?.max) }}</td>
        <td>{{ formatters.formatDecimal(measurement.humidity?.min) }}</td>
        <td>{{ formatters.formatDecimal(measurement.humidity?.average) }}</td>
        <td>{{ formatters.formatDecimal(measurement.humidity?.max) }}</td>
      </tr>
    </table>
    <br>
</div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import { Measurement } from '../models/measurement';
import formatters from '../hooks/useFormatters';
import { capitalize } from 'lodash-es';

const props = defineProps<{
  device: string;
  date: string;
  time: 'current' | 'hour' | 'day' | 'month';
  measurements: Array<Measurement>;
}>();

const getTimeHeader = () => {
  return capitalize(props.time);
};

const getTime = (date: string) => {
  if (props.time === 'hour') {
    return formatters.formatHour(date);
  }
  if (props.time === 'day') {
    return formatters.formatDay(date);
  }
  if (props.time === 'month') {
    return formatters.formatMonth(date);
  }

  return date;
};

const getTimeStamp = () => {
  if (props.time === 'hour') {
    return formatters.formatDate(props.date);
  }
  if (props.time === 'day') {
    return formatters.formatMonth(props.date);
  }
  if (props.time === 'month') {
    return formatters.formatYear(props.date);
  }

  return props.date;
};

</script>

<style scoped="true">
.header {
  display: flex;
  flex-flow: row nowrap;
  justify-content: space-between;
}
.border-right {
  border-right: 1px solid;
}
.width-small {
  min-width: 50px;
}
th {
  text-align: center;
  min-width: 100px;
  background-color: #FFF8E7;
}
td {
  text-align: center;
  width: 50px;
}
table, th, td {
  border-bottom: 1px dotted;
}
</style>
