<template>
    <div class="buttons">
      <button @click="changeDay(-1)">-</button>
      <button @click="resetDay()">Today</button>
      <button @click="changeDay(1)">+</button>
    </div>
    <TableTemplate :measurements="measurements" :date="date" :device="device" :time="'hour'"></TableTemplate>
</template>

<script setup lang="ts">
import { Ref, inject, ref, defineProps } from 'vue';
import Store from '@/stores/store';
import { Measurement } from '@/models/measurement';
import moment from 'moment';
import formatters from '@/hooks/useFormatters';
import TableTemplate from '@/templates/TableTemplate';

const store = inject<typeof Store>('Store');
const measurements: Ref<Array<Measurement>> = ref([] as Array<Measurement>);
const date = ref(moment().toISOString(true));

const props = defineProps<{
  device: string;
}>();

const getData = (refresh = false) => {
  store?.getters
    .getDay$(props.device, formatters.formatDate(date.value), refresh)
    .subscribe((data) => { measurements.value = data?.reverse(); });
};

const changeDay = (count: number) => {
  const newDate = moment(date.value).add(count, 'days');
  if (newDate.isSameOrAfter(moment())) return;

  date.value = newDate.toISOString(true);
  getData();
};

const resetDay = () => {
  date.value = moment().toISOString(true);
  getData(true);
};

getData();

</script>

<style scoped="true">
</style>
