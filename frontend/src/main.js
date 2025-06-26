import './assets/styles/main.css'

import { createApp } from 'vue'
import { authDirective } from '@/assets/utils/authDirective.js';
import App from './App.vue'
import router from './router/index.js'; // <-- Подключаем роутер
import axios  from 'axios'

axios.defaults.withCredentials = true
const app = createApp(App);
app.directive('auth', authDirective);
app.use(router); // <-- Подключаем роутер к приложению
app.mount('#app');