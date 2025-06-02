<script setup>
    
</script>

<template>
    <div class="side-menu">
    <!-- Содержимое бокового меню -->
        <ul>
            <li><a href="#">Главная</a></li>
            <li><a href="#">Мои видео</a></li>
            <li><a href="#">История</a></li>
            <li><a href="#">Настройки</a></li>
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
    }

    .side-menu a {
        color: #f3f0e9;
        text-decoration: none;
    }

    .side-menu a:hover {
        color: #f39e60;
    }
</style>