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

    const emit = defineEmits(['click', 'kebab-click']);

    const buttonRef = ref(null);

    const handleKebabClick = (event) => {
    event.stopPropagation();
    if (!buttonRef.value) return;
        // Получаем DOM-элемент кнопки
        const buttonElement = buttonRef.value.$el || buttonRef.value;
        emit('kebab-click', {
            videoId: props.video.id,
            buttonElement
        });
    };

    const handleCardClick = () => {
    emit('click', props.video.id);
    };

    // const handleKebabMenuClick = () => {
    // if (kebabMenuRef.value) {
    //     kebabMenuRef.value.toggleMenu(); // Вызываем toggleMenu из KebabMenu
    // }
    // };
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
    defineExpose({buttonRef});
</script>

<template>
  <div class="video-card" @click="handleCardClick">
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
                ref="buttonRef" 
                :onClick="handleKebabClick">
            </KebabButton>        
        </div>
    </div>
</template>

<style scoped>
    .video-card {
        display: block;
        width: 20%;
        padding-bottom: 20px;
        cursor: pointer;
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
        justify-content: space-between;
        width: 100%;
    }

    .video-info {
        display: flex;
        margin-top: 1vh;
        width: 90%;
        flex-direction: column;
        color: #f3f0e9;
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
        -webkit-line-clamp: 2; /* Ограничение в 2 строки */
        -webkit-box-orient: vertical; /* Вертикальное направление */
        line-height: 1.4; /* Оптимальный межстрочный интервал */
        max-height: calc(2 * 1.4em); /* Дополнительная страховка */
        word-break: break-word; /* Перенос длинных слов */
    }
</style>