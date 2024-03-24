<template>
  <div>
    {{props.device}}
  </div>
  <ul>
    <li>{{ formatDateToTime(measurement?.timeStamp) }} - Next Refresh in {{ minutesToNextRefresh }} minutes</li>
    <li>{{ measurement?.temperature?.average }} °C</li>
    <li>{{ measurement?.humidity?.average }} %</li>
    <li><button @click="refresh()">Refresh</button></li>
</ul>
</template>

<script setup lang="ts">
import { Ref, defineProps, inject, onMounted, onUnmounted, ref } from 'vue';
import { Measurement } from '@/models/measurement';
import { first } from 'lodash-es';
import moment from 'moment';
import { concatMap, interval, map, of, takeWhile, tap } from 'rxjs';
import Store from '@/stores/store';

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

const refresh = () => {
  refresh$(true).subscribe();
};

const refresh$ = (force = false) => {
  return store?.getters.getHour$(props.device, force).pipe(
    map(data => {
      measurement.value = first(data) || {} as Measurement;

      const lastTimeStamp = moment(measurement.value.timeStamp);
      const diff = refreshIntervall - moment().diff(lastTimeStamp, 'minutes');
      minutesToNextRefresh.value = diff;

      if (minutesToNextRefresh.value <= 0) {
        minutesToNextRefresh.value = refreshIntervall;
      }

      return null;
    }));
};

const formatDateToTime = (date: string) => {
  return moment(date).format('HH:mm');
};

</script>

<style scoped="true">
</style>
