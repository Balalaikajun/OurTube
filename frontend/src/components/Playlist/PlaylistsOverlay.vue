<script setup>
    import { nextTick, ref } from 'vue'
    import api from '@/assets/utils/api.js'
    import { createKeyboardTrap } from '@/assets/utils/keyTrap.js'
    import PlaylistStroke from './PlaylistStroke.vue'


    const videoId = ref(0);

    const playlists = ref([]);
    const error = ref(null);
    const isOpen = ref(false);
    const overlayContentRef = ref(null);
    const newPlaylistName = ref('');
    const isMain = ref(true);

    const keyboardTrap = createKeyboardTrap(overlayContentRef);

    const toggleMenu = async (id) => {
        videoId.value = id;
        await nextTick();
        isOpen.value = !isOpen.value;
        isMain.value = true;
        newPlaylistName.value = '';

        if (isOpen.value) {
            await fetchPlaylists();
            document.addEventListener('click', handleClickOutside);
            keyboardTrap.setup(); // Активируем ловушку клавиатуры
        } else {
            document.removeEventListener('click', handleClickOutside);
            keyboardTrap.teardown(); // Деактивируем ловушку клавиатуры
        }
    };

    const handleClickOutside = (event) => {
        // console.log("handleClickOutside")
        if (overlayContentRef.value && !overlayContentRef.value.contains(event.target)) {
            toggleMenu();
        }
    };

    const addToPlaylist = async (playlistId, isContained) => {
        if(isContained)
        {
            const response = await api.post(`api/Playlist/${playlistId}/${videoId.value}`);
            // console.log("Добавление в плейлист")
        }
        else
        {
            const response = await api.delete(`api/Playlist/${playlistId}/${videoId.value}`);
            // console.log("Удаление из плейлиста")
        }

    };

    const createNewPlaylist = async () => {
        if (!isMain.value && newPlaylistName.value.trim()) {
            try {
                const response = await api.post('api/Playlist', {
                    title: newPlaylistName.value.trim(),
                    description: "плейлист"
                });
                // console.log(response.data.id)
                if (response.data.id) {
                    await api.post(`api/Playlist/${response.data.id}/${videoId.value}`);
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

            // console.log(videoId.value)
            const response = await api.get(`api/Playlist/video/${videoId.value}`);
            playlists.value = response.data;
            

        } catch (err) {
            error.value = err.response?.data?.message || 
                        err.message || 
                        'Ошибка при загрузке плейлистов';
            console.error("Ошибка при загрузке плейлистов:", err);
        } finally {
            error.value = null;
            // console.log(playlists.value)
        }
    };

    // onBeforeUnmount(toggleMenu);

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
                    <button 
                        @click.stop="createNewPlaylist" 
                        class="reusable-button"
                    >
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
                        class="standart-input"
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
                        class="reusable-button"
                        :class="{ 
                            'disabled': !newPlaylistName.trim(), 
                            'isFilled': newPlaylistName.trim() 
                        }"
                        :disabled="!newPlaylistName.trim()"
                        @click.stop="createNewPlaylist" 
                    >
                        Создать
                    </button>
                    <button 
                        @click.stop="toggleMenu" 
                        class="reusable-button"
                    >
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
        min-height: 200px;
        max-height: 300px;
        flex-direction: column;
        gap: 10px;
        flex: 1;
        overflow-y: auto; /* Добавляем вертикальный скролл */
        scrollbar-width: thin; /* Для Firefox */
        scrollbar-color: #F39E60 #4A4947; /* Для Firefox */
        padding-right: 5px; /* Чтобы контент не прилипал к скроллу */
        scrollbar-width: thin; /* или 'auto' или 'none' */
        scrollbar-color: #F39E60 #4A4947;
    }
    .playlist-wrapper::-webkit-scrollbar {
        width: 8px; /* Ширина скроллбара */
        height: 8px; /* Высота горизонтального скроллбара (если нужен) */
    }
    /* .playlist-wrapper::-webkit-scrollbar-track {
        background: #4A4947;
        border-radius: 4px;
    } */

    .playlist-wrapper::-webkit-scrollbar-thumb {
        background-color: #F39E60;
        border-radius: 4px;
        border: 2px solid #4A4947; /* Создает эффект "отступа" */
    }

    .playlist-wrapper::-webkit-scrollbar-thumb:active,
    .playlist-wrapper::-webkit-scrollbar-thumb:hover,
    .playlist-wrapper::-webkit-scrollbar-thumb:focus {
        background-color: #F39E60 !important;
        background: #F39E60 !important; /* Оставляем тот же цвет */
    }
    .playlist-wrapper {
        scrollbar-color: #F39E60 transparent !important;
        -moz-scrollbar-color: #F39E60 !important;
    }

    .playlist-title {
        width: 100%;
        min-height: fit-content; /* Начальная высота */
        max-height: 200px; /* Максимальная высота (если нужно ограничить) */
        border: 1px solid #F3F0E9;
        border-radius: 4px;
    }
</style>