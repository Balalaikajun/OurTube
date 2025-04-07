<script setup>
import { ref, onMounted, onUnmounted, computed, nextTick  } from "vue";
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
const windowWidth = ref(0);
const parentWidth = ref(0);

// Определяем количество колонок в зависимости от ширины экрана
const blocksInRow = computed(() => {
  windowWidth.value = window.innerWidth;
  const width = windowWidth.value;
  if (width < 600) {
    console.log(width); //
    return 1;
  } 
  if (width < 800) {
    console.log(width); //
    return 2;
  }    
  if (width < 1200) {
    console.log(width); // 
    return 3;
  }
  if (width < 1920) {
    console.log(width); //
    return 4;
  }

  if (width >= 1920)
  {
    console.log(width); //
    return 5;
  } 
  return Math.floor(width / 200); // Для очень широких экранов
});

const blockWidth = computed(() => {
  const widthScreen = windowWidth.value;
  const widthParent = parentWidth.value;
  if (widthScreen < 600) {
    console.log(widthScreen); //
    return widthScreen;
  } 
  if (widthScreen < 800) {
    console.log(widthParent * 0.49); //
    return widthParent * 0.49;
  }    
  if (widthScreen < 1200) {
    console.log(widthParent * 0.32); // 
    return widthParent * 0.32;
  }
  if (widthScreen < 1920) {
    console.log(widthParent * 0.24); //
    return widthParent * 0.24;
  }

  if (widthScreen >= 1920)
  {
    console.log(widthParent * 0.19); //
    return widthParent * 0.19;
  } 
  console.log(200); //
  return 200; // Для очень широких экранов
})

const adaptiveView = () => {
  console.log(window.getComputedStyle(videosPlace.value).width, 0) //
  console.log(windowWidth.value, 1) //
  windowWidth.value = window.innerWidth;
  console.log(windowWidth.value, 2) //
  parentWidth.value = parseFloat(window.getComputedStyle(videosPlace.value).width);

  const gap = blocksInRow.value > 1 ? Math.max(0, (parentWidth.value - (blockWidth.value * blocksInRow.value))) / (blocksInRow.value - 1) : 0;
  console.log(gap);
  videosPlace.value.style.gap = `30px ${gap}px`;
  console.log(window.getComputedStyle(videosPlace.value).gap) //
}

// function updateDimensions() {
//   windowWidth.value = window.innerWidth;
//   if (videosPlace.value) {
//       parentWidth.value = parseFloat(window.getComputedStyle(videosPlace.value).width);
//   }
// }

const skeletonsCount = computed(() => {
  const remainder = videos.value.length % blocksInRow.value; //ffffffffffffffffff
  return remainder === 0 ? 0 : blocksInRow.value - remainder;
});

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
  // updateDimensions();
  // await nextTick();
  // window.addEventListener('resize', updateDimensions);

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
    console.log(blocksInRow.value)
    // Запрашиваем количество видео, кратное количеству колонок
    const limit = blocksInRow.value * 4; // 4 строки
    const response = await fetch(`${API_BASE_URL}/api/Recommendation?limit=${limit}&after=${nextAfter.value}`);
    const data = await response.json();
    
    if (!response.ok) throw new Error(data.message || "Ошибка при загрузке видео");
    
    videos.value = [...videos.value, ...data.videos];
    console.log(videos.value.length)
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
  } */

  @media (max-width: 600px) {
    .video-list {
      padding: 20px 10px;
    }
  }
</style>