<script setup>
import { onMounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/assets/utils/api.js'
import MasterHead from '../components/Solid/MasterHead.vue'
import ConfirmPannel from '@/components/Solid/ConfirmPannel.vue'
import PlaylistOverlay from '@/components/Playlist/PlaylistsOverlay.vue'
import VideosPresentation from '@/components/Video/VideosPresentation.vue'

const router = useRouter();
    const route = useRoute();

    const videoId = ref(null);
    const confirmRef = ref(null);
    const confirmContext = ref("")
    const playlistRef = ref(null);
    const queryText = ref("");
    const searchQuery = ref(""); // Отдельное состояние для поискового запроса
    const isInputFocused = ref(false);

    const saveOpen = (videoId) => {
      playlistRef.value.toggleMenu(videoId);
    }

    const handleSearch = (event) => {
        // Добавляем event?.preventDefault() для дополнительной защиты
        event?.preventDefault();
        if (queryText.value.trim()) {
                searchQuery.value = queryText.value.trim();
                router.push({ 
                path: '/history',
                query: { q: queryText.value.trim() } 
            });
        } else {
            clearSearch();
        }
    };

    const clearSearch = () => {
        queryText.value = "";
        searchQuery.value = "";
        // router.push({ path: '/history' });
    };

    const handleDeleteHistory = () => {
        confirmContext.value = "Удаление истории";
        confirmRef.value.openMenu();
    };
    const handleDeleteFromHistory = async (videoId) => {
        try {
            await api.delete(`History/${videoId}`);
            console.log(videoId)
        } catch (error) {
            console.error('History error:', error);
        }
        finally {
            clearSearch();
        }
    };

    const clearHistory = async () => {
        try {
            await api.delete('History');
            console.log('Clear history');
        } catch (error) {
            console.error('History error:', error);
        }
        finally {
            clearSearch();
        }
    }

    watch(() => route.query.q, (newQuery) => {
        if (newQuery !== undefined && newQuery !== queryText.value) {
            queryText.value = newQuery;
            searchQuery.value = newQuery;
        }
    }, { immediate: true });
    onMounted(async () => {
        // document.title = "История просмотра";
    });
</script>
<template>
    <MasterHead />
    <PlaylistOverlay ref="playlistRef" 
    />
    <ConfirmPannel 
        ref="confirmRef" 
        :action="confirmContext"
        @confirm="clearHistory"
    />
    <main class="main-wrapper">
        <h1>
            История просмотра
        </h1>
        <div class="columns-wrapper">
            <div style="color: #F3F0E9; width: 100%;" v-auth="false">
                История просмотра недоступна. Войдите в учётную запись.
            </div>
            <VideosPresentation
                request="history"
                context="aside-recomend"
                :search-query="searchQuery"
                :is-infinite-scroll="true"
                :row-layout=true
                @add-to-playlist="saveOpen"
                @delete="handleDeleteFromHistory"
            />
            <aside class="history-functional">
                <div class="history-menu">
                    <div class="enter-query">
                        <div class="enter-line">
                            <button class="control-button enter-query-btn" @click="handleSearch">
                                <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40">
                                    <!-- Определяем маску для инвертированного вырезания -->
                                    <defs>
                                        <mask id="circleMask">
                                            <rect x="0" y="0" width="40" height="40" fill="white"/>
                                            <circle cx="19" cy="19" r="11" fill="black"/>
                                        </mask>
                                    </defs>

                                    <circle cx="19" cy="19" r="10" stroke="#F3F0E9" stroke-width="1" fill="none"/>
                                    
                                    <path 
                                        d="M 19 19 L 31 31" 
                                        stroke="#F3F0E9"
                                        style="stroke-width: 1px !important;"
                                        mask="url(#circleMask)"
                                        fill="none"
                                    />
                                </svg>
                            </button>
                            <textarea
                                ref="textareaRef"
                                class="component-input"    
                                placeholder="Поиск по истории" 
                                v-model="queryText"
                                @focus="isInputFocused = true"
                                @blur="isInputFocused = false"
                                @keydown.enter.prevent="handleSearch"
                                rows="1"
                                wrap="off"
                            ></textarea>
                            <button v-if="queryText" class="control-button enter-query-btn" @click="clearSearch">
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
                        </div>
                        <div class="line-container">
                            <span class="line-decoration" :class="{'active': isInputFocused}"></span>
                            <span style="position: absolute; width: 100%; height: 1px; background-color: #F3F0E9;"></span>
                        </div>
                    </div>
                    <button class="control-button delete-btn" @click.stop="handleDeleteHistory">
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
                        Очистить историю
                    </button>
                </div>            
            </aside>
        </div>
    </main>

        <!-- :search-query="searchQuery" -->
</template>
<style scoped>
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
        gap: 10vw;
    }
    .history-functional {
        position: relative;
        width: 30%;
        box-sizing: border-box;
        padding: 0 30px 0 30px;
    }
    .history-menu {
        display: flex;
        flex-direction: column;
        position: sticky;
        top: 90px;
        gap: 2vh;
    }
    .enter-query {
        display: flex;
        flex-direction: column;
    }
    .enter-line {
        display: flex;
        flex-direction: row;
        align-items: center;
    }
    .enter-query-btn {
        width: 40px;
        height: 40px;
    }
    .enter-query-btn:hover {
        background-color: #4A4947;
    }
    .component-input {
        width: 100%;
        opacity: 0.7;
        min-height: 15px;
        color: #F3F0E9;
        line-height: 15px; 
        font-size: 14px;
        outline: none;
        resize: none; /* Запрещаем изменение размера */
        box-sizing: border-box;
        background: transparent;
        border: none;
        white-space: nowrap; /* Запрещаем перенос строк */
        overflow-x: hidden; /* Горизонтальный скролл при переполнении */
        overflow-y: hidden; /* Скрываем вертикальный скролл */
    }
    .line-container {
        position: relative;
        width: 100%;
        height: 2px; /* Фиксированная высота контейнера */
    }

    .line-decoration {
        position: absolute;
        left: 0;
        right: 0;
        height: 2px;
        background-color: #F3F0E9;
        transform: scaleX(0);
        transform-origin: center;
        transition: transform 0.3s ease;
    }

    .line-decoration.active {
        width: 100%;
        left: 0;
        transform: none;
    }

    .delete-btn {
        width: 100%;
        height: 40px;
        border: 1px solid #F3F0E9;
        box-sizing: border-box;
    }
    .delete-btn:hover {
        border: 1px solid #4A4947;
        background-color: #4A4947;
    }
    .delete-btn svg {
        position: absolute;
        left: 0;
    }

    .component-input:focus {
        opacity: 1;
    }
    .component-input:focus::-webkit-input-placeholder { /* Chrome/Opera/Safari */
        opacity: 0;
    }
    .component-input::-webkit-input-placeholder { /* Chrome/Opera/Safari */
        display: -webkit-box;
        -webkit-line-clamp: 1;
        -webkit-box-orient: horizontal;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: normal;
        /* min-height: 17px; */
    }
    .component-input:focus::-moz-placeholder { /* Firefox 19+ */
        opacity: 0;
    }
    .component-input:focus:-ms-input-placeholder { /* IE 10+ */
        opacity: 0;
    }
    .component-input:focus:-moz-placeholder { /* Firefox 18- */
        opacity: 0;
    }
    @media (max-width: 1000px) {
        .delete-btn svg {
            display: none;
        }
        .columns-wrapper {
            gap: 1vw;
        }
        .history-functional {
            padding-right: 0;
        }
    }
</style>