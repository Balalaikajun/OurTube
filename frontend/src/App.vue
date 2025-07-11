<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import MasterHead from './components/Solid/MasterHead.vue'
import UploadVideoModal from './components/Video/UploadVideoModal.vue'
import { saveUserDataToLocalStorage } from '@/assets/utils/userServiсe.js'

const route = useRoute()
const showHeader = computed(() => !route.meta.hideHeader)

const uploadVideo = ref(null);

const openUpload = () => {
    uploadVideo.value.openMenu();
}

// Проверяем токен и загружаем данные при монтировании
    onMounted(async () => {
        // console.log(window.innerWidth);
        // console.log(window.devicePixelRatio);
        await saveUserDataToLocalStorage();
    });
</script>

<template>
    <MasterHead v-if="showHeader" @open-upload="openUpload"/>
    <UploadVideoModal ref="uploadVideo" v-if="showHeader"/>
    <router-view :key="$route.fullPath"/>
</template>

<style scoped>
    
</style>