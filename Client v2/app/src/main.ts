import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import Store from '@/stores/store';

createApp(App).use(router).provide('Store', Store).mount('#app');
