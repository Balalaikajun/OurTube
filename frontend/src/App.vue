<script setup>
    import { onMounted } from "vue";
    import { saveUserDataToLocalStorage } from "@/assets/utils/userServiсe.js";

        // Проверяем токен и загружаем данные при монтировании
    onMounted(async () => {
        console.log(window.innerWidth);
        console.log(window.devicePixelRatio);
        await saveUserDataToLocalStorage();
    });
</script>

<template>
    <router-view :key="$route.fullPath" />
</template>

<style scoped>
    
</style>