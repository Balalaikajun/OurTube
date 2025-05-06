<script setup>
    import { ref, watch } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import MasterHead from '@/components/MasterHead.vue';
    import VideoContentPresentation from '@/components/VideoContentPresentation.vue';

    const route = useRoute(); // Переименовано для ясности
    const searchResults = ref([]);
    const isLoading = ref(false);
    const errorMessage = ref('');
    const currentVideoId = ref('');
    const kebabMenuRef = ref(null);
    const shareRef = ref(null);
    const searchQuery = ref('');

    watch(() => route.query.q, (newQuery) => {
        console.log("смена запроса")
        searchQuery.value = newQuery || '';
    }, { immediate: true });
</script>

<template>
    <MasterHead />
    <VideoContentPresentation
        context="search"
        :search-query="searchQuery"
        :is-infinite-scroll="true"
        :row-layout=true
        @load-more="loadMoreVideos"
    />
</template>
  

<style scoped>
    .search-results-container {
        box-sizing: border-box;
        margin-top: 70px;
        padding: 0 100px;
        width: 100%;
        margin-left: auto;
        margin-right: auto;
        padding-top: 20px;
    }

    .error-state {
        color: #f39e60;
        text-transform: uppercase;
        text-align: center;
        padding: 20px;
        background: rgba(255, 0, 0, 0.05);
        border-radius: 8px;
        margin: 20px 0;
    }

    .empty-results {
        text-align: center;
        color: #f39e60;
        font-size: 18px;
        padding: 40px 0;
    }

    .results-grid {
        display: grid;
        gap: 20px;
    }

    @media (max-width: 768px) {
        .search-results-container {
            padding: 10px;
        }
        
        .results-grid {
            grid-template-columns: 1fr;
        }
    }
</style>