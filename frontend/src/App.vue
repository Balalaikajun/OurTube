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
    const Auth = defineAsyncComponent(() => import("./views/Auth.vue"));
    const Reg = defineAsyncComponent(() => import("./views/Reg.vue"));
    const FogPass = defineAsyncComponent(() => import("./views/FogPass.vue"));
    const ResetPassword = defineAsyncComponent(() => import("./views/ResetPassword.vue"));
    const SearchResults = defineAsyncComponent(() => import("./views/SearchResultsView.vue"));

    const currentComponent = computed(() => {
        if (route.path === "/") return MainPage;
        if (route.path.startsWith("/video/")) return VideoPage;
        if (route.path === "/login") return Auth;
        if (route.path === "/register") return Reg;
        if (route.path === "/forgot-password") return FogPass;
        if (route.path === "/reset-password") return ResetPassword;
        if (route.path === "/search") return SearchResults;
        
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