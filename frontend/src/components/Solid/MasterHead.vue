<script setup>
    import { ref, onMounted, onUnmounted, inject } from "vue";
    import MainMenu from "./MainMenu.vue"; // Импортируем компонент бокового меню
    import { API_BASE_URL } from "@/assets/config.js"
    import { useRouter } from 'vue-router';
    import { injectFocusEngine } from '@/assets/utils/focusEngine.js';

    const router = useRouter();
    const { register, unregister } = injectFocusEngine();

    const isSideMenuVisible = ref(false);
    const searchQuery = ref("");
    const searchResults = ref([]);
    const isLoading = ref(false);
    const errorMessage = ref("");

    const API_URL = API_BASE_URL + "/api/Search";

    const handleFocus = () => {
        register('searchInput');
    };

    const handleBlur = () => {
        setTimeout(() => {
            if (!document.activeElement?.closest('.search-block')) {
            unregister('searchInput');
            }
        }, 100);
    };

    const toggleSideMenu = (event) => {
        if (event.target.closest('.burger-button')) {
            isSideMenuVisible.value = !isSideMenuVisible.value;
        }
    };

    const handleClickOutside = (event) => {
        if (isSideMenuVisible.value && 
            !event.target.closest('.side-menu') && 
            !event.target.closest('.burger-button')) {
            isSideMenuVisible.value = false;
        }
    };

    const pushToMain = () => {
        router.push(`/`);
    }

    const handleSearch = async (event) => {
        event.preventDefault();
        if (searchQuery.value.trim()) {
            router.push({ path: '/search', query: { q: searchQuery.value.trim() } });
        }
    };

    onMounted(() => {
        document.addEventListener('click', handleClickOutside);
    });

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside);
    });
</script>

<template>
    <div class="master-head-block">
        <button class="burger-button" v-on:click="toggleSideMenu">
            <span></span>
            <span></span>
            <span></span>
        </button>

        <h1 class="logo-text" @click="pushToMain"><span class="our">Our</span>Tube</h1>

        <search class="search-block">
            <form @submit="handleSearch">
                <input 
                    type="search" 
                    v-model="searchQuery"
                    placeholder="Поиск видео..."
                    aria-label="Поиск видео"
                    class="search-input"
                    @focus="handleFocus"
                    @blur="handleBlur"
                    @keypress.enter="handleSearch"
                >
                <button 
                    type="submit" 
                    class="search-button"
                    :disabled="isLoading || !searchQuery.trim()"
                    >
                    <span v-if="isLoading">Поиск...</span>
                    <span v-else>Поиск</span>
                </button>
            </form>
        </search>
    </div>
    
    <MainMenu v-if="isSideMenuVisible" class="side-menu"/>
    
</template>

<style scoped>
    .master-head-block {
        display: flex;
        justify-content: space-between; /* Равномерное распределение пространства */
        box-sizing: border-box;
        align-items: center;
        position: fixed;
        background: #100E0E;
        top: 0;
        height: 70px;
        padding: 0 25px;
        width: 100%;
        z-index: 1000;
        gap: 20px; /* Добавляем отступ между элементами */
    }
    .our {
        color: #F39E60;
    }
    .burger-button {
        display: flex;
        flex-direction: column;
        gap: 15px;
        cursor: pointer;
        background: transparent;
    }
    .burger-button span {
        width: 50px;
        height: 2px;
        background-color: #F3F0E9;
    }

    .logo-text {
        position: relative; 
        color: #F3F0E9; 
    }

    .search-block {
        display: flex;
        flex-grow: 1; /* Занимает все доступное пространство */
        justify-content: center; /* Центрируем содержимое */
        margin: 0 auto; /* Дополнительное центрирование */
    }

    .search-block form {
        display: flex;
        width: 100%; /* Форма занимает всю ширину search-block */
        justify-content: safe; /* Центрируем элементы формы */
    }

    .search-input {
        padding: 7px 15px;
        border-radius: 15px 0 0 15px;
        box-sizing: border-box;
        border: 1px solid #F3F0E9;
        color: #F3F0E9;
        background: transparent;
        outline: none;
        width: 15vw;
        transform-origin: left center;
        transition: width 0.3s, border-color 0.3s;
        font-size: 14px;
    }

    .search-input::placeholder {
        color: #F3F0E9;
        opacity: 0.6;
    }

    .search-input:focus::placeholder {
        opacity: 1;
    }

    .search-input:focus {
        width: 25vw;
        border-color: #F39E60;
    }

    .search-button {
        padding: 8px 15px;
        box-sizing: content-box;
        border: none;
        background-color: #F39E60;
        color: #100E0E;
        cursor: pointer;
        transition: background-color 1s ease;
        font-size: 14px;
    }

    .search-button:hover {
        background-color: #3a7bc8;
    }
</style>