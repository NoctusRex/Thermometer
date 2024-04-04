<template>
  <div class="header">
    <h1>Thermometer</h1>
    <div class="header-container">
      <button v-for="(dataType, index) in data" :key="index" class="header-container-item" :style="getActiveStyles(dataType)" @click="dataToDisplay = dataType">
        {{dataType}}
      </button>
    </div>
  </div>
  <br>
  <DevicesTemplate :component="getComponent()" />
</template>

<script setup lang="ts">
import DevicesTemplate from '../templates/DevicesTemplate.vue';
import DayComponent from '@/components/DayComponent.vue';
import HourComponent from '@/components/HourComponent.vue';
import MonthComponent from '@/components/MonthComponent.vue';
import YearComponent from '@/components/YearComponent.vue';
import { Ref, ref } from 'vue';

const data = ['Current', 'Day', 'Month', 'Year'] as const;
type Data = typeof data[number];

const dataToDisplay: Ref<Data> = ref('Current');

const getActiveStyles = (selection: Data) => {
  if (selection === dataToDisplay.value) {
    return 'font-weight: bold; border-style: dotted; border-width: thick;';
  }

  return '';
};

const getComponent = () => {
  if (dataToDisplay.value === 'Current') return HourComponent;
  if (dataToDisplay.value === 'Day') return DayComponent;
  if (dataToDisplay.value === 'Month') return MonthComponent;
  if (dataToDisplay.value === 'Year') return YearComponent;

  return null;
};

</script>

<style scoped="true">
.header {
  background-color: #FFF8E7;
  height: 120px;
}

h1 {
  text-align: center;
}

.header-container {
  display: flex;
  justify-content: space-evenly;
  align-items: baseline;
  padding-left: 10%;
  padding-right: 10%;
  flex-wrap: wrap;
}

.header-container-item {
  padding: 5px;
  width: 20%;
  text-align: center;
}
</style>
