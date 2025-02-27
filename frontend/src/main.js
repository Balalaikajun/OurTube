import './assets/styles/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'; // <-- Подключаем роутер

const app = createApp(App);
app.use(router); // <-- Подключаем роутер к приложению
app.mount('#app');