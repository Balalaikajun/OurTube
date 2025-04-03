<script setup>
    import VideoPage from "@/views/VideoPage.vue";
    import Auth from "./views/Auth.vue";
    import Reg from "./views/Reg.vue";
    import FogPass from "./views/FogPass.vue";
    import MainPage from "./views/MainPage.vue";
    import ResetPassword from "./views/ResetPassword.vue";
    import SearchResultsView from "./views/SearchResultsView.vue";

    import { computed } from "vue";
    import { useRoute } from 'vue-router';
    const route = useRoute();
    
    const showMainPage = computed(() => route.path === "/");
    const showAuth = computed(() => route.path === "/login");
    const showReg = computed(() => route.path === "/register");
    const showFogPass = computed(() => route.path === "/forgot-password");
    const showReset = computed(() => route.path === "/reset-password");
    const showVideoPage = computed(() => route.path.startsWith("/video/"));
    const showSearchResults = computed(() => route.path.startsWith("/search"));
</script>

<template>
    <MainPage v-if="showMainPage" />
    <VideoPage v-else-if="showVideoPage" />
    <Auth v-else-if="showAuth" />
    <Reg v-else-if="showReg" />
    <FogPass v-else-if="showFogPass" />
    <ResetPassword v-else-if="showReset" />
    <SearchResultsView v-else-if="showSearchResults" />
</template>

<style scoped>
    
</style>