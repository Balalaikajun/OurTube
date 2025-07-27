<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/assets/utils/api.js'

const router = useRouter();

const email = ref("");
const loading = ref(false);
const errorMessage = ref("");

const submitForm = async () => {
    loading.value = true;
    errorMessage.value = "";

    console.log(email.value)
    try {
        const response = await api.post(`/identity/forgotPassword`, 
            JSON.stringify({ email: email.value }), 
            {
                headers: {
                "Content-Type": "application/json",
                },
            }
        );

        router.push({ path: "/reset-password", query: { email: email.value } });
    } 
    catch (error) {
        errorMessage.value = "Произошла ошибка при отправке запроса. Пожалуйста, попробуйте еще раз.";
        console.error("Ошибка:", error);
    } 
    finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="auth-page">
        <div class="auth-page__enter">
            <form @submit.prevent="submitForm" class="auth-form">
                <h1 class="auth-form__title">Восстановление доступа</h1>
                <div class="auth-form__fields">
                    <input
                        class="auth-form__input"  
                        type="email"
                        v-model="email"
                        name="email"
                        placeholder="Почта"
                        required
                        autocomplete="new-email"
                    />
                    <p v-if="!errorMessage" class="info-text instruction-text">
                        На указанную почту будет отправлена форма для смены пароля.
                    </p>
                    <p v-if="errorMessage" class="info-text error">{{ errorMessage }}</p>
                </div>

                <button class="auth-form__submit" :disabled="loading">
                    Восстановление
                </button>
            </form>
            
        </div>
        <div class="auth-page__actions">
            <p class="info-text">
                Регистрируясь на нашей платформе Вы автоматически соглашаетесь с политикой пользователя.
            </p>
        </div>
        <div class="right-pannel"></div>
    </div>
</template>

<style scoped>

</style>