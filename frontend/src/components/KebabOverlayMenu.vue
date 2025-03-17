<script setup>
    import { ref } from "vue";
    import ShareOverlay from "./ShareOverlay.vue";
    import PlayListOverlay from "./PlayListOverlay.vue";

    const isMenuOpen = ref(false);
    const isShareOverlayOpen = ref(false);
    const isPlaylistOverlayOpen = ref(false);

    const toggleMenu = () => {
        isMenuOpen.value = !isMenuOpen.value;
    };

    const openShareOverlay = () => {
        isShareOverlayOpen.value = true;
        isMenuOpen.value = false;
    };

    const openPlaylistOverlay = () => {
        isPlaylistOverlayOpen.value = true;
        isMenuOpen.value = false;
    };

    const addToWatchLater = () => {
        alert("Добавлено в 'Смотреть позже'");
        isMenuOpen.value = false;
    };
</script>

<template>
    <div class="kebab-menu-container">
        <!-- Кнопка кебаб-меню -->
        <button class="control-button" @click="toggleMenu">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20">
            <path
            fill="#F3F0E9"
            d="M10 16.5c.83 0 1.5.67 1.5 1.5s-.67 1.5-1.5 1.5-1.5-.67-1.5-1.5.67-1.5 1.5-1.5zM8.5 12c0 .83.67 1.5 1.5 1.5s1.5-.67 1.5-1.5-.67-1.5-1.5-1.5-1.5.67-1.5 1.5zm0-6c0 .83.67 1.5 1.5 1.5s1.5-.67 1.5-1.5-.67-1.5-1.5-1.5-1.5.67-1.5 1.5z"
            />
        </svg>
        </button>

        <!-- Меню -->
        <div v-if="isMenuOpen" class="kebab-menu">
        <button @click="openPlaylistOverlay">Добавить в плейлист</button>
        <button @click="addToWatchLater">Смотреть позже</button>
        <button @click="openShareOverlay">Поделиться</button>
        </div>

        <!-- Оверлей для плейлистов -->
        <PlayListOverlay
        v-if="isPlaylistOverlayOpen"
        @close="isPlaylistOverlayOpen = false"
        />

        <!-- Оверлей для копирования ссылки -->
        <ShareOverlay
        v-if="isShareOverlayOpen"
        @close="isShareOverlayOpen = false"
        />
    </div>
</template>

<style scoped>
    .kebab-menu-container {
        position: relative;
    }

    .kebab-menu {
        position: absolute;
        top: 100%;
        right: 0;
        background: #333;
        border: 1px solid #555;
        border-radius: 4px;
        padding: 8px;
        z-index: 1000;
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .kebab-menu button {
        background: none;
        border: none;
        color: #f3f0e9;
        cursor: pointer;
        padding: 4px 8px;
        text-align: left;
    }

    .kebab-menu button:hover {
        background: #555;
    }
</style>