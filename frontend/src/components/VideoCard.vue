<script setup>
    import { ref } from "vue";
    import KebabButton from "../components/KebabButton.vue";
    import { MINIO_BASE_URL } from "@/assets/config.js";

    const props = defineProps({
    video: {
        type: Object,
        required: true,
    },
    });

    const buttonRef = ref(null);
    const kebabMenuRef = ref(null);

    const handleKebabMenuClick = () => {
    if (kebabMenuRef.value) {
        kebabMenuRef.value.toggleMenu(); // Вызываем toggleMenu из KebabMenu
    }
    };
    const handleImageError = (event) => {
        // Установите fallback изображение
        event.target.src = '/path/to/default-thumbnail.jpg';
        // Или скройте изображение
        // event.target.style.display = 'none';
    };
    const getPreviewUrl = (fileName) => {
        if (!fileName) return ''; // или URL дефолтного изображения
        // console.log(`${MINIO_BASE_URL}/${fileName}`);
        return `${MINIO_BASE_URL}/videos/${fileName}`;
    };
</script>

<template>
  <div class="video-card">
    <div class="video-block">
      <div class="thumbnail-overlay-badge">
        <div class="badge" role="img" :aria-label="`${video.duration} секунд`">
          <div class="badge-text">{{ video.duration }}</div>
        </div>
      </div>
      <img 
        class="video-thumbnail" 
        :src="getPreviewUrl(video.preview?.fileName)" 
        :alt="video.title"
        @error="handleImageError" 
      />
    </div>
    
    <div class="bottom-block">
            <div class="video-info">
                <!-- Заголовок видео -->
                <h3 class="video-title">{{ video.title }}</h3>
                <!-- Название канала -->
                <p class="channel-name">{{video.user.userName}}</p>
            </div>
            <!-- Кнопка управления -->
            <KebabButton 
                ref="kebabButtonRef" 
                :onClick="handleKebabMenuClick">
            </KebabButton>        
        </div>
        <!-- Кебаб-меню -->

    </div>
</template>

<style scoped>
    .video-card {
        display: block;
        width: 20%;
    }

    .video-block {
        position: relative;
        width: 100%;
        height: auto;
        position: relative;
        flex-shrink: 0;
        overflow: hidden;
        aspect-ratio: 16/9;
    }

    .thumbnail-overlay-badge {
        position: absolute;
        bottom: 5px;
        right: 5px;
        width: 7%;
        height: 5%;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 3px;
        padding: 2px 4px;
        z-index: 1;
        color: #f3f0e9;
        font-size: 12px;
    }

    .video-thumbnail {
        display: block;
        width: 100%;
        width: 100%;
        height: auto;
        position: relative;
        flex-shrink: 0;
        overflow: hidden;
        aspect-ratio: 16/9;
        background: #f39e60;
        object-fit: cover;
    }

    .bottom-block {
        display: flex;
        width: 100%;
        justify-content: space-between;
    }

    .video-info {
        display: flex;
        flex-direction: column;
        gap: 2vh;
        color: #f3f0e9;
        width: 12vw;
        overflow: hidden;
        word-wrap: break-word;
        white-space: normal;
    }
    .video-info:first-child {
        padding-top: 1vh;
    }

    .video-title,
    .channel-name {
        margin: 0;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
    }
</style>