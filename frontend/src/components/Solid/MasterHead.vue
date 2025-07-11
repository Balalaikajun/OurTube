<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import MainMenu from './MainMenu.vue' // Импортируем компонент бокового меню
import UserAvatar from './UserAvatar.vue'

import { useRoute, useRouter } from 'vue-router'
import { injectFocusEngine } from '@/assets/utils/focusEngine.js'
import api from '@/assets/utils/api.js'

const router = useRouter();
    const route = useRoute();
    const { register, unregister } = injectFocusEngine();

    const emit = defineEmits(['open-upload']);

    const userData = computed(() => JSON.parse(localStorage.getItem('userData')));
    const activeMenu = ref(null); // 'side' | 'account' | null
    const searchQuery = ref("");
    const isLoading = ref(false);

    const logout = async () => {
        try {
            const response = await api.post(`/api/identity/logout`)
            // console.log(response.status)
        }
        catch (error) {
            console.error('Ошибка разлогирования:', error)
        }
        finally
        {
            document.cookie.split(";").forEach(cookie => {
                const [name] = cookie.split("=");
                document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
            });
        
            localStorage.removeItem('userData');
            
            // Добавьте эту строку
            window.dispatchEvent(new CustomEvent('auth-update'));
            
            activeMenu.value = null;
            
            router.push('/');
        }
    };

    // const handleFocus = () => {
    //     register('searchInput');
    // };

    // const handleBlur = () => {
    //     setTimeout(() => {
    //         if (!document.activeElement?.closest('.search-block')) {
    //         unregister('searchInput');
    //         }
    //     }, 100);
    // };

    const toggleMenu = (menuType) => {
        if (activeMenu.value === menuType) {
            activeMenu.value = null;
        } else {
            activeMenu.value = menuType;
        }
    };

    const handleClickOutside = (event) => {
        const clickedOnMenuTrigger = 
            event.target.closest('.burger-button') || 
            event.target.closest('.user-avatar-container');
        
        if (!clickedOnMenuTrigger) {
            activeMenu.value = null;
        }
    };

    // const pushToMain = () => {
    //     router.push(`/`);
    // }

    const handleSearch = async (event) => {
        event.preventDefault();
        const query = searchQuery.value.trim();
        if (query) {
            // Если уже на странице поиска с тем же запросом - не навигируем
            if (route.path === '/search' && route.query.q === query) {
                return;
            }
            await router.push({ path: '/search', query: { q: query } });
        }
    };

    onMounted(() => {
        document.addEventListener('click', handleClickOutside);
        if (route.path === '/search' && route.query.q) {
            searchQuery.value = route.query.q;
        }
    });

    onUnmounted(() => {
        document.removeEventListener('click', handleClickOutside);
    });
</script>

<template>
    <header class="top-head">
      <div class="master-head-block">
        <button class="burger-button" @click="() => toggleMenu('side')" aria-label="Главное меню">
          <span></span>
          <span></span>
          <span></span>
        </button>
  
        <h1 class="logo-text" @click="() => router.push('/')">
          <span class="our">Our</span>Tube
        </h1>
  
        <form class="search-block" @submit="handleSearch">
          <input 
            type="search" 
            v-model="searchQuery"
            placeholder="Поиск видео..."
            aria-label="Поиск видео"
            class="search-input"
            @focus="() => register('searchInput')"
            @blur="() => setTimeout(() => !document.activeElement?.closest('.search-block') && unregister('searchInput'), 100)"
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
        <button v-auth="true" class="reusable-button fit" @click.stop="emit('open-upload')">
            Создать
        </button>
        <div class="user-avatar-container" @click="() => toggleMenu('account')">
          <UserAvatar />
        </div>
      </div>
      
      <MainMenu v-if="activeMenu === 'side'" class="side-menu" />
      
      <div v-if="activeMenu === 'account'" class="account-menu">
        <div v-auth="true" class="account-wrapper">
            <div class="account-header">
                <UserAvatar />
                <p class="user-name">{{ userData?.userName }}</p>
            </div>
            
            <span class="divider"></span>
            
            <div class="account-actions">                
                <button class="control-button">Настройки аккаунта</button>
                <button class="control-button" @click="logout">Выйти из аккаунта</button>
            </div>
        </div>
        <div v-auth="false" class="account-wrapper">
            <div class="account-actions">
                <router-link to="/login" custom v-slot="{ navigate }">
                    <button class="control-button" @click="navigate">Войти</button>
                </router-link>             
                <router-link to="/register" custom v-slot="{ navigate }">
                    <button class="control-button" @click="navigate">Регистрация</button>
                </router-link>
            </div>
        </div>
      </div>
    </header>
  </template>

<style scoped>
    input[type="search"]::-webkit-search-cancel-button {
        -webkit-appearance: none; /* Убираем стандартный стиль */
        height: 1em;
        width: 1em;
        background: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="%23F3F0E9"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg>') no-repeat;
        background-size: contain;
        cursor: pointer;
    }

    .top-head {
        position: fixed;
        top: 0;
        z-index: 1000;
        width: 100%;
    }

    .master-head-block {
        display: flex;
        justify-content: space-between;
        align-items: center;
        background: #100E0E;
        height: 70px;
        padding: 0 25px;
        gap: 20px;
    }

    .user-avatar-container {
        cursor: pointer;
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
        flex-grow: 1;
        margin: 0 auto;
    }

    .search-block form {
        display: flex;
        justify-content: start;
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
        opacity: 0;
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
        font-size: 14px;
        border-radius: 0 15px 15px 0;
    }

    .search-button:hover {
        background-color: #4A4947;
    }

    .search-button span {
        color: #100E0E;
    }

    .search-button:hover span {
        color: #F3F0E9;
    }

    .side-menu,
    .account-menu {
        position: absolute;
        z-index: 1001;        
    }
    .account-menu {
        right: 25px;
        top: 70px;
        width: 250px;
        padding: 20px;
        background: #4A4947;
    }

    .account-header {
        display: flex;
        align-items: center;
        gap: 15px;
    }

    .side-menu {
        left: 0;
        top: 70px;
        width: 300px;
        height: calc(100vh - 70px);
    }

    .account-menu span {
        display: block;
        content: "";
        height: 1px;
        width: 100%;
        background: #F3F0E9;
        margin-top: 10px;
        margin-bottom: 10px;
    }

    .reusable-button {
        border: 1px dashed #F3F0E9;
    }

    .user-name {
        font-size: 1rem;
        color: #F3F0E9;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
        max-width: 150px;
    }

    .divider {
        display: block;
        height: 1px;
        background: #F3F0E9;
        margin: 15px 0;
        opacity: 0.3;
    }

    .account-actions {
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    .account-wrapper div:nth-child(1) {
        gap: 20px;
    }

    .account-wrapper div:nth-last-child(1) {
        flex-direction: column;
    }

    .account-wrapper div:nth-last-child(1) button {
        padding: 10px;
    }

    .account-wrapper div:nth-last-child(1) button:hover {
        background: #100E0E;
    }

    .account-wrapper div {
        display: flex;
        flex-direction: row;
        gap: 10px;

    }
    .comment-text {
        display: -webkit-box;
        -webkit-line-clamp: 1; /* Количество строк до обрезки */
        -webkit-box-orient: vertical;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: normal;
    }

    @media (max-width: 768px) {
    .master-head-block {
        padding: 0 15px;
        gap: 15px;
    }
    
    .logo-text {
        font-size: 1.2rem;
    }
    }
</style>