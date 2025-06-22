<script setup>
    import { ref, onMounted, onUnmounted, computed, nextTick, provide, watch  } from "vue";
    import { useRoute, useRouter } from 'vue-router';
    import axios from 'axios';

    import KebabMenu from "../Kebab/KebabMenu.vue";
    import RenamePlaylistOverlay from "./RetitlePlaylistOverlay.vue";
    import LoadingState from "@/components/Solid/LoadingState.vue";
    import PlaylistCard from "./PlaylistCard.vue";
    import { API_BASE_URL } from "@/assets/config.js";
    import useInfiniteScroll from "@/assets/utils/useInfiniteScroll.js";
    import { after } from "lodash-es";

    const props = defineProps({
        errorMessage: {
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
        }
    });
    
    const emit = defineEmits(['load-more', 'rename', 'delete']);
    const router = useRouter();
    const currentPlaylist = ref({});
    const parentWidth = ref(0);
    const kebabMenuRef = ref(null);
    const retitlePlaylistRef = ref(null);
    const shareRef = ref(null);
    const error = ref(null);


    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const { 
        data: playlists, 
        observerTarget, 
        hasMore, 
        isLoading, 
        error: scrollError, 
        container,
        loadMore,
        reset: resetPlaylists
    } = useInfiniteScroll({
        fetchMethod: async (after) => {
            const result = await fetchMethod(after);
            // emit('load-more');
            return result;
        },
        scrollElement: props.scrollElement,
        isEnabled: props.isInfiniteScroll,
        initialLoad: true
    });

    const handleKebabClick = ({ playlist, buttonElement }) => {
        currentPlaylist.value = playlist;
        kebabMenuRef.value?.openMenu(buttonElement);
    };

    const handleRetitlePlaylist = (event) => {
        event?.stopPropagation(); // Добавьте проверку на существование event
        console.log('handleRetitlePlaylist', currentPlaylist.value.playlistTitle);
        retitlePlaylistRef.value?.toggleMenu(currentPlaylist.value.playlistTitle);
    };

    const handleDeletePlaylist = (event) => {
        event?.stopPropagation(); // Добавьте проверку на существование event
        emit('delete', currentPlaylist.value.playlistId);
    };

    const retitlePlaylist = async (playlistTitle) => {
        try {
            console.log(playlistTitle)
            console.log(currentPlaylist.value)
            await api.patch(`/api/Playlist/${currentPlaylist.value.playlistId}`,
                {
                    "title": playlistTitle,
                    "description": "плейлист"
                }
            );
            await resetPlaylists();
        } catch (err) {
            error.value = err.response?.data?.message || err.message || 'Ошибка при удалении плейлиста';
        }
    };

    const deletePlaylist = async (playlistId) => {
        try {
            await api.delete(`/api/Playlist/${playlistId}`);
            await resetPlaylists();
        } catch (err) {
            error.value = err.response?.data?.message || err.message || 'Ошибка при удалении плейлиста';
        }
    };

    const navigateToPlaylist = (playlist) => {
        router.push({
            path: `/playlist/${playlist.id}`
        });
    };

    const fetchMethod = async (after) => {
        const limit = computedBlocksInRow.value * 4;
        try {
        const response = await api.get(`/api/Playlist`, {
            params: {
            limit: limit,
            after: after || 0
            }
        });
        return {
            items: response.data,
            nextAfter: response.data[response.data.length - 1]?.id || null,
            hasMore: response.data.length === limit
        };
        } catch (error) {
        console.error('Ошибка получения рекомендаций:', error);
        if (error.response?.status === 401) {
            router.push('/login');
        }
        return { playlists: [], nextAfter: 0 };
        }
    }

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
        if (parentWidth.value < 1920) return 5;
        return 5;
    });

    const computedBlockWidth = computed(() => {
        if (props.rowLayout || props.context === "aside-recomend") return "100%";
        if (parentWidth.value < 600) return `${parentWidth.value}px`;
        if (parentWidth.value < 800) return `${Math.floor(parentWidth.value * 0.49)}px`;
        if (parentWidth.value < 1200) return `${Math.floor(parentWidth.value * 0.32)}px`;
        if (parentWidth.value < 1920) return `${Math.floor(parentWidth.value * 0.19)}px`;
        return `${Math.floor(parentWidth.value * 0.19)}px`;
    });

    onMounted(async () => {
        await nextTick();
        await adaptiveView();
        window.addEventListener('resize', adaptiveView);
        console.log("Request prop:", props.request);
    });

    onUnmounted(() => {
        window.removeEventListener('resize', adaptiveView);
    });

    watch(() => props.request, () => loadMore(true));
    watch(() => props.searchQuery, (newVal, oldVal) => {
        if (newVal !== oldVal) loadMore(true);
    }, { immediate: true });

    defineExpose({
        deletePlaylist
    });
</script>
<template>
    <KebabMenu 
        ref="kebabMenuRef"
        :context="'playlist'"
        @retitle="handleRetitlePlaylist"
        @delete="handleDeletePlaylist"
    />
    <RenamePlaylistOverlay 
        ref="retitlePlaylistRef"
        @retitle="retitlePlaylist"
    />
    <div class="container-wrapper">
        <div v-if="!isLoading && errorMessage.length > 0" class="results-grid">
            <div v-if="playlists.length === 0" class="empty-results">
                Ничего не найдено
            </div>
        </div>
        <div v-if="scrollError" class="error-state">
            {{ scrollError }}
        </div>
        <div v-else ref="container" class="container" style="width: 100%; color: aliceblue;">
            <PlaylistCard
                v-for="playlist in playlists"
                :playlist="playlist"
                :key="playlist.id"
                @click="navigateToPlaylist(playlist)"
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
    .container-wrapper{
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