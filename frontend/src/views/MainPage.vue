<script setup>
import { ref, onMounted, onUnmounted, computed } from "vue";
import { useRouter } from "vue-router";
import MasterHead from "../components/MasterHead.vue";
import LoadingState from "@/components/LoadingState.vue";
import KebabMenu from "../components/KebabMenu.vue";
import VideoCard from "../components/VideoCard.vue";
import VideoCardSkeleton from "../components/VideoCardSkeleton.vue";
import { API_BASE_URL } from "@/assets/config.js";

const router = useRouter();
const videos = ref([]);
const loading = ref(false);
const loadingMore = ref(false);
const errorMessage = ref("");
const currentVideoId = ref("");
const kebabMenuRef = ref(null);
const nextAfter = ref(0);
const hasMore = ref(true);
const isMobile = ref(false);
const videosPlace = ref(null);
const windowWidth = ref(window.innerWidth);
const parentWidth = ref(windowWidth - 200);

// Определяем количество колонок в зависимости от ширины экрана
const blocksInRow = computed(() => {
  const width = windowWidth.value;
  if (width < 600) return 1;
  if (width < 800) return 2;
  if (width < 1200) return 3;
  if (width < 1920) return 4;
  if (width >= 1920) return 5;
  return Math.floor(width / 200); // Для очень широких экранов
});

const blockWidth = computed(() => {
  const widthScreen = windowWidth.value;
  const widthParent = parentWidth.value;
  console.log(widthParent)
  if (widthScreen < 600) return widthParent;
  if (widthScreen < 800) return widthParent * 0.49;
  if (widthScreen < 1200) return widthParent * 0.32;
  if (widthScreen < 1920) return widthParent * 0.24;
  if (widthScreen >= 1920) return widthParent * 0.19;
  return 200; // Для очень широких экранов
})

const adaptiveView = () => {
  const gap = blocksInRow.value > 1 ? Math.max(0, (parentWidth.value - (blockWidth.value * blocksInRow.value))) / (blocksInRow.value - 1) : 0;
  videosPlace.value.style.gap = `30px ${gap}px`;
}

function updateDimensions() {
  windowWidth.value = window.innerWidth;
  if (videosPlace.value) {
      parentWidth.value = parseFloat(window.getComputedStyle(videosPlace.value).width
    );
  }
}
// Определяем, нужно ли использовать row layout
// не актуально для текущей реализации
// const rowLayout = computed(() => {
//   return window.innerWidth / window.innerHeight < 1;
// });

// Количество скелетонов для заполнения строки
const skeletonsCount = computed(() => {
  const remainder = videos.value.length % blocksInRow.value;
  return remainder === 0 ? 0 : blocksInRow.value - remainder;
});

const fetchVideos = async (initial = false) => {
  if (loading.value || loadingMore.value || !hasMore.value) return;
  
  if (initial) {
    loading.value = true;
    videos.value = [];
    nextAfter.value = 0;
    hasMore.value = true;
  } else {
    loadingMore.value = true;
  }
  
  errorMessage.value = "";

  try {
    // Запрашиваем количество видео, кратное количеству колонок
    const limit = blocksInRow.value * 4; // 4 строки
    const response = await fetch(`${API_BASE_URL}/api/Recommendation?limit=${limit}&after=${nextAfter.value}`);
    const data = await response.json();
    
    if (!response.ok) throw new Error(data.message || "Ошибка при загрузке видео");
    
    videos.value = [...videos.value, ...data.videos];
    nextAfter.value = data.nextAfter;
    
    if (data.videos.length === 0) {
      hasMore.value = false;
      if (videos.value.length === 0) errorMessage.value = "Нет видео.";
    }
  } catch (error) {
    errorMessage.value = error.message;
    console.error("Ошибка загрузки видео:", error);
  } finally {
    loading.value = false;
    loadingMore.value = false;
  }
};

const handleScroll = () => {
  const { scrollTop, scrollHeight, clientHeight } = document.documentElement;
  const isNearBottom = scrollTop + clientHeight >= scrollHeight - 500;
  
  if (isNearBottom && hasMore.value && !loadingMore.value) {
    fetchVideos();
  }
};

const navigateToVideo = (videoId) => {
  router.push(`/video/${videoId}`);
};

const handleKebabClick = ({ videoId, buttonElement }) => {
  currentVideoId.value = videoId;
  kebabMenuRef.value?.openMenu(buttonElement);
};

onMounted(async () => {
  updateDimensions();
  window.addEventListener('resize', updateDimensions);
  window.addEventListener('resize', adaptiveView);
  window.addEventListener('scroll', handleScroll);
  await fetchVideos(true);
  adaptiveView();
});

onUnmounted(() => {
  window.removeEventListener('resize', adaptiveView);
  window.removeEventListener('resize', updateDimensions);
  window.removeEventListener('scroll', handleScroll);
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
    <div ref="videosPlace" v-if="!errorMessage && !loading" class="videos">
      <!-- Реальные видео -->
      <VideoCard
        class="video-card"
        :style="{ width: `${blockWidth}px` }"
        v-for="video in videos"
        :key="video.id"
        :video="video"
        @click="navigateToVideo(video.id)"
        @kebab-click="handleKebabClick"
      />

      <!-- Скелетоны для заполнения последней строки -->
      <VideoCardSkeleton
        class="video-card"
        :style="{ width: `${blockWidth}px` }"
        v-for="i in skeletonsCount"
        :key="`skeleton-${i}`"
      />
    </div>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <LoadingState v-if="loading" />
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

  .videos {
      display: flex;
      flex-wrap: wrap;
      width: 100%;
  }

  /* @media (min-width: 1920px) {
    .video-card {
      width: 200px;
    }
  }

  @media (min-width: 1200px) and (max-width: 1920px) {
    .video-card {
      width: 24%;
    }
  }

  @media (max-width: 1200px) {
    .video-card {
      width: 32%;
    }
  }

  @media (max-width: 800px) {
    .video-card {
      width: 49%;
    }
  }

  @media (max-width: 600px) {
    .video-list {
      padding: 20px 10px;
    }
    .video-card {
      width: 100%;
    }
    .videos {
      row-gap: 40px;
      justify-content: left;
    }
  } */
</style>