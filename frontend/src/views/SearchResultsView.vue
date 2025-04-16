<script setup>
    import { ref, watch } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import MasterHead from '@/components/MasterHead.vue';
    import KebabMenu from '@/components/KebabMenu.vue';
    import ShareOverlay from "../components/ShareOverlay.vue";
    import VideoCard from "../components/VideoCard.vue";
    import LoadingState from '@/components/LoadingState.vue'; // Импортируем новый компонент
    import { API_BASE_URL } from '@/assets/config.js';

    const route = useRoute(); // Переименовано для ясности
    const router = useRouter(); 
    const searchResults = ref([]);
    const isLoading = ref(false);
    const errorMessage = ref('');
    const currentVideoId = ref('');
    const kebabMenuRef = ref(null);
    const shareRef = ref(null);

    const fetchSearchResults = async (query) => {
    if (!query) {
        searchResults.value = [];
        return;
    }

    try {
        isLoading.value = true;
        errorMessage.value = '';
        
        const response = await fetch(`${API_BASE_URL}/api/Search?query=${encodeURIComponent(query)}`);
        
        if (!response.ok) {
        throw new Error('Ошибка при загрузке результатов');
        }
        
        const data = await response.json();
        searchResults.value = data;
    } catch (error) {
        errorMessage.value = error.message || 'Произошла ошибка';
        searchResults.value = [];
    } finally {
        isLoading.value = false;
    }
    };

    // const toggleSubscribe = (userId) => {
    // const user = searchResults.value.find(v => v.user.id === userId)?.user;
    // if (user) {
    //     user.isSubscribed = !user.isSubscribed;
    //     // Здесь можно добавить вызов API для обновления подписки
    // }
    // };

    const navigateToVideo = (videoId) => {
        console.log(videoId)
        router.push(`/video/${videoId}`);
    };

    const handleKebabClick = ({ videoId, buttonElement }) => {
        currentVideoId.value = videoId;
        kebabMenuRef.value?.openMenu(buttonElement);
    };
    
    const handleShareClick = () => {
        // Проверяем, что ссылка существует и имеет метод
        if (shareRef.value && typeof shareRef.value.openMenu === 'function') {
        shareRef.value.openMenu();
        } else {
        console.error('ShareOverlay ref is not properly set or missing openMenu method');
        }
        
    };

    watch(() => route.query.q, (newQuery) => {
        searchResults.value = [];
        fetchSearchResults(newQuery);
    }, { immediate: true });
</script>

<template>
    <MasterHead />
    <KebabMenu 
        ref="kebabMenuRef"
        @close="currentVideoId = ''"
        @share="handleShareClick"
    />
    <ShareOverlay
        ref="shareRef" 
        :videoId="currentVideoId"
    />
    <div class="search-results-container">
      <!-- Используем новый компонент -->
      <LoadingState v-if="isLoading" />
      
      <!-- Сообщение об ошибке -->
      <div v-if="errorMessage" class="error-state">
        {{ errorMessage }}
      </div>
  
      <!-- Результаты поиска -->
      <div v-if="!isLoading && !errorMessage" class="results-grid">
        <div v-if="searchResults.length === 0" class="empty-results">
            Ничего не найдено
        </div>
  
        <VideoCard
            class="video-card"
            v-for="video in searchResults"
            :key="video.id"
            :video="video"
            row-layout
            @kebab-click="handleKebabClick"
            @click="navigateToVideo(video.id)"        
        />
      </div>
    </div>
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