<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import VideoPlayer from "@/components/VideoPlayer.vue";
import LoadingState from "@/components/LoadingState.vue"; // Импортируем компонент загрузки
import { API_BASE_URL } from "@/assets/config.js";
import { MINIO_BASE_URL } from "@/assets/config.js";

const route = useRoute();
const videoId = route.params.id;
const videoData = ref(null);
const hlsUrl = ref("");
const isLoading = ref(true); // Добавляем состояние загрузки
const error = ref(null); // Добавляем обработку ошибок

const fetchVideoData = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    const response = await fetch(`${API_BASE_URL}/api/Video/${videoId}`);
    if (!response.ok) throw new Error("Ошибка загрузки видео");
    
    const data = await response.json();
    videoData.value = data;
    
    if (data.files && data.files.length > 0) {
      const file = data.files[0];
      hlsUrl.value = `${MINIO_BASE_URL}/videos/0f9c22f2-ce05-4fbc-bba5-8c706196ce89/1080/playlist.m3u8`;
    }
  } catch (err) {
    error.value = err.message;
    console.error("Ошибка загрузки данных видео:", err);
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  fetchVideoData();
});
</script>

<template>
  <div class="video-page">
    <!-- Отображаем загрузку -->
    <LoadingState v-if="isLoading" />
    
    <!-- Отображаем ошибку -->
    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    
    <!-- Отображаем контент когда загружено -->
    <template v-else>
      <VideoPlayer v-if="hlsUrl" :video-src="hlsUrl" />
      <div v-else class="no-video">Видео недоступно</div>
      
      <div v-if="videoData" class="video-info">
        <h1>{{ videoData.title }}</h1>
        <p>{{ videoData.description }}</p>
        <!-- Другая информация о видео -->
      </div>
    </template>
  </div>
</template>

<style scoped>
.video-page {
  padding: 20px;
}

.error-message {
  color: #ff4d4f;
  text-align: center;
  padding: 40px 0;
}

.no-video {
  color: #f39e60;
  text-align: center;
  padding: 40px 0;
}

.video-info {
  margin-top: 20px;
  color: #f3f0e9;
}

.video-info h1 {
  font-size: 24px;
  margin-bottom: 10px;
}

.video-info p {
  font-size: 16px;
  line-height: 1.5;
}
</style>