import './assets/styles/main.css'

import { createApp } from 'vue'
import { authDirective } from '@/assets/utils/authDirective.js'
import App from './App.vue'
import router from './router/index.js' // <-- Подключаем роутер
import { createPinia } from 'pinia'

const app = createApp(App);
const pinia = createPinia(); // Создаем экземпляр Pinia

app.directive('auth', authDirective);
app.use(pinia);
app.use(router); // <-- Подключаем роутер к приложению
app.mount('#app');