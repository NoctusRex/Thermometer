<template>
  <div class="container">
    <component class="container-item" v-for="(deviceName, index) in devices" :key="index" :device="deviceName" :is="props.component" />
  </div>
</template>

<script setup lang="ts">
import { Ref, inject, ref, defineProps } from 'vue';
import Store from '@/stores/store';

const store = inject<typeof Store>('Store');
const devices: Ref<Array<string>> = ref([]);

store?.getters.getDevices$().subscribe((result) => {
  devices.value = [...result];
});

const props = defineProps<{
  component: any;
}>();

</script>

<style scoped="true">
.container {
  display: flex;
  flex-flow: column nowrap;
  justify-content: center;
  align-items: center;
}

.container-item {
  padding-top: 20px;
}
</style>
