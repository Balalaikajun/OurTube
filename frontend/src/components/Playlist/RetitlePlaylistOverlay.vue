<script setup>
    import { ref, onMounted, onBeforeUnmount, nextTick} from "vue";
    import { injectFocusEngine } from '@/assets/utils/focusEngine.js';

    const emit = defineEmits(['retitle']);

    const { register, unregister } = injectFocusEngine();

    const isOpen = ref(false);
    const overlayContentRef = ref(null);
    const playlist = ref('');

    const toggleMenu = async (title) => {
        console.log(title)
        playlist.value = title;
        await nextTick();
        isOpen.value = !isOpen.value;

        if (isOpen.value) {
            document.addEventListener('mousedown', handleClickOutside);
        } else {
            document.removeEventListener('mousedown', handleClickOutside);
        }
    };

    const handleFocus = () => {
        register('retitleOverlay');
    };

    const handleBlur = () => {
        setTimeout(() => {
            if (!document.activeElement?.closest('.overlay-content')) {
                unregister('retitleOverlay');
            }
        }, 100);        
    };

    const handleClickOutside = (event) => {
        // Проверяем, что клик был вне оверлея и не по кнопке, которая его открывает
        if (overlayContentRef.value && 
            !overlayContentRef.value.contains(event.target) &&
            !event.target.closest('[data-ignore-outside-click]')) {
            isOpen.value = false;
            document.removeEventListener('mousedown', handleClickOutside);
        }
    };

    const retitlePlaylist = async (event) => {
        event?.stopPropagation();
        console.log(playlist.value)
        emit('retitle', playlist.value);
        isOpen.value = false;
        document.removeEventListener('mousedown', handleClickOutside);
    };
    onBeforeUnmount(() => {
        document.removeEventListener('mousedown', handleClickOutside);
    });

    defineExpose({
        toggleMenu
    });
</script>

<template>
    <div class="overlay" v-if="isOpen">
        <div ref="overlayContentRef">
            <div class="overlay-content">
                <div class="top">
                    <h3>Переименование плейлиста</h3>
                </div>

                <div class="playlist-title">
                    <textarea 
                        v-model="playlist"
                        @focus="handleFocus"
                        @blur="handleBlur"
                        placeholder="Введите название"
                        maxlength="100"
                        cols="1"
                        rows="1"
                        @keydown.enter="retitlePlaylist"
                    ></textarea>
                </div>

                <div class="bottom">
                    <button 
                        class="control-button comment-button"
                        :class="{ 
                            'disabled-button': !playlist.trim(), 
                            'comment-isFilled': playlist.trim() 
                        }"
                        :disabled="!playlist.trim()"
                        @click.stop="retitlePlaylist" 
                    >
                        Переименовать
                    </button>
                    <button @click.stop="() => {isOpen = false}" class="control-button comment-button">
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
        min-height: 60px; /* Начальная высота */
        max-height: 200px; /* Максимальная высота (если нужно ограничить) */
        border: 1px solid #F3F0E9;
        border-radius: 4px;
    }

    .playlist-title textarea {
        box-sizing: border-box;
        padding: 8px;
        overflow-y: hidden;
        width: 300px;
        min-height: 60px; /* Начальная высота */
        max-height: 200px; /* Максимальная высота (если нужно ограничить) */
        background: #252525;
        color: #F3F0E9;
        border: none;
        resize: none;
        font-family: inherit;
        font-size: inherit;
    }

    .playlist-title textarea:focus {
        outline: none;
        border-color: #F3F0E9;;
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

    .disabled-button:hover {
        cursor: default !important;
        background-color: #252525;
    }
    .comment-isFilled:hover {
        cursor: pointer !important;
        background-color: #F39E60;
        color: #100E0E;
    }
</style>