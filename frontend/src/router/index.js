import { createRouter, createWebHistory } from 'vue-router';
import MainPage from '../views/MainPage.vue';
import VideoPage from '../views/VideoPage.vue';
import Auth from '../views/Auth.vue';
import Reg from '../views/Reg.vue';
import FogPass from '../views/FogPass.vue';
import ResetPassword from '../views/ResetPassword.vue';
import SearchResultsView from '../views/SearchResultsView.vue';

const routes = [
    { path: '/', component: MainPage },
    { path: '/video/:id', component: VideoPage },
    { path: '/login', component: Auth },
    { path: '/register', component: Reg },
    { path: '/forgot-password', component: FogPass },
    { path: '/reset-password', component: ResetPassword},
    { 
        path: '/search', 
        component: SearchResultsView,
        props: (route) => ({ query: route.query.q }) 
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;