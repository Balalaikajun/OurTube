<script setup>
    import { defineAsyncComponent, computed } from "vue";
    import { useRoute } from 'vue-router';
    const route = useRoute();
    
    const MainPage = defineAsyncComponent(() => import("./views/MainPage.vue"));
    const VideoPage = defineAsyncComponent(() => import("./views/VideoPage.vue"));
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
        if (route.path === "/reset-password") return Reset;
        if (route.path === "/search") return SearchResults;
        
        return MainPage; // или 404
    });
</script>

<template>
    <!-- <MainPage v-if="showMainPage" />
    <VideoPage v-else-if="showVideoPage" />
    <Auth v-else-if="showAuth" />
    <Reg v-else-if="showReg" />
    <FogPass v-else-if="showFogPass" />
    <ResetPassword v-else-if="showReset" />
    <SearchResultsView v-else-if="showSearchResults" /> -->
    <Suspense>
        <component :is="currentComponent" />
    </Suspense>
</template>

<style scoped>
    
</style>