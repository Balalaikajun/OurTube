<script setup>
import { ref, onMounted, onUnmounted, computed, nextTick  } from "vue";
// import { useRouter } from "vue-router";
import MasterHead from "../components/Solid/MasterHead.vue";
import PlaylistOverlay from "@/components/Playlist/PlaylistsOverlay.vue";
// import LoadingState from "@/components/Solid/LoadingState.vue";
// import KebabMenu from "../components/Kebab/KebabMenu.vue";
// import ShareOverlay from "../components/Kebab/ShareOverlay.vue";
// import VideoCard from "../components/Video/VideoCard.vue";
// import VideoCardSkeleton from "../components/VideoCardSkeleton.vue";
// import { API_BASE_URL } from "@/assets/config.js";
import VideosPresentation from "@/components/Video/VideosPresentation.vue";

const loadingMore = ref(false);
const errorMessage = ref("");
const currentVideoId = ref("");
const kebabMenuRef = ref(null);
const shareRef = ref(null);
const nextAfter = ref(0);
const hasMore = ref(true);
const isMobile = ref(false);
const videosPlace = ref(null);
const playlistRef = ref(null);

// const handleScroll = () => {
//   const { scrollTop, scrollHeight, clientHeight } = document.documentElement;
//   const isNearBottom = scrollTop + clientHeight >= scrollHeight - 500;
  
//   if (isNearBottom && hasMore.value && !loadingMore.value) {
//     // fetchVideos();
//   }
// };

const saveOpen = (videoId) => {
    console.log("save")
    playlistRef.value.toggleMenu(videoId);
}

// onMounted(async () => {
//   window.addEventListener('scroll', handleScroll);
//   // document.title = "Главная";
// });

// onUnmounted(() => {
//   window.removeEventListener('scroll', handleScroll);
// });
</script>

<template>
  <MasterHead/>
  <PlaylistOverlay 
    ref="playlistRef" 
  />
  <VideosPresentation
    request="recomend"
    context="recomend"
    @add-to-playlist="saveOpen"
    :is-infinite-scroll="true"
  />
    <!-- @load-more="loadMoreVideos" -->
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

  /* @media (max-width: 600px) {
    .video-list {
      padding: 20px 10px;
    }
  } */
</style>