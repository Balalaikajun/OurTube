<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/assets/utils/api.js'

const router = useRouter();
    const route = useRoute();

    const passwordInput = ref(null);

    const code = ref("");
    const newPassword = ref("");
    const loading = ref(false);
    const errorMessage = ref("");

    // Проверка наличия email в query-параметрах
    onMounted(() => {
        if (!route.query.email) {
            // Если email отсутствует, перенаправляем на страницу восстановления пароля
            router.push("/forgot-password");
        }
    });

    const submitForm = async () => {
        loading.value = true;
        errorMessage.value = "";

        try {
            const response = await api.post(`/identity/resetPassword`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    email: route.query.email, // Используем email из query-параметров
                    resetCode: code.value,
                    newPassword: newPassword.value,
                }),
            });

            router.push("/login");
        } catch (error) {
            errorMessage.value = "Произошла ошибка при смене пароля. Пожалуйста, попробуйте еще раз.";
            console.error("Ошибка:", error);
        } finally {
            loading.value = false;
        }
    };
</script>

<template>
    <div class="auth-page">
        <div class="auth-page__enter">
            <form @submit.prevent="submitForm" class="auth-form">
                <h1 class="auth-form__title">Смена пароля</h1>
                <div class="auth-form__fields">
                    <input
                        class="auth-form__input"
                        type="text"
                        v-model="code"
                        placeholder="Временный код"
                        required
                        autocomplete="off"
                    />
                    <input
                        ref="passwordInput"
                        class="auth-form__input"
                        type="password"
                        v-model="newPassword"
                        placeholder="Новый пароль"
                        required
                        autocomplete="new-password"
                        @focus="passwordInput.type = 'text'"
                        @blur="passwordInput.type = 'password'"
                    />
                </div>
                <button class="auth-form__submit" :disabled="loading">
                    Сменить пароль
                </button>
            </form>
            <p v-if="errorMessage" class="info-text error">{{ errorMessage }}</p>
        </div>
    </div>
</template>

<style scoped>

</style>