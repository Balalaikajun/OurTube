<script setup>
    import { ref, onMounted, onUnmounted } from "vue";
    import MainMenu from "./MainMenu.vue"; // Импортируем компонент бокового меню
    import { API_BASE_URL } from "@/assets/config.js"
    import { useRouter } from 'vue-router';

    const router = useRouter();

    const isSideMenuVisible = ref(false);
    const searchQuery = ref("");
    const searchResults = ref([]);
    const isLoading = ref(false);
    const errorMessage = ref("");

    const API_URL = API_BASE_URL + "/api/Search";

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

    const fetchVideos = async (query) => {
    try {
        isLoading.value = true;
        errorMessage.value = "";
        
        const response = await fetch(`${API_URL}?query=${encodeURIComponent(query)}`);
        
        // if (!response.ok) {
        //     throw new Error(`Ошибка HTTP: ${response.status}`);
        // }
        
        const data = await response.json();
        searchResults.value = data;
        
    } catch (error) {
        console.error("Ошибка поиска:", error);
        errorMessage.value = "Не удалось загрузить результаты. Попробуйте позже.";
        searchResults.value = [];
    } finally {
        isLoading.value = false;
    }
    };

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

        <h1 class="logo-text" style="">OurTube</h1>

        <search class="search-block">
            <form @submit="handleSearch">
                <input 
                    type="search" 
                    v-model="searchQuery"
                    placeholder="Поиск видео..."
                    aria-label="Поиск видео"
                    class="search-input"
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
        flex-direction: row;
        align-items: center;
        position: fixed;
        background: #100E0E;
        top: 0;
        height: 70px;
        padding: 0 25px;
        width: 100%; /* Чтобы блок занимал всю ширину */
        z-index: 1000; /* Чтобы блок был поверх других элементов */
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
        top: -2px; 
        margin-left: 25px;
        color: #F3F0E9; 
        line-height: 1px;
    }

    .search-block {
        display: flex;
        align-items: center;
        margin-left: 25vw;
    }

    .search-block form {
        display: flex;
        flex-direction: row;
        align-items: center;
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
        transition: background-color 0.3s;
        font-size: 14px;
    }

    .search-button:hover {
        background-color: #3a7bc8;
    }
</style>