<template>
  <header class="header-container">
    <button v-for="(dataType, index) in data" :key="index" class="header-container-item" :style="getActiveStyles(dataType)" @click="dataToDisplay = dataType">
      {{dataType}}
    </button>
  </header>
  <div>
    <DevicesTemplate :component="getComponent()" />
  </div>
</template>

<script setup lang="ts">
import DevicesTemplate from '../templates/DevicesTempalte.vue';
import DayComponent from '@/components/DayComponent.vue';
import HourComponent from '@/components/HourComponent.vue';
import { Ref, ref } from 'vue';

const data = ['Current', 'Day', 'Week', 'Month', 'Year'] as const;
type Data = typeof data[number];

const dataToDisplay: Ref<Data> = ref('Current');

const getActiveStyles = (selection: Data) => {
  if (selection === dataToDisplay.value) {
    return 'font-weight: bold;';
  }

  return '';
};

const getComponent = () => {
  if (dataToDisplay.value === 'Current') return HourComponent;
  if (dataToDisplay.value === 'Day') return DayComponent;
};

</script>

<style scoped="true">
.header-container {
  display: flex;
  flex-flow: row nowrap;
  justify-content: space-evenly;
  align-items: baseline;
  column-gap: 20px;
  padding-left: 10%;
  padding-right: 10%;
}

.header-container-item {
  padding: 5px;
  width: 20%;
  text-align: center;
}
</style>
