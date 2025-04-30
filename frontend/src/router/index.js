import { createRouter, createWebHistory } from 'vue-router';
import MainPage from '../views/MainPage.vue';
import VideoPage from '../views/VideoPage.vue';
import AuthPage from '../views/AuthPage.vue';
import RegPage from '../views/RegPage.vue';
import FogPassPage from '../views/FogPassPage.vue';
import ResetPassword from '../views/ResetPasswordPage.vue';
import SearchResultPage from '../views/SearchResultPage.vue';

const routes = [
    { path: '/', component: MainPage },
    { path: '/video/:id', component: VideoPage },
    { path: '/login', component: AuthPage },
    { path: '/register', component: RegPage },
    { path: '/forgot-password', component: FogPassPage },
    { path: '/reset-password', component: ResetPassword},
    { 
        path: '/search', 
        component: SearchResultPage,
        props: (route) => ({ query: route.query.q }) 
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;