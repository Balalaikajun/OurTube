<script setup>
    import { ref, onMounted, onUnmounted, computed, nextTick, watch  } from "vue";
    import { useRoute, useRouter } from 'vue-router';
    import VideoCard from "../components/VideoCard.vue";
    import KebabMenu from "../components/KebabMenu.vue";
    import ShareOverlay from "../components/ShareOverlay.vue";
    import LoadingState from "@/components/LoadingState.vue";
    import { API_BASE_URL } from "@/assets/config.js";

    const props = defineProps({
        errorMessage: {
            type: String,
            default: ""
        },
        context: {
            type: String,
            required: true,
            validator: (value) => [
            'recomend', 
            'aside-recomend',
            'search'
            ].includes(value)
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
            default: 'window' // или 'parent' или CSS-селектор
        },          
        rowLayout: {
            type: Boolean,
            default: false
        }
    });

    const videos = ref([]);
    const router = useRouter(); // Получаем роутер
    const scrollElement = ref(null);
    const container = ref(null);
    const hasMore = ref(true);
    const nextAfter = ref(0);
    const loading = ref(false);
    const errorMessage = ref("");
    const currentVideoId = ref("");
    const parentWidth = ref(0);
    const parentHeight = ref(0);
    const kebabMenuRef = ref(null);
    const shareRef = ref(null);
    const isInitialized = ref(false);
    const testWidth = ref(1089);

    const emit = defineEmits(['load-more']);

    const handleScroll = () => {
        if (!props.isInfiniteScroll) return;

        const element = getScrollElement();
        if (!element) return;

        const { scrollTop, scrollHeight, clientHeight } = element;
        const isNearBottom = scrollTop + clientHeight >= scrollHeight - 500;

        if (isNearBottom) {
            loadMoreVideos();
        }
    };

    const loadMoreVideos = async () => {
        await fetchVideos(false);
        emit('load-more');
    };

    const getScrollElement = () => {
        if (props.scrollElement === 'window') return document.documentElement;
        if (props.scrollElement === 'parent') return container.value?.parentElement;
        return document.querySelector(props.scrollElement);
    };

    const handleKebabClick = ({ videoId, buttonElement }) => {
        currentVideoId.value = videoId;
        kebabMenuRef.value?.openMenu(buttonElement);
    };

    const handleKebabClose = () => {
        if (!shareRef.value?.isOpen) {
            currentVideoId.value = '';
        }
    };

    const handleShareClick = () => {
        if (shareRef.value?.openMenu) {
            shareRef.value.openMenu();
        }
    };

    const navigateToVideo = (videoId) => {
        console.log('tovideo')
        router.push(`/video/${videoId}`);
    };

    const adaptiveView = () => {
        return new Promise((resolve) => {
            if (!container.value) return resolve();
            
            nextTick(() => {
            const rect = container.value.getBoundingClientRect();
            parentWidth.value = rect.width - 20;
            console.log('Ширина родителя:', parentWidth.value);
            
            if (isNaN(parentWidth.value)) {
                console.error("Не удалось вычислить ширину родителя");
                return resolve();
            }

            const gap = blocksInRow.value > 1 
                ? Math.floor(Math.max(0, (parentWidth.value - (parseFloat(blockWidth.value) * blocksInRow.value))) / (blocksInRow.value - 1))
                : 0;
            
            container.value.style.gap = `30px ${Math.floor(gap)}px`;
            console.log('Gap установлен:', container.value.style.gap);
            resolve();
            });
        });
    };

    const blocksInRow = computed(() => {
        const widthParent = parentWidth.value;
        console.log(widthParent, 'ширина родителя в blocksInRow')
        if (widthParent < 600 || props.rowLayout) return 1;
        if (widthParent < 800) return 2;
        if (widthParent < 1200) return 3;
        if (widthParent < 1920) return 4;
        return 5;
    });

    const blockWidth = computed(() => {
        const widthParent = parentWidth.value;
        // const widthParent = testWidth.value;
        console.log(widthParent, 'ширина родителя в blockWidth', "\n",props.rowLayout, "стройчный вид")
        if (!widthParent || widthParent <= 0 || props.rowLayout) return "100%"; // Запасной вариант

        if (widthParent < 600)      {console.log(widthParent, 'ширина блока в blockWidth');                                 return `${widthParent}px`};
        if (widthParent < 800)      {console.log(Math.floor(widthParent * 0.49), 'ширина блока в blockWidth');              return `${Math.floor(widthParent * 0.49)}px`};
        if (widthParent < 1200)     {console.log(Math.floor(widthParent * 0.32), 'ширина блока в blockWidth');              return `${Math.floor(widthParent * 0.32)}px`};
        if (widthParent < 1920)     {console.log(Math.floor(widthParent * 0.24), 'ширина блока в blockWidth');              return `${Math.floor(widthParent * 0.24)}px`};
        if (widthParent >= 1920)    {console.log(Math.floor(widthParent * 0.19), 'ширина блока в blockWidth');              return `${Math.floor(widthParent * 0.19)}px`};
        
        return "200px"; // Запасной вариант
    });

    const fetchMethods = {
        async recomend() {
            if (!isInitialized.value) {
            await new Promise(resolve => setTimeout(resolve, 100));
            }
            
            const limit = blocksInRow.value * 8;
            console.log('Запрос рекомендаций с параметрами:', { limit, after: nextAfter.value });
            
            try {
            const response = await fetch(`${API_BASE_URL}/api/Recommendation?limit=${limit}&after=${nextAfter.value}`);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            const data = await response.json();
            console.log('Получены данные:', data);
            return data;
            } catch (error) {
            console.error('Ошибка получения рекомендаций:', error);
            return [];
            }
        },
  
        async search() {
    // Если компонент ещё не инициализирован, ждём
            if (!isInitialized.value) {
                await new Promise(resolve => setTimeout(resolve, 100));
            }

            // Проверка пустого запроса
            if (!props.searchQuery.trim()) {
                console.log('Пустой поисковый запрос');
                return [];
            }

            try {
                // Формируем URL с кодированием параметров
                // const url = new URL(`${API_BASE_URL}/api/Search`);
                // url.searchParams.append('query', encodeURIComponent(props.searchQuery));
                // url.searchParams.append('after', nextAfter.value.toString());

                console.log('Выполняем поиск с параметрами:', {
                    query: props.searchQuery,
                    after: nextAfter.value
                });

                const response = await fetch(`${API_BASE_URL}/api/Search?query=${encodeURIComponent(props.searchQuery,)}`);
                // const response = await fetch(url.toString(), {
                //     headers: {
                //         'Accept': 'application/json'
                //     }
                // });

                if (!response.ok) {
                    const errorData = await response.json().catch(() => null);
                    console.error('Ошибка поиска:', {
                        status: response.status,
                        statusText: response.statusText,
                        errorData
                    });
                    throw new Error(`Ошибка ${response.status}: ${response.statusText}`);
                }

                const data = await response.json();
                console.log('Получены результаты поиска:', data);

                // Проверяем формат ответа
                if (!Array.isArray(data)) {
                    console.error("Некорректный формат ответа, ожидался массив:", data);
                    return [];
                }

                return data;
            } 
            catch (error) {
                console.error('Ошибка при выполнении поиска:', error);
                errorMessage.value = `Ошибка поиска: ${error.message}`;
                return [];
            }
        }
    };

    const fetchVideos = async (initial = false) => {
        if (loading.value || (!initial && !hasMore.value)) return;
        
        try {
            loading.value = true;
            if (initial) {
                videos.value = [];
                hasMore.value = true;
            }
            
            const data = await fetchMethods[props.context]();
            console.log(data)

            if (!Array.isArray(data) && !Array.isArray(data.videos)) {
                throw new Error('API вернул не массив видео');
            }
            if(props.context === "search") videos.value = [...videos.value, ...data];   //костыль
            else videos.value = [...videos.value, ...data.videos];                      
            console.log(videos.value)
            nextAfter.value = data.nextAfter || 0;
            hasMore.value = data.length > 0;
        } 
        catch (error) {
            errorMessage.value = error.message;
            hasMore.value = false;
        } 
        finally {
            loading.value = false;
        }
    };

    // 1. Первая загрузка
    onMounted(async () => {
        try {
            // 1. Инициализация DOM элементов
            scrollElement.value = getScrollElement();
            if (scrollElement.value) {
            scrollElement.value.addEventListener('scroll', handleScroll);
            }
            
            // 2. Ожидаем полный рендеринг
            await nextTick();
            
            // 3. Настройка адаптивного вида
            await adaptiveView();
            
            // 4. Загрузка данных
            await fetchVideos(true);
            
            // 5. Финальная настройка после загрузки
            await adaptiveView();
            
            isInitialized.value = true;
        } 
        catch (error) {
            console.error('Ошибка инициализации:', error);
            errorMessage.value = 'Ошибка загрузки компонента';
        }
    });

    onUnmounted(() => {
        if (scrollElement.value) {
            scrollElement.value.removeEventListener('scroll', handleScroll);
        }        
        window.removeEventListener('resize', adaptiveView);
    });

    watch(() => props.context, () => fetchVideos(true));

    watch(() => props.searchQuery, (newVal, oldVal) => {
        if (newVal !== oldVal) {
            fetchVideos(true);
        }
    }, { immediate: true });
</script>

<template>
    <KebabMenu 
        ref="kebabMenuRef" 
        @share="handleShareClick"
        @close="handleKebabClose"
    />
    <ShareOverlay
        ref="shareRef" 
        :videoId="currentVideoId"
    />
    <div class="container-wrapper" :class="[rowLayout && context == 'recomend' ? 'aside-recomend' : `context-${context}`, { 'row-layout': rowLayout }]">
        <div v-if="!loading && errorMessage.length > 0" class="results-grid">
            <div v-if="videos.length === 0" class="empty-results">
                Ничего не найдено
            </div>
        </div>
        <div v-if="errorMessage" class="error-state">
            {{ errorMessage }}
        </div>
        <div v-else ref="container" class="container" style="width: 100%;">

            <VideoCard
                v-for="video in videos"
                :video="video"
                :key="video.id"
                :row-layout="rowLayout"
                @click="navigateToVideo(video.id)"
                @kebab-click="handleKebabClick"
                :style="{ width: blockWidth }"
            />
            <LoadingState v-if="loading"/>
        </div>
    </div>
</template>

<style scoped>
    .container-wrapper {
        display: flex;
        flex-wrap: wrap;
        box-sizing: border-box !important;
    }
    .container-wrapper.context-recomend {
        width: 100%;
        padding: 20px 100px;
        margin-top: 70px;
    }
    .container-wrapper.aside-recomend {
        width: 100%;
        padding: 0p;
    }
    .container-wrapper.context-search {
        width: 100%;
        padding: 20px 100px;
        margin-top: 70px;
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
</style>