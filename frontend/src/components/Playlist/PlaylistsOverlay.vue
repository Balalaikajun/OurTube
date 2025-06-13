<script setup>
    import { ref, onMounted, onBeforeUnmount, nextTick} from "vue";
    import axios from 'axios';
    import { injectFocusEngine } from '@/assets/utils/focusEngine.js';
    import PlaylistStroke from "./PlaylistStroke.vue";
    import { API_BASE_URL } from "@/assets/config.js";
    import { scroll  } from '@/assets/utils/scroll.js';

    // const props = defineProps(
    //     {
    //         videoId: {
    //             type: Number,
    //             required: true,
    //             default: 0
    //         }
    //     }
    // )

    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true, // Важно для передачи кук
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const { register, unregister } = injectFocusEngine();

    const videoId = ref(0);

    const playlists = ref([]);
    const error = ref(null);
    const isOpen = ref(false);
    const overlayContentRef = ref(null);
    const newPlaylistName = ref('');
    const isMain = ref(true);

    const toggleMenu = async (id)  => {
        isOpen.value = !isOpen.value;
        isMain.value = true;
        newPlaylistName.value = '';
        videoId.value = id;
        console.log(videoId.value, "инициализация работы с видео через меню")
        await nextTick(); 
        if(isOpen.value)
        {
            document.addEventListener('click', handleClickOutside);
        }
        else
        {
            document.removeEventListener('click', handleClickOutside);
        }
    }

    const handleFocus = () => {
        register('createOverlay');
    };

    const handleBlur = () => {
        setTimeout(() => {
            if (!document.activeElement?.closest('.overlay-content')) {
                unregister('createOverlay');
            }
        }, 100);        
    };

    const handleClickOutside = (event) => {
        console.log("handleClickOutside")
        if (overlayContentRef.value && !overlayContentRef.value.contains(event.target)) {
            toggleMenu();
        }
    };

    const addToPlaylist = async (playlistId, isContained) => {
        if(isContained)
        {
            const response = await api.post(`/api/Playlist/${playlistId}/${videoId.value}`);
            console.log("Добавление в плейлист")
        }
        else
        {
            const response = await api.delete(`/api/Playlist/${playlistId}/${videoId.value}`);
            console.log("Удаление из плейлиста")
        }

    };

    const createNewPlaylist = async () => {
        if (!isMain.value && newPlaylistName.value.trim()) {
            try {
                const response = await api.post('/api/Playlist', {
                    title: newPlaylistName.value.trim(),
                    description: "плейлист"
                });
                console.log(response.data.id)
                if (response.data.id) {
                    await api.post(`/api/Playlist/${response.data.id}/${props.videoId}`);
                    await fetchPlaylists();
                }
                
                isMain.value = true;
                newPlaylistName.value = '';
            } catch (err) {
                error.value = err.response?.data?.message || 
                            err.message || 
                            'Ошибка при создании плейлиста';
                console.error("Ошибка при создании плейлиста:", err);
            }
        } else {
            isMain.value = false;
            await nextTick();
            // Фокусируем textarea после переключения в режим создания
            const textarea = document.querySelector('.playlist-title textarea');
            if (textarea) textarea.focus();
        }
    };

    const fetchPlaylists = async () =>
    {
        try {
            error.value = null;

            const response = await api.get(`/api/Playlist/video/${videoId.value}`);
            playlists.value = response.data;
            

        } catch (err) {
            error.value = err.response?.data?.message || 
                        err.message || 
                        'Ошибка при загрузке комментариев';
            console.error("Ошибка при загрузке комментариев:", err);
        } finally {
            error.value = null;
            console.log(playlists.value)
        }
    };

    onMounted(fetchPlaylists);

    onBeforeUnmount(toggleMenu);

    defineExpose({
        toggleMenu
    });
</script>

<template>
    <div class="overlay" v-if="isOpen">
        <div ref="overlayContentRef">
            <div class="overlay-content" v-if="isMain">
                <div class="top">
                    <h3>Плейлисты</h3>
                </div>
                
                <div class="playlist-wrapper">
                    <PlaylistStroke 
                        v-for="playlist in playlists"
                        :key="playlist.id"
                        :id="playlist.id"
                        :title="playlist.title"
                        :count="playlist.count"
                        :hasVideo="playlist.hasVideo"
                        :isContained="false"
                        @select="addToPlaylist"
                    />
                </div>

                <div class="bottom">
                    <button @click.stop="createNewPlaylist" class="control-button comment-button">
                        Новый
                    </button>
                </div>
            </div>
            <div class="overlay-content" v-if="!isMain">
                <div class="top">
                    <h3>Новый плейлист</h3>
                </div>

                <div class="playlist-title">
                    <textarea 
                        v-model="newPlaylistName"
                        @focus="handleFocus"
                        @blur="handleBlur"
                        placeholder="Введите название"
                        maxlength="100"
                        cols="1"
                        rows="1"
                        @keydown.enter="createNewPlaylist"
                    ></textarea>
                </div>

                <div class="bottom">
                    <button 
                        class="control-button comment-button"
                        :class="{ 
                            'disabled-button': !newPlaylistName.trim(), 
                            'comment-isFilled': newPlaylistName.trim() 
                        }"
                        :disabled="!newPlaylistName.trim()"
                        @click.stop="createNewPlaylist" 
                    >
                        Создать
                    </button>
                    <button @click.stop="toggleMenu" class="control-button comment-button">
                        Отмена
                    </button>
                </div>
            </div>
        </div>        
    </div>
</template>

<style scoped>
    .overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.5);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 1000;
    }

    .overlay-content {
        display: flex;
        flex-direction: column;
        color: #F3F0E9;
        box-sizing: border-box;
        /* width: 300px; */
        /* height: 450px; */
        width: min-content;
        /* min-height: 450px; */
        /* height: fit-content; */
        background: #4A4947;
        padding: 20px;
        border-radius: 4px;
    }

    .top {
        display: flex;
        justify-content: space-between;
        padding-bottom: 20px;
        box-sizing: border-box;
    }

    .bottom {
        display: flex;
        justify-content: center;
        padding-top: 20px;
        gap: 10%;
    }

    .playlist-wrapper {
        display: flex;
        min-width: 250px;
        min-height: 300px;
        flex-direction: column;
        gap: 10px;
        flex: 1;
    }

    .playlist-title {
        width: 100%;
        min-height: 60px; /* Начальная высота */
        max-height: 200px; /* Максимальная высота (если нужно ограничить) */
        border: 1px solid #F3F0E9;
        border-radius: 4px;
    }

    .playlist-title textarea {
        box-sizing: border-box;
        padding: 8px;
        overflow-y: hidden;
        width: 300px;
        min-height: 60px; /* Начальная высота */
        max-height: 200px; /* Максимальная высота (если нужно ограничить) */
        background: #252525;
        color: #F3F0E9;
        border: none;
        resize: none;
        font-family: inherit;
        font-size: inherit;
    }

    .playlist-title textarea:focus {
        outline: none;
        border-color: #F3F0E9;;
    }

    .comment-button {
        width: 100%;
        align-self: center;
        padding: 10px;
        font-size: 0.875rem;
        background-color: #252525;
    }
    .comment-button:hover{
        background-color: #100E0E;
    }

    .disabled-button:hover {
        cursor: default !important;
        background-color: #252525;
    }
    .comment-isFilled:hover {
        cursor: pointer !important;
        background-color: #F39E60;
        color: #100E0E;
    }
</style>