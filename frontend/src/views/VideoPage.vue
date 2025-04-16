<script setup>
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import MasterHead from "../components/MasterHead.vue";
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

const ensureHttpUrl = (url) => {
  if (!url) return '';
  return url.startsWith('http') ? url : `http://${url}`;
};

const fetchVideoData = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    const response = await fetch(`${API_BASE_URL}/api/Video/${videoId}`);
    if (!response.ok) throw new Error("Ошибка загрузки видео");
    
    const data = await response.json();
    videoData.value = data;
    
    if (data.files?.length) {
      // Используем реальные данные из API
      const file = data.files[0];
      // console.log(data)
      // console.log(file.fileName)
      // Убеждаемся, что URL начинается с http://
      // console.log(`${MINIO_BASE_URL}/${file.fileName}`, 39)
      hlsUrl.value = ensureHttpUrl(`${MINIO_BASE_URL}/videos/${file.fileName}`);
      // console.log("hlsURL", hlsUrl.value)
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
  <MasterHead />
  <main class="video-page">
    <!-- Состояние загрузки -->
    <LoadingState v-if="isLoading" />
    
    <!-- Состояние ошибки -->
    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    
    <!-- Основной контент -->
    <template v-else>
      <section class="video-container">
        <VideoPlayer 
          v-if="hlsUrl" 
          :video-src="hlsUrl" 
          :poster="videoData?.thumbnailUrl"
          class="video-player"
        />
        <div v-else class="no-video">
          Видео недоступно
        </div>
      </section>
      
      <section 
        v-if="videoData" 
        class="video-info"
      >
        <h1 class="video-title">{{ videoData.title }}</h1>
        <div class="video-meta">
          <span v-if="videoData.views">{{ videoData.views }} просмотров</span>
          <span v-if="videoData.uploadDate">{{ formatDate(videoData.uploadDate) }}</span>
        </div>
        <p class="video-description">{{ videoData.description }}</p>
      </section>
    </template>
  </main>
</template>

<style scoped>
.video-page {
  box-sizing: border-box;
  width: 100%;
  padding: 20px 100px;
  margin-top: 70px;
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