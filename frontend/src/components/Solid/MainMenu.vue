<script setup>
    import { useRouter } from 'vue-router';
    const router = useRouter();

    const pushToMain = () => {
        router.push(`/`);
    }
    const pushToPlaylists = () => {
        router.push(`/playlists`);
    }
    const pushToHistory = () => {
        router.push(`/history`);
    }
</script>

<template>
    <div class="side-menu">
    <!-- Содержимое бокового меню -->
        <ul>
            <li @click="pushToMain">Главная</li>
            <!-- <li>Ваши видео</li> -->
            <li @click.stop="pushToPlaylists">Плейлисты</li>
            <li @click.stop="pushToHistory">История</li>
            <li>Учётная запись</li>
            <!-- <li>Настройки</li> -->
        </ul>
    </div>
</template>

<style scoped>
    .side-menu {
        position: fixed;
        font-size: 24px;
        top: 70px; /* Высота верхней панели */
        left: 0;
        width: 250px;
        height: calc(100vh - 70px);
        background: #100E0E;
        color: #f3f0e9;
        padding-left: 25px;
        z-index: 1000;
    }

    .side-menu ul {
        list-style: none;
        padding: 0;
    }

    .side-menu li {
        margin: 20px 0;
        color: #f3f0e9;
        text-decoration: none;
        cursor: pointer;
    }

    .side-menu li:hover {
        color: #f39e60;
    }
</style>