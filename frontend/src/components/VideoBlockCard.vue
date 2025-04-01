<script setup>
    import { ref } from "vue";
    import KebabMenu from "./KebabMenu.vue";
    import KebabButton from "./KebabButton.vue";

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
            <!-- <img class="video-thumbnail" :src="video.preview.fileName" :alt="video.title" /> -->
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
        width: 20%;
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