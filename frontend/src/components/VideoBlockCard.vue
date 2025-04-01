<script setup>
    import { ref } from "vue";
    import KebabMenu from "./KebabMenu.vue";

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
</script>

<template>
    <div class="video-card">
        <div class="video-block">
        <!-- Значок времени -->
        <div class="thumbnail-overlay-badge">
            <div class="badge" role="img" :aria-label="`${video.duration} секунд`">
            <div class="badge-text">{{ video.duration }}</div>
            </div>
        </div>
        <!-- Изображение -->
        <img class="video-thumbnail" :src="video.preview.fileName" :alt="video.title" />
        </div>
        <div class="bottom-block">
        <div class="video-info">
            <!-- Заголовок видео -->
            <h3 class="video-title">{{ video.title }}</h3>
            <!-- Название канала -->
            <p class="channel-name">Мистер Макс</p>
        </div>
        <!-- Кнопка управления -->
        <button class="control-button" ref="buttonRef" @click="handleKebabMenuClick">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
            <path
                fill="#F3F0E9"
                d="M10 16.5c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5-1.5-.67-1.5-1.5.67-1.5 1.5-1.5zM8.5 12c0 .83.67 1.5 1.5 1.5s1.5-.67 1.5-1.5-.67-1.5-1.5-1.5-1.5.67-1.5 1.5zm0-6c0 .83.67 1.5 1.5 1.5s1.5-.67 1.5-1.5-.67-1.5-1.5-1.5-1.5.67-1.5 1.5z"
            />
            </svg>
        </button>
        </div>
        <!-- Кебаб-меню -->
        <KebabMenu
        v-if="buttonRef"
        :videoId="video.id"
        :buttonRef="buttonRef"
        ref="kebabMenuRef"
        />
    </div>
</template>

<style scoped>
    .video-card {
    display: block;
    width: 15vw;
    }

    .video-block {
    position: relative;
    }

    .thumbnail-overlay-badge {
    position: absolute;
    bottom: 5px;
    right: 5px;
    background: rgba(0, 0, 0, 0.8);
    border-radius: 3px;
    padding: 2px 4px;
    z-index: 1;
    color: #f3f0e9;
    font-size: 12px;
    }

    .video-thumbnail {
    width: 100%;
    height: 18vh;
    background: #f39e60;
    object-fit: cover;
    display: block;
    }

    .control-button {
    display: flex;
    position: relative;
    top: 0;
    right: 0;
    justify-content: center;
    align-items: center;
    border: 0;
    padding: 0;
    cursor: pointer;
    background: transparent;
    width: 20px;
    height: 20px;
    }

    .control-button svg {
    display: inherit;
    width: 20px;
    height: 20px;
    justify-content: center;
    align-items: center;
    }

    .bottom-block {
    display: flex;
    width: 100%;
    justify-content: space-between;
    }

    .video-info {
    color: #f3f0e9;
    width: 12vw;
    overflow: hidden;
    word-wrap: break-word;
    white-space: normal;
    }

    .video-title,
    .channel-name {
    margin: 0;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    margin-top: 8px;
    }
</style>