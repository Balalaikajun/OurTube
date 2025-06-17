<script setup>
    import { ref } from "vue";
    import { useRouter } from "vue-router";
    import { API_BASE_URL } from "@/assets/config"; // Импортируем базовый URL
    import axios from "axios";
    
    const email = ref("");
    const password = ref("");
    const loading = ref(false);
    const errorMessage = ref("");

    const router = useRouter();

    const api = axios.create({
        baseURL: API_BASE_URL,
        headers: {
            "Content-Type": "application/json",
        },
    });

    const submitForm = async () => {
    errorMessage.value = "";
    loading.value = true;

    try {
        const response = await api.post("/identity/login?useCookies=true&useSessionCookies=true", {
            email: email.value,
            password: password.value,
        });

        // Сохраняем токен
        localStorage.setItem("token", response.data.accessToken);
        
        // Добавляем интерсептер для автоматической подстановки токена
        // api.interceptors.request.use((config) => {
        //     const token = localStorage.getItem("token");
        //     if (token) {
        //         config.headers.Authorization = `Bearer ${token}`;
        //     }
        //     return config;
        // });

        router.push("/");
    } catch (error) {
        if (error.response) {
        // Обрабатываем ответ сервера
        const errorData = error.response.data;
        errorMessage.value = errorData.title || "Ошибка авторизации";
        
        // Показываем детали ошибок валидации
        if (errorData.errors) {
            const errors = Object.values(errorData.errors).flat();
            errorMessage.value += ": " + errors.join(", ");
        }
        } else {
        errorMessage.value = "Ошибка соединения с сервером";
        }
    } finally {
        loading.value = false;
    }
    };
</script>

<template>
    <div class="content">
        <div class="authorize">
            <form @submit.prevent="submitForm" class="main-page-block">
                <label>Авторизация</label>
                <input 
                    type="email" 
                    v-model="email" 
                    name="user_email" 
                    placeholder="Почта" 
                    required 
                    autocomplete="new-email"
                >
                <input 
                    type="password" 
                    v-model="password" 
                    name="user_password" 
                    placeholder="Пароль" 
                    required
                    autocomplete="new-password"
                >
                <button type="submit" :disabled="loading">
                    {{ loading ? "Загрузка..." : "Вход" }}
                </button>
                <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
            </form>
        </div>
        <div class="other-content">
            <router-link to="/register" custom v-slot="{ navigate }">
                <button @click="navigate">Регистрация</button>
            </router-link>
            <router-link to="/forgot-password" custom v-slot="{ navigate }">
                <button @click="navigate">Восстановление доступа</button>
            </router-link>
        </div>
        <div class="right-pannel">

        </div>
    </div>
</template>

<style scoped>
    .error {
        color: red;
        text-align: center;
        margin-top: 10px;
    }

    .content {
        display: flex;
        justify-content: center;
        align-items: center;
        gap: 126px;
        height: 100vh;
    }
    .authorize {
        display: flex;
        width: 25vw;
        height: 100vh;
        align-items: center;
        justify-content: center;
        background: #F3F0E9;
    }

    .main-page-block {
        display: flex;
        flex-direction: column;
        width: 100%;
        height: 50vh;
    }

    label, button{
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 8vh;
        color: #F3F0E9;
        font-size: 36px;
        background: #100E0E;
    }

    .content button{
        margin-top: auto;
        text-decoration: none;
    }
    .content button:hover {
        color: #100E0E;
        background: #F39E60;
        cursor: pointer;
    }
    .main-page-block input {
        width: 15vw;
        height: 3vh;
        align-self: center;
        color: #100E0E;
        background-color: #F3F0E9;
        border-radius: 0px;
        outline: none;
        border: 1px solid #100E0E;
    }
    .main-page-block input::placeholder {
        color: #100E0E;
    }
    .main-page-block input:focus::placeholder {
        opacity: 0;
    }
    .main-page-block input:focus, input:disabled {
        background-color: #F39E60;
        border-radius: 0px;
        outline: none;
    }
    .main-page-block input:first-of-type {
        margin-top: auto;
        margin-bottom: 11vh;
    }

    .other-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 25vw;
        height: 34vh;
    }
    .other-content button:first-of-type {
        margin-top: 0;
    }
</style>