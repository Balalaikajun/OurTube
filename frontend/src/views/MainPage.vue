<script setup>
  import { ref, onMounted, onBeforeUnmount, watch, nextTick } from "vue";
  import { useRouter } from "vue-router";
  import VideoCard from "../components/VideoBlockCard.vue";
  import MasterHead from "../components/MasterHead.vue";
  import LoadingState from "@/components/LoadingState.vue"; // Импортируем компонент
  import { API_BASE_URL } from "@/assets/config.js";

  const router = useRouter();
  const videos = ref([]);
  const loading = ref(false);
  const errorMessage = ref("");
// const videosGridRef = ref(null);
// const gapSize = ref(15); // Фиксированный отступ 15px

// const calculateLayout = () => {
//   if (!videosGridRef.value) return;
  
//   const container = videosGridRef.value;
//   const cards = container.querySelectorAll('.video-card');
//   if (cards.length === 0) return;
  
//   const containerWidth = container.clientWidth;
//   const cardWidth = cards[0].clientWidth;
//   const cardsPerRow = Math.floor(containerWidth / cardWidth);
  
//   if (cardsPerRow <= 1) {
//     // Если карточки не помещаются в одну строку
//     container.style.justifyContent = 'center';
//     gapSize.value = 15;
//     return;
//   }
  
//   const totalGap = containerWidth - (cardsPerRow * cardWidth);
//   const gap = totalGap / (cardsPerRow - 1);
  
//   if (gap > 15) {
//     // Если расчетный отступ больше 10px - центрируем
//     container.style.justifyContent = 'center';
//     gapSize.value = 10;
//   } else {
//     // Иначе растягиваем на всю ширину
//     container.style.justifyContent = 'space-between';
//     gapSize.value = gap;
//   }
// };

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
    // nextTick(() => {
    //   calculateLayout();
    // });
  }
};

const navigateToVideo = (videoId) => {
  router.push(`/video/${videoId}`);
};

onMounted(async () => {
  await fetchVideos();
  // window.addEventListener('resize', calculateLayout);
});

onBeforeUnmount(() => {
  // window.removeEventListener('resize', calculateLayout);
});

// watch(videos, () => {
//   nextTick(calculateLayout);
// }, { deep: true });
</script>

<template>
  <MasterHead/>
  <div class="video-list">
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    
    <!-- Заменяем старый лоадер на компонент LoadingState -->
    <LoadingState v-if="loading" />
    
    <div v-else ref="videosGridRef" class="videos-grid">
      <VideoCard
        v-for="video in videos"
        :key="video.id"
        :video="video"
        @click="navigateToVideo(video.id)"
      />
    </div>
  </div>
</template>

<style scoped>
  .video-list {
    box-sizing: border-box;
    width: 100%;
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

  .videos-grid .video-card {
    flex: 0 0 auto;
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