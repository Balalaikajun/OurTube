<script setup>
    import VideoPlayer from "@/components/VideoPlayer.vue";
    import Auth from "./views/Auth.vue";
    import Reg from "./views/Reg.vue";
    import FogPass from "./views/FogPass.vue";
    import MainPage from "./views/MainPage.vue";
    import VideoBlock from "./components/VideoBlockCard.vue";

    import { computed } from "vue";
    import { useRoute } from 'vue-router';
    import ResetPassword from "./views/ResetPassword.vue";
    const route = useRoute();
    
    const showAuth = computed(() => route.path === "/login");
    const showReg = computed(() => route.path === "/register");
    const showFogPass = computed(() => route.path === "/forgot-password");
    const showReset = computed(() => route.path === "/reset-password");
    const showMainPage = computed(() => route.path === "/");
    const showVideoPlayer = computed(() => route.path.startsWith("/video/"));
</script>

<template>
    <MainPage v-if="showMainPage" />
    <VideoPlayer v-else-if="showVideoPlayer" />
    <Auth v-else-if="showAuth" />
    <Reg v-else-if="showReg" />
    <FogPass v-else-if="showFogPass" />
    <ResetPassword v-else-if="showReset" />
</template>

<style scoped>
    
</style>