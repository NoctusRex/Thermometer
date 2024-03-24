<template>
  <div>
    <HourComponent v-for="(deviceName, index) in devices" :key="index" :device="deviceName"></HourComponent>
  </div>
</template>

<script setup lang="ts">
import { Ref, inject, ref } from 'vue';
import HourComponent from '@/components/HourComponent.vue';
import Store from '@/stores/store';

const store = inject<typeof Store>('Store');
const devices: Ref<Array<string>> = ref([]);

store?.getters.getDevices$().subscribe((result) => {
  devices.value = [...result];
});

</script>
