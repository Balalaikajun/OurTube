<script setup>
    import { ref, computed, onMounted } from "vue";
</script>
<template>
    <div class="video-card">
        <div class="video-block">
            <div class="thumbnail-overlay-badge">
                <div class="badge" role="img" aria-label="3 минуты 49 секунд">
                    <div class="badge-text">3:49</div>
                </div>
            </div>
            <img src="" alt="">
        </div>
        <div class="bottom-block">
            <div class="video-info">
                <h3 class="video-title">укеыуеыуыукыупsrssfdgsdfgsdfgsffffffff</h3>
                <p class="channel-name">ffffffffffffffssssssssaaaaaaa</p>
            </div>
            <button class="control-button">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
                    <path fill="#F3F0E9"
                        d="M10 16.5c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5-1.5-.67-1.5-1.5.67-1.5 1.5-1.5zM8.5 12c0 .83.67 1.5 1.5 1.5s1.5-.67 1.5-1.5-.67-1.5-1.5-1.5-1.5.67-1.5 1.5zm0-6c0 .83.67 1.5 1.5 1.5s1.5-.67 1.5-1.5-.67-1.5-1.5-1.5-1.5.67-1.5 1.5z">
                    </path>
                </svg>
            </button>
        </div>
    </div>
</template>
<style scoped>
    .video-card {
        display: block;
        width: 15vw;
    }
    .video-block {
        position: relative; /* Родительский контейнер для позиционирования значка */
    }

    .thumbnail-overlay-badge {
        position: absolute; /* Абсолютное позиционирование */
        bottom: 5px; /* Отступ от нижнего края */
        right: 5px; /* Отступ от правого края */
        background: rgba(0, 0, 0, 0.8); /* Полупрозрачный черный фон */
        border-radius: 3px; /* Скругленные углы */
        padding: 2px 4px; /* Внутренние отступы */
        z-index: 1; /* Значок поверх изображения */
        color: #F3F0E9;
        font-size: 12px;
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
    img {
        width: 100%;
        height: 18vh;
        background: #F39E60;
    }
    .bottom-block {
        display: flex;
        width: 100%;
        display: flex;
        justify-content: space-between;
    }
    .video-info {
        color: #F3F0E9;
        width: 12vw;
        overflow: hidden; /* Обрезает текст, если он выходит за пределы блока */
        word-wrap: break-word; /* Переносит слова на новую строку */
        white-space: normal; /* Разрешает перенос строк */
    }

    .video-title,
    .channel-name {
        margin: 0; /* Убираем отступы по умолчанию */
        overflow: hidden; /* Обрезает текст, если он выходит за пределы блока */
        text-overflow: ellipsis; /* Добавляет многоточие, если текст не помещается */
        display: -webkit-box;
        -webkit-line-clamp: 2; /* Ограничивает текст до 2 строк */
        -webkit-box-orient: vertical; /* Вертикальное направление текста */
        margin-top: 8px;
    }
</style>