import { createRouter, createWebHistory } from 'vue-router';

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
        },
        props: route => ({ id: Number(route.params.id) }) // Передаем параметры маршрута как props
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
        meta: { title: 'Регистрация' }
    },
    { 
        path: '/forgot-password', 
        component: () => import('../views/FogPassPage.vue'),
        meta: { title: 'Восстановление' }
    },
    { 
        path: '/reset-password', 
        component: () => import('../views/ResetPasswordPage.vue'),
        meta: { title: 'Сброс' }
    },
    { 
        path: '/search', 
        component: () => import('../views/SearchResultPage.vue'),
        meta: { title: 'Поиск' },
        props: route => ({ query: route.query.q })
    }
];

const router = createRouter({
    // Если приложение в корне сервера, можно просто createWebHistory()
    history: createWebHistory(import.meta.env.BASE_URL),
    
    routes,
    
    scrollBehavior(to, from, savedPosition) {
      // Прокрутка к якорю если есть hash
      if (to.hash) {
        return { el: to.hash, behavior: 'smooth' }
      }
      // Возврат к сохраненной позиции или в начало
      return savedPosition || { top: 0 }
    }
});

router.afterEach((to, from) => {
    console.log(`Navigated from ${from.path} to ${to.path}`);
});

// router.beforeEach((to) => {
//     document.title = to.meta.title ? `${to.meta.title} | MyApp` : 'MyApp';
// });

export default router;