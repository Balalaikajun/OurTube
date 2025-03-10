<script setup>
    import VideoPlayer from "@/components/VideoPlayer.vue";
    import Auth from "./views/Auth.vue";
    import Reg from "./views/Reg.vue";
    import FogPass from "./views/FogPass.vue";
    import MainPage from "./views/MainPage.vue";
    import VideoBlock from "./components/VideoBlockCard.vue";

    import { computed } from "vue";
    import { useRoute } from 'vue-router';
    const route = useRoute();
    const authRoutes = ["/identity/login"];
    const regRoutes = ["/identity/register"];
    const fogPassRoutes = ["/identity/forgot-password"];
    const mainPage = ["/"];
    const showAuth = computed(() => authRoutes.includes(route.path));
    const showReg = computed(() => regRoutes.includes(route.path));
    const showFogPass = computed(() => fogPassRoutes.includes(route.path));
    const showVideoPlayer = computed(() => route.path.startsWith("/video/"));
</script>

<template>
    <div>
        <MainPage v-if="mainPage" />
        <VideoPlayer v-if="showVideoPlayer" />
        <Auth v-if="showAuth" />
        <Reg v-if="showReg"/>
        <FogPass v-if="showFogPass"/>
    </div>
    
</template>

<style scoped>
    
</style>