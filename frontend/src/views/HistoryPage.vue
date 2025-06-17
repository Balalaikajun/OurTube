<script setup>
    import { ref, onMounted, onUnmounted, computed, nextTick, provide, watch  } from "vue";
    import MasterHead from "../components/Solid/MasterHead.vue";
    import PlaylistOverlay from "@/components/Playlist/PlaylistsOverlay.vue";
    import VideosPresentation from "@/components/Video/VideosPresentation.vue";

    const saveOpen = (videoId) => {
      playlistRef.value.toggleMenu(videoId);
    }
</script>
<template>
    <MasterHead />
    <PlaylistOverlay ref="playlistRef" 
    />
    <VideosPresentation
        request="history"
        context="search"
        :is-infinite-scroll="true"
        :row-layout=true
        @add-to-playlist="saveOpen"
    />
        <!-- :search-query="searchQuery" -->
</template>
<style scoped>

</style>