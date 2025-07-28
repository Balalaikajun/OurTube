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

            if (!response.status == 200) {
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
    <div class="auth-page">
        <div class="auth-page__enter">
            <form @submit.prevent="submitForm" class="auth-form">
                <h1 class="auth-form__title">Регистрация</h1>
                <div class="auth-form__fields">
                    <input
                        class="auth-form__input"
                        type="text"
                        v-model="username"
                        name="username"
                        placeholder="Логин"
                        required
                        disabled
                        autocomplete="off"
                    /> <!--Пока нет возможности-->
                    <input
                        class="auth-form__input"
                        type="email"
                        v-model="email"
                        name="email"
                        placeholder="Почта"
                        required
                        autocomplete="new-email"
                    />
                    <input
                        class="auth-form__input"
                        type="password"
                        v-model="password"
                        name="password"
                        placeholder="Пароль"
                        required
                        autocomplete="new-password"
                    />
                </div>
                <button class="auth-form__submit" type="submit" :disabled="loading">
                    {{ loading ? "Загрузка..." : "Зарегистрироваться" }}
                </button>
                <p v-if="errorMessage" class="info-text error">{{ errorMessage }}</p>
            </form>
        </div>
        <div class="auth-page__actions">
            <router-link to="/register" custom v-slot="{ navigate }">
                <button
                    class="auth-page__action"
                    @click="navigate">Регистрация</button>
            </router-link>
            <router-link to="/forgot-password" custom v-slot="{ navigate }">
                <button 
                    class="auth-page__action"
                    @click="navigate">Восстановление доступа</button>
            </router-link>
        </div>
    </div>
</template>

<style scoped>

</style>