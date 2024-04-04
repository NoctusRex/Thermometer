<template>
  <div class="buttons">
    <button @click="refresh()">Refresh</button>
  </div>
  <div>
    <div></div>
  </div>
  <TableTemplate :measurements="[measurement]" :date="getTime()" :device="device" :time="'current'"></TableTemplate>
</template>

<script setup lang="ts">
import { Ref, defineProps, inject, onMounted, onUnmounted, ref } from 'vue';
import { Measurement } from '@/models/measurement';
import { first } from 'lodash-es';
import moment from 'moment';
import { concatMap, interval, map, of, takeWhile, tap } from 'rxjs';
import Store from '@/stores/store';
import formatters from '@/hooks/useFormatters';
import TableTemplate from '@/templates/TableTemplate';

const store = inject<typeof Store>('Store');
const measurement: Ref<Measurement> = ref({} as Measurement);
const minutesToNextRefresh = ref(0);
const refreshIntervall = 20;
let mounted = true;

const props = defineProps<{
  device: string;
}>();

onUnmounted(() => { mounted = false; });

onMounted(() => {
  refresh$().pipe(
    concatMap(() => interval(1000 * 60)),
    concatMap(() => {
      if (!measurement.value) return of(null);

      minutesToNextRefresh.value -= 1;

      if (minutesToNextRefresh.value > 0) return of(null);

      return store?.getters.getHour$(props.device, true)
        .pipe(
          tap(data => {
            measurement.value = first(data) || {} as Measurement;
            minutesToNextRefresh.value = refreshIntervall;
          })
        );
    }),
    takeWhile(() => mounted)
  ).subscribe();
});

const refresh$ = (force = false) => {
  return store
    ? store.getters.getHour$(props.device, force).pipe(
      map(data => {
        measurement.value = first(data) || {} as Measurement;

        const lastTimeStamp = moment(measurement.value.timeStamp);
        const diff = refreshIntervall - moment().diff(lastTimeStamp, 'minutes');
        minutesToNextRefresh.value = diff;

        if (minutesToNextRefresh.value <= 0) {
          minutesToNextRefresh.value = refreshIntervall;
        }

        return null;
      }))
    : of(null);
};

const refresh = () => {
  refresh$(true).subscribe();
};

const getTime = () => {
  return `Next Refresh in ${minutesToNextRefresh.value} minutes - ${formatters.formatDateToTime(measurement.value?.timeStamp)}`;
};

</script>

<style scoped="true">

</style>
