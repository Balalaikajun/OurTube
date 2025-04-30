<script setup>
    import { defineAsyncComponent, computed } from "vue";
    import { useRoute } from 'vue-router';
    const route = useRoute();
    
    const MainPage = defineAsyncComponent({
        loader: () => import("./views/MainPage.vue"),
        loadingComponent: { template: '<div>Загрузка...</div>' },
        errorComponent: { template: '<div>Ошибка загрузки</div>' }
    });
    const VideoPage = defineAsyncComponent({
        loader: () => import("./views/VideoPage.vue"),
        loadingComponent: { template: '<div>Загрузка видео...</div>' },
        errorComponent: { template: '<div>Ошибка загрузки видео</div>' }
    });
    const AuthPage = defineAsyncComponent(() => import("./views/AuthPage.vue"));
    const RegPage = defineAsyncComponent(() => import("./views/RegPage.vue"));
    const FogPassPage = defineAsyncComponent(() => import("./views/FogPassPage.vue"));
    const ResetPasswordPage = defineAsyncComponent(() => import("./views/ResetPasswordPage.vue"));
    const SearchResultPage = defineAsyncComponent(() => import("./views/SearchResultPage.vue"));

    const currentComponent = computed(() => {
        if (route.path === "/") return MainPage;
        if (route.path.startsWith("/video/")) return VideoPage;
        if (route.path === "/login") return AuthPage;
        if (route.path === "/register") return RegPage;
        if (route.path === "/forgot-password") return FogPassPage;
        if (route.path === "/reset-password") return ResetPasswordPage;
        if (route.path === "/search") return SearchResultPage;
        
        return MainPage; // или 404
    });
</script>

<template>
    <Suspense>
        <component :is="currentComponent" />
    </Suspense>
</template>

<style scoped>
    
</style>