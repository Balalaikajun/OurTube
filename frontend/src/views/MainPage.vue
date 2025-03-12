<script setup>
    import { ref, onMounted } from "vue";
    import { useRouter } from "vue-router";
    import VideoCard from "../components/VideoBlockCard.vue"; // Импортируем компонент VideoCard

    const router = useRouter();
    const videos = ref([]); // Массив для хранения данных о видео
    const loading = ref(false); // Состояние загрузки
    const errorMessage = ref(""); // Сообщение об ошибке

// Функция для загрузки данных
    const fetchVideos = async () => {
        loading.value = true;
        errorMessage.value = "";

        try {
            // Запрос к API для получения списка видео
            const response = await fetch(`${API_BASE_URL}/api/Recommendation?limit=10&after=0`);
            const data = await response.json();
            
            if (!response.ok) {
            throw new Error(data.message || "Ошибка при загрузке видео");
            }

            videos.value = [...data]; // Сохраняем данные в массив
            // console.log(videos[0]);
        } catch (error) {
            errorMessage.value = error.message;
            console.log("Нот гуд");
        } finally {
            loading.value = false;
        }
    };

    // Функция для перехода на страницу видео
    const navigateToVideo = (videoId) => {
        router.push(`/video/${videoId}`);
    };

    // Загружаем данные при монтировании компонента
    onMounted(async () => {
        await fetchVideos();
        console.log(videos);
    });
</script>

<template>
    <div class="video-list">
        <!-- Сообщение об ошибке -->
        <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

        <!-- Состояние загрузки -->
        <div v-if="loading" class="loading">Загрузка...</div>

        <!-- Список видео -->
        <div v-else class="videos">
            <VideoCard
                v-for="video in videos"
                :key="video.id"
                :video="video"
                @click="navigateToVideo(video.id)"
            />
        </div>
    </div>
</template>

<style scoped>
    .video-list {
        padding: 20px;
    }

    .error {
        color: red;
        text-align: center;
        margin-top: 10px;
    }

    .loading {
        text-align: center;
        font-size: 18px;
        color: #F3F0E9;
    }

    .videos {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
        gap: 20px;
    }
</style>