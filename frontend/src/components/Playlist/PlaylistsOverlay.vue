<script setup>
    import { ref, onMounted, onBeforeUnmount, nextTick} from "vue";
    import axios from 'axios';
    import PlaylistStroke from "./PlaylistStroke.vue";
    import { API_BASE_URL } from "@/assets/config.js";
    import { scroll  } from '@/assets/utils/scroll.js';

    const props = defineProps(
        {
            videoId: {
                type: Number,
                required: true,
                default: 0
            }
        }
    )

    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true, // Важно для передачи кук
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const playlists = ref([]);

    const error = ref(null);

    const isOpen = ref(false);

    const overlayContentRef = ref(null);

    // const emit = defineEmits(["close"]);


    const toggleMenu = async ()  => {
        isOpen.value = !isOpen.value;
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

    const handleClickOutside = (event) => {
        if (overlayContentRef.value && !overlayContentRef.value.contains(event.target)) {
            toggleMenu();
        }
    };

    const addToPlaylist = async (playlistId, isContained) => {
        if(isContained )
        {
            const response = await api.post(`/api/Playlist/${playlistId}/${props.videoId}`);
            console.log("Добавление в плейлист")
        }
        else
        {
            const response = await api.delete(`/api/Playlist/${playlistId}/${props.videoId}`);
            console.log("Удаление из плейлиста")
        }

    };

    const createNewPlaylist = () => {
        const newPlaylistName = prompt("Введите название нового плейлиста");
        if (newPlaylistName) {
            playlists.value.push({ id: playlists.value.length + 1, name: newPlaylistName });
        }
    };

    const fetchPlaylists = async () =>
    {
        try {
            error.value = null;

            const response = await api.get(`/api/Playlist`);
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
        <div ref="overlayContentRef" class="overlay-content">
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
                    :isContained="false"
                    @select="addToPlaylist"
                />
            </div>
            <!-- <button @click="createNewPlaylist">Создать новый плейлист</button>
            <button @click="$emit('close')">Закрыть</button> -->
            <div class="bottom">
                <button class="control-button comment-button">
                    Новый
                </button>
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
        width: 300px;
        height: 450px;
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
    }

    .playlist-wrapper {
        display: flex;
        flex-direction: column;
        gap: 10px;
        flex: 1;
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
</style>