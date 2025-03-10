import { createRouter, createWebHistory } from 'vue-router';
import MainPage from '../views/MainPage.vue';
import VideoPage from '../views/VideoPage.vue';
import Auth from '../views/Auth.vue';
import Reg from '../views/Reg.vue';
import FogPass from '../views/FogPass.vue';

const routes = [
    { path: '/', component: MainPage },
    { path: '/video/:id', component: VideoPage },
    { path: '/identity/login', component: Auth },
    { path: '/identity/register', component: Reg },
    { path: '/identity/forgot-password', component: FogPass },
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;
