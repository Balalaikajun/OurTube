<script setup>
    import { ref, onMounted } from "vue";
    import axios from 'axios';
    import { API_BASE_URL } from "@/assets/config.js";

    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    // Функция для получения данных пользователя
    const fetchUserData = async () => {
        try {
            const response = await api.get('/api/User');
            return response.data;
        } catch (err) {
            console.error('Ошибка при получении данных пользователя:', err);
            return null;
        }
        };

        // Проверяем токен и загружаем данные при монтировании
    onMounted(async () => {
            
        const userData = await fetchUserData();
        if (userData) {
            // Сохраняем данные пользователя в localStorage
            localStorage.setItem('userData', JSON.stringify(userData));
            console.log("userData", userData);
        }
    });
</script>

<template>
    <router-view :key="$route.fullPath" />
</template>

<style scoped>
    
</style>