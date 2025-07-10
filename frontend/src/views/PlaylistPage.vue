<script setup>
    import { onMounted, ref, watch } from 'vue'
    import { useRoute, useRouter } from 'vue-router'
    import api from '@/assets/utils/api.js'

    import MasterHead from '../components/Solid/MasterHead.vue'
    import PlaylistOverlay from '@/components/Playlist/PlaylistsOverlay.vue'
    import RenamePlaylistOverlay from '@/components/Playlist/RetitlePlaylistOverlay.vue'
    import ConfirmPannel from '@/components/Solid/ConfirmPannel.vue'
    import LoadingState from '@/components/Solid/LoadingState.vue'
    import VideosPresentation from '@/components/Video/VideosPresentation.vue'

    const props = defineProps(
        {
            id: String,
            title: String,
            count: Number
        }

    );

    const route = useRoute()
    const router = useRouter()

    const playlistData = ref({
        id: props.id || route.params.id || '', // Берем id из props или route
        title: props.title || 'Плейлист',
        count: props.count || 0
    })
    const playlistPresentation = ref(null);
    const currentPlaylistId = ref(null);
    const playlistRef = ref(null);
    const confirmRef = ref(null);
    const confirmContext = ref("");
    const retitlePlaylistRef = ref(null);
    const isLoading = ref(true);
    const videosRef = ref(null);
    const loadingMore = ref(false);
    const errorMessage = ref("");
    const currentVideoId = ref("");
    const kebabMenuRef = ref(null);
    const shareRef = ref(null);
    const nextAfter = ref(0);
    const hasMore = ref(true);
    const isMobile = ref(false);
    const videosPlace = ref(null);

    const setDocumentTitle = (title) => {
        document.title = `${title} | OurTube`
    }

    const fetchPlaylistData = async () => {
        isLoading.value = true
        errorMessage.value = null
        
        // Проверяем, есть ли данные в state навигации
        try {
            // Если есть props - обновляем данные
            if (props.id || props.title || props.count) {
                playlistData.value = {
                id: props.id || playlistData.value.id,
                title: props.title || playlistData.value.title,
                count: props.count || playlistData.value.count
                }
                
                if (props.title) {
                    setDocumentTitle(props.title)
                }
            }
            
            // Всегда запрашиваем свежие данные с сервера
            const response = await api.get(`api/Playlist/${playlistData.value.id}`)
            if (response.data) {
            Object.assign(playlistData.value, response.data)
            setDocumentTitle(playlistData.value.title)
            }
        } catch (err) {
            errorMessage.value = err.response?.data?.message || err.message || 'Ошибка загрузки'
            console.error('Ошибка:', err)
            setDocumentTitle('Плейлист')
        } finally {
            isLoading.value = false
        }
    }

    const fetchPlaylistPresentation = async () => {
        isLoading.value = true
        errorMessage.value = null
        
        // Проверяем, есть ли данные в state навигации
        console.log(router.currentRoute.value.state?.playlistData)
        if (router.currentRoute.value.state?.playlistData) {
            playlistData.value = router.currentRoute.value.state.playlistData
        }
        
        try {
            const response = await api.get(`api/Playlist/${currentPlaylistId.value}`)
            playlistData.value = response.data
        } catch (err) {
            errorMessage.value = err.response?.data?.message || err.message || 'Ошибка загрузки'
            console.error('Ошибка:', err)
        } finally {
            isLoading.value = false
        }
    }

    const saveOpen = (videoId) => {
        console.log("save")
        playlistRef.value.toggleMenu(videoId);
    }

    const handleRetitlePlaylist = (event) => {
        event?.stopPropagation(); // Добавьте проверку на существование event
        console.log('handleRetitlePlaylist', playlistData.value);
        retitlePlaylistRef.value?.toggleMenu(playlistData.value.title);
    };

    const handleDeletePlaylist = () => {
        confirmContext.value = "Удаление плейлиста";
        confirmRef.value.openMenu();
    };

    const retitlePlaylist = async (playlist) => {
        try {
            console.log(playlist)
            await api.patch(`api/Playlist/${currentPlaylistId.value}`,
                {
                    "title": playlist,
                    "description": "плейлист"
                }
            );
            playlistData.value.title = playlist;
        } catch (err) {
            errorMessage.value = err.response?.data?.message || err.message || 'Ошибка при удалении плейлиста';
        }        
    };

    const deletePlaylist = async () => {
        try {
            await api.delete(`api/Playlist/${currentPlaylistId.value}`);
            await videosRef.value.resetPlaylist();
        } catch (err) {
            error.value = err.response?.data?.message || err.message || 'Ошибка при удалении плейлиста';
        }
    };

    const handleDeleteFromPlaylist = async (videoId) => {
        try {
            console.log("Удаление", videoId, "из", currentPlaylistId.value )
            await api.delete(`api/Playlist/${currentPlaylistId.value}/${videoId}`);
            await videosRef.value.resetPlaylist();
        } catch (error) {
            console.error('Playlist error:', error);
        }
    };

    watch(
        () => route.params.id,
        (newId) => {
            if (newId && newId !== playlistData.value.id) {
            playlistData.value.id = newId
            fetchPlaylistData()
            }
        },
        { immediate: true }
    )

    onMounted(async () => {
        currentPlaylistId.value = route.params.id;
        console.log(currentPlaylistId.value, props.id, props.title, props.count)
        await fetchPlaylistData()
    })
</script>
<template>
    <!-- <MasterHead/> -->
    <PlaylistOverlay 
        ref="playlistRef" 
    />
    <ConfirmPannel 
        ref="confirmRef" 
        :action="confirmContext"
        @confirm="deletePlaylist"
    />
    <RenamePlaylistOverlay 
        ref="retitlePlaylistRef"
        @retitle="retitlePlaylist"
    />
    <main class="main-wrapper">
        <LoadingState v-if="isLoading" />
    
        <div v-else-if="errorMessage" class="error-message">
            {{ errorMessage }}
        </div>
        <div class="columns-wrapper">
            <aside class="playlist-functional">
                <div class="playlist-menu">
                    <div class="playlist-data" v-if="playlistData">
                        <div class="filler-block">
                            <p>{{ playlistData.count }}</p>
                        </div>
                        <p>{{ playlistData.title }}</p>
                    </div>
                    <div class="bottom-block">
                        <button class="control-button short-action-btn" @click="handleDeletePlaylist">
                            <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"> 
                                <path 
                                    d="M 10 10 L 30 30" 
                                    stroke="#F3F0E9"
                                    style="stroke-width: 1px !important;"
                                    fill="none"
                                />
                                <path 
                                    d="M 10 30 L 30 10" 
                                    stroke="#F3F0E9"
                                    style="stroke-width: 1px !important;"
                                    fill="none"
                                />
                            </svg>
                        </button>
                        <button class="control-button short-action-btn" @click="handleRetitlePlaylist">
                            <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"> 
                                <path 
                                    d="M 17 3 L 23 3 L 20 35 Z"
                                    transform="rotate(45 20 20)"
                                    stroke="#F3F0E9"
                                    style="stroke-width: 1px !important;"
                                    fill="none"
                                />
                            </svg>
                        </button>
                    </div>

                </div>            
            </aside>
            <VideosPresentation
                ref="videosRef"
                request="playlist"
                context="aside-recomend"
                @add-to-playlist="saveOpen"
                @delete="handleDeleteFromPlaylist"
                :is-infinite-scroll="true"
                :row-layout=true
            />
        </div>
    </main>
</template>
<style scoped>
    .error-message {
        color: #ff4d4f;
        text-align: center;
        padding: 40px 0;
    }
    .main-wrapper {
        display: flex;
        flex-direction: column;
        gap: 5vh;
        box-sizing: border-box;
        width: 100%;  
        margin-top: 70px;
        padding: 20px 100px;
    }
    .main-wrapper h1 {
        -webkit-text-stroke: 0.3px currentColor;
    }
    .columns-wrapper {
        display: flex;
        flex-direction: row;
        justify-content: flex-start;
        gap: 2vw;
    }
    .playlist-functional {
        position: relative;
        width: 30%;
        box-sizing: border-box;
    }
    .playlist-menu {
        display: flex;
        flex-direction: column;
        position: sticky;
        top: 90px;
        gap: 1vh;
    }
    .playlist-data {
        width: 100%;
    }
    .playlist-data p:last-child {
        color: #F3F0E9;
        margin-top: 10px; /* Добавьте отступ сверху */
        text-indent: 10px;
        font-size: 1.4rem;
    }
    .filler-block {
        display: flex;
        flex-direction: row;
        justify-content: center; /* Центрирование по горизонтали */
        align-items: center;     /* Центрирование по вертикали */
        position: relative;
        width: 100%;
        height: auto;
        overflow: hidden;
        aspect-ratio: 16/9;
        background: #f39e60;
    }
    .filler-block p:first-child {
        color: #100E0E;
        font-size: 2rem;
        -webkit-text-stroke: 3px currentColor;
    }

    .bottom-block {
        display: flex;
        justify-content: end;
        width: 100%;
        line-height: 1.4;
        gap: 1vw;
    }

    .short-action-btn {
        width: min-content;
        height: min-content;
        border-radius: 50%;
    }
    .short-action-btn:hover {
        background-color: #4A4947;
    }
    .short-action-btn svg {
        position: relative;
    }

    /* @media (max-width: 1000px) {
        .delete-btn svg {
            display: none;
        }
        .columns-wrapper {
            gap: 1vw;
        }
        .history-functional {
            padding-right: 0;
        }
    } */
</style>