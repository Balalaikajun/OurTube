<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import MasterHead from "../components/MasterHead.vue";
import LoadingState from "@/components/LoadingState.vue";
import KebabMenu from "../components/KebabMenu.vue";
import VideoCard from "../components/VideoCard.vue";
import { API_BASE_URL } from "@/assets/config.js";

const router = useRouter();
const videos = ref([]);
const loading = ref(false);
const errorMessage = ref("");
const currentVideoId = ref("");
const kebabMenuRef = ref(null);

const fetchVideos = async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const response = await fetch(`${API_BASE_URL}/api/Recommendation?limit=10&after=0`);
    const data = await response.json();
    
    if (!response.ok) throw new Error(data.message || "Ошибка при загрузке видео");
    
    videos.value = [...data.videos];
    if (videos.value.length === 0) errorMessage.value = "Нет видео.";
  } catch (error) {
    errorMessage.value = error.message;
    console.error("Ошибка загрузки видео:", error);
  } finally {
    loading.value = false;
  }
};

const navigateToVideo = (videoId) => {
  router.push(`/video/${videoId}`);
};

const handleKebabClick = ({ videoId, buttonElement }) => {
  if (!buttonElement) {
    console.error('Button element is missing');
    return;
  }
  currentVideoId.value = videoId;
  kebabMenuRef.value?.openMenu(buttonElement);
};

onMounted(async () => {
  await fetchVideos();
});
</script>

<template>
  <MasterHead/>
  <KebabMenu 
    ref="kebabMenuRef" 
    :videoId="currentVideoId"
    @close="currentVideoId = ''"
  />
  <div class="video-list">
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <LoadingState v-if="loading" />
    
    <div v-else class="videos-grid">
      <VideoCard
        v-for="video in videos"
        :key="video.id"
        :video="video"
        @click="navigateToVideo(video.id)"
        @kebab-click="handleKebabClick"
      />
    </div>
  </div>
</template>

<style scoped>
    .video-list {
        box-sizing: border-box;
        width: 100%;
        height: 1000vh;
        padding: 20px 100px;
        margin-top: 70px;
    }

    .error {
        color: red;
        text-align: center;
        margin-top: 10px;
    }

    .videos-grid {
        display: flex;
        flex-wrap: wrap;
        width: 100%;
        /* gap: 15px; */
    }

    @media (max-width: 768px) {
        .video-list {
          padding: 20px;
        }
        
        .videos-grid .video-card {
          width: 200px;
        }
    }

    @media (max-width: 480px) {
        .videos-grid .video-card {
          width: 160px;
        }
    }
</style>