import { createRouter, createWebHistory } from 'vue-router'

const routes = [
    { 
        path: '/', 
        component: () => import('../views/MainPage.vue'),
        meta: { 
          title: 'Главная'
        }
    },
    { 
        path: '/video/:id', 
        component: () => import('../views/VideoPage.vue'),
        meta: { 
            title: 'Просмотр', //правки - на название видео
            requiresAuth: false 
        }
        // props: route => ({ id: Number(route.params.id) }) // Передаем параметры маршрута как props //правки
    },
    { 
        path: '/login', 
        component: () => import('../views/AuthPage.vue'),
        meta: { 
            title: 'Авторизация',
            hideHeader: true 
        }
    },
    { 
        path: '/register', 
        component: () => import('../views/RegPage.vue'),
        meta: { 
            title: 'Регистрация',
            hideHeader: true 
        }
    },
    { 
        path: '/forgot-password', 
        component: () => import('../views/FogPassPage.vue'),
        meta: { 
            title: 'Восстановление пароля',
            hideHeader: true 
        }
    },
    { 
        path: '/reset-password', 
        component: () => import('../views/ResetPasswordPage.vue'),
        meta: { 
            title: 'Сброс пароля',
            hideHeader: true 
        }
    },
    { 
        path: '/search', 
        component: () => import('../views/SearchResultPage.vue'),
        meta: { title: 'Поиск' },
        props: route => ({ query: route.query.q })
    }
    ,
    { 
        path: '/history', 
        component: () => import('../views/HistoryPage.vue'),
        meta: { title: 'История просмотра' }
    }
    ,
    { 
        path: '/account', 
        component: () => import('../views/AccountPage.vue'),
        meta: { title: 'Страница пользователя' }
    }
    ,
    { 
        path: '/playlists', 
        component: () => import('../views/PlaylistsPage.vue'),
        meta: { title: 'Плейлисты' }
    }
    ,
    { 
        path: '/playlist/:id', 
        component: () => import('../views/PlaylistPage.vue'),
        meta: { title: 'Плейлист' }
    }
];

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition;
        } else if (to.hash) {
            return { el: to.hash, behavior: 'smooth' };
        } else {
            return { top: 0 };
        }
    }
});

router.beforeEach((to, from, next) => {
    // Устанавливаем title из meta или дефолтный
    document.title = to.meta.title ? `${to.meta.title}` : 'OurTube';
    next();
});

router.afterEach((to, from) => {
    console.log(`Navigated from ${from.path} to ${to.path}`);
});

// router.beforeEach((to) => {
//     document.title = to.meta.title ? `${to.meta.title} | MyApp` : 'MyApp';
// });

export default router;