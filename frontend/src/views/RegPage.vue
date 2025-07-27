<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/assets/utils/api.js'

const username = ref("");
    const email = ref("");
    const password = ref("");
    const loading = ref(false);
    const errorMessage = ref("");

    const router = useRouter();

    const submitForm = async () => {
        errorMessage.value = "";
        loading.value = true;

        try {
            // Запрос на бэкенд для регистрации
            // Эндпоинты менять не забывайте
            // console.log(email.value)
            const response = await api.post(`/identity/register`, {
                email: email.value,
                password: password.value,
            });

            if (!response.ok) {
                //const result = await response.json();
                throw new Error("Ошибка регистрации");
            }

            router.push("/login"); // Перенаправление на страницу авторизации

        } catch (error) {
            errorMessage.value = error.message;
        } finally {
            loading.value = false;
        }
    };
</script>

<template>
    <div class="content">
        <div class="reg">
            <form @submit.prevent="submitForm" class="main-page-block">
                <label>Регистрация</label>
                <input 
                    type="text"
                    v-model="username"
                    name="username"
                    placeholder="Логин"
                    required
                    disabled
                    autocomplete="off"
                /> <!--Пока нет возможности-->
                <input
                    type="email"
                    v-model="email"
                    name="email"
                    placeholder="Почта"
                    required
                    autocomplete="new-email"
                />
                <input
                    type="password"
                    v-model="password"
                    name="password"
                    placeholder="Пароль"
                    required
                    autocomplete="new-password"
                />
                <button type="submit" :disabled="loading">
                    {{ loading ? "Загрузка..." : "Зарегистрироваться" }}
                </button>
            </form>
            <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
        </div>
        <div class="other-content">
            <!-- <p>
                Регистрируясь на нашей платформе Вы автоматически соглашаетесь политикой пользователя.
            </p> -->
            <router-link to="/login" custom v-slot="{ navigate }">
                <button @click="navigate">Войти</button>
            </router-link>
            <router-link to="/forgot-password" custom v-slot="{ navigate }">
                <button @click="navigate">Восстановление доступа</button>
            </router-link>
        </div>
    </div>
</template>

<style scoped>
    p {
        color: #F3F0E9;
        font-size: 36px;
    }

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

    .reg {
        display: flex;
        flex-direction: column;
        width: 25vw;
        height: 100vh;
        align-items: center;
        justify-content: center;
        background: #f3f0e9;
    }

    .main-page-block {
        display: flex;
        flex-direction: column;
        gap: 4vh;
        width: 100%;
        height: 50vh; /* Увеличил высоту для формы регистрации */
    }

    label,
    button {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 8vh;
        color: #f3f0e9;
        font-size: 36px;
        background: #100e0e;
    }

    .content button {
        margin-top: auto;
        text-decoration: none;
    }

    .content button:hover {
        color: #100e0e;
        background: #f39e60;
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
        color: #100e0e;
    }

    .main-page-block input:focus::placeholder {
        opacity: 0;
    }

    .main-page-block input:focus, input:disabled {
        background-color: #f39e60;
        border-radius: 0px;
        outline: none;
    }
    .main-page-block input:first-of-type {
        margin-top: auto;
        /* margin-bottom: 11vh; */
    }

    .other-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 25vw;
        height: 34vh;
        gap: 4vh;
    }

    .other-content button:first-of-type {
        margin-top: 0;
    }
</style>