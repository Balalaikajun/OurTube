<script setup>
    import { ref, onMounted, onUnmounted, computed, nextTick, watch } from "vue";
    import { useRoute, useRouter } from 'vue-router';
    import VideoCard from "@/components/Video/VideoCard.vue";
    import KebabMenu from "../Kebab/KebabMenu.vue";
    import ShareOverlay from "../Kebab/ShareOverlay.vue";
    import LoadingState from "@/components/Solid/LoadingState.vue";
    import { API_BASE_URL } from "@/assets/config.js";
    import useInfiniteScroll from "@/assets/utils/useInfiniteScroll.js";

    const props = defineProps({
        errorMessage: {
            type: String,
            default: ""
        },
        request: {
            type: String,
            required: true,
            validator: (value) => ['recomend', 'search'].includes(value)
        },
        context: {
            type: String,
            required: true,
            validator: (value) => ['recomend', 'aside-recomend', 'search'].includes(value)
        },
        searchQuery: {
            type: String,
            default: ""
        },
        blocksInRow: {
            type: Number,
            default: 4
        },
        isInfiniteScroll: {
            type: Boolean,
            default: true
        },              
        scrollElement: {
            type: String,
            default: 'window'
        },          
        rowLayout: {
            type: Boolean,
            default: false
        }
    });

    const router = useRouter();
    const currentVideoId = ref(0);
    const parentWidth = ref(0);
    const kebabMenuRef = ref(null);
    const shareRef = ref(null);

    const emit = defineEmits(['load-more', 'add-to-playlist']);

    // Функции для работы с видео
    const handleKebabClick = ({ videoId, buttonElement }) => {
        currentVideoId.value = videoId;
        kebabMenuRef.value?.openMenu(buttonElement);
    };

    const handleKebabClose = () => {
        if (!shareRef.value?.isOpen) {
            currentVideoId.value = '';
        }
    };

    const handleAddToPlaylist = () => {
        emit('add-to-playlist', currentVideoId.value);
    };

    const handleShareClick = () => {
        if (shareRef.value?.openMenu) {
            shareRef.value.openMenu();
        }
    };

    const navigateToVideo = (video) => {
        router.push(`/video/${video.id}`);
    };

    // Логика бесконечной прокрутки
    const fetchMethods = {
        async recomend(after) {
            const limit = computedBlocksInRow.value * 1;
            try {
                const response = await fetch(`${API_BASE_URL}/api/Recommendation?limit=${limit}&after=${after || 0}`);
                if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
                return await response.json();
            } catch (error) {
                console.error('Ошибка получения рекомендаций:', error);
                return { videos: [], nextAfter: 0 };
            }
        },
        
        async search(after) {
            if (!props.searchQuery.trim()) return { videos: [], nextAfter: 0 };
            const limit = computedBlocksInRow.value * 1;
            try {
                const response = await fetch(`${API_BASE_URL}/api/Search?query=${encodeURIComponent(props.searchQuery)}&limit=${limit}&after=${after || 0}`);
                if (!response.ok) throw new Error(`Ошибка ${response.status}: ${response.statusText}`);
                return await response.json();
            } catch (error) {
                console.error('Ошибка при выполнении поиска:', error);
                return { videos: [], nextAfter: 0 };
            }
        }
    };

    const { 
        data: videos, 
        observerTarget, 
        hasMore, 
        isLoading, 
        error: scrollError, 
        container,
        loadMore 
    } = useInfiniteScroll({
        fetchMethod: async (after) => {
            const result = await fetchMethods[props.request](after);
            emit('load-more');
            return result;
        },
        scrollElement: props.scrollElement,
        isEnabled: props.isInfiniteScroll,
        initialLoad: true
    });

    // Адаптивный дизайн
    const updateDimensions = () => {
        if (!container.value) return;
        const rect = container.value.getBoundingClientRect();
        parentWidth.value = rect.width - 20;
    };

    const adaptiveView = async () => {
        await nextTick();
        updateDimensions();
        
        if (!container.value) return;

        const gap = computedBlocksInRow.value > 1 
            ? Math.max(10, Math.floor((parentWidth.value - (parseFloat(computedBlockWidth.value) * computedBlocksInRow.value)) / (computedBlocksInRow.value - 1))) 
            : 0;
        
        container.value.style.gap = `30px ${Math.floor(gap)}px`;
    };

    const computedBlocksInRow = computed(() => {
        if (props.rowLayout || props.context === "aside-recomend") return 1;
        if (parentWidth.value < 600) return 1;
        if (parentWidth.value < 800) return 2;
        if (parentWidth.value < 1200) return 3;
        if (parentWidth.value < 1920) return 4;
        return 5;
    });

    const computedBlockWidth = computed(() => {
        if (props.rowLayout || props.context === "aside-recomend") return "100%";
        if (parentWidth.value < 600) return `${parentWidth.value}px`;
        if (parentWidth.value < 800) return `${Math.floor(parentWidth.value * 0.49)}px`;
        if (parentWidth.value < 1200) return `${Math.floor(parentWidth.value * 0.32)}px`;
        if (parentWidth.value < 1920) return `${Math.floor(parentWidth.value * 0.24)}px`;
        return `${Math.floor(parentWidth.value * 0.19)}px`;
    });

    onMounted(async () => {
        await nextTick();
        await adaptiveView();
        window.addEventListener('resize', adaptiveView);
    });

    onUnmounted(() => {
        window.removeEventListener('resize', adaptiveView);
    });

    watch(() => props.request, () => loadMore(true));
    watch(() => props.searchQuery, (newVal, oldVal) => {
        if (newVal !== oldVal) loadMore(true);
    }, { immediate: true });
</script>

<template>
    <KebabMenu 
        ref="kebabMenuRef"
        @add-to-playlist="handleAddToPlaylist"
        @share="handleShareClick"
        @close="handleKebabClose"
    />
    <ShareOverlay
        ref="shareRef" 
        :videoId="currentVideoId"
    />
    <div class="container-wrapper" :class="[context == 'recomend' || context == 'search' ? 'standart-recomend' : `aside-recomend`, { 'row-layout': rowLayout || context == 'aside-recomend' }]">
        <div v-if="!isLoading && errorMessage.length > 0" class="results-grid">
            <div v-if="videos.length === 0" class="empty-results">
                Ничего не найдено
            </div>
        </div>
        <div v-if="scrollError" class="error-state">
            {{ scrollError }}
        </div>
        <div v-else ref="container" class="container" style="width: 100%;">
            <VideoCard
                v-for="video in videos"
                :video="video"
                :key="video.id"
                :row-layout="rowLayout"
                @click="navigateToVideo(video)"
                @kebab-click="handleKebabClick"
                :style="{ width: computedBlockWidth }"
            />
            <LoadingState v-if="isLoading"/>
            <div ref="observerTarget" class="observer-target" v-if="isInfiniteScroll && hasMore"></div>
        </div>
    </div>
</template>

<style scoped>
    .container-wrapper {
        display: flex;
        flex-wrap: wrap;
        box-sizing: border-box !important;
        container-type: inline-size;
        container-name: recommendations-container;
    }
    .container-wrapper.standart-recomend {
        width: 100%;
        padding: 20px 100px;
        margin-top: 70px;
    }
    .container-wrapper.aside-recomend {
        width: 100%;
        padding: 0px;
    }
    .container {
        display: flex;
        flex-wrap: wrap;
    }
    .row-layout {
        display: grid;
        columns: 1;
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

    .observer-target {
        width: 100%;
        height: 1px;
        margin: 0;
        padding: 0;
        background: #f39e60;
    }
</style>