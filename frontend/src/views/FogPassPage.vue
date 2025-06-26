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

    try {
        const response = await api.post(`/identity/forgotPassword`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                email: email.value,
            }),
        });

        if (!response.ok) {
            throw new Error("Ошибка при отправке запроса");
        }

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
    <div class="content">
        <div class="reg">
            <form @submit.prevent="submitForm" class="main-page-block">
                <label>Восстановление доступа</label>
                <input style="margin-top: 13.5vh;"
                    type="email"
                    v-model="email"
                    name="email"
                    placeholder="Почта"
                    required
                    autocomplete="new-email"
                />
                <p v-if="!errorMessage" class="instruction-text">
                    На указанную почту будет отправлена форма для смены пароля.
                </p>
                <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
                <button type="" :disabled="loading">
                    Отправить запрос на восстановление
                </button>
            </form>
            
        </div>
        <div class="other-content">
            <p style="color: #F3F0E9; font-size: 36px;">
                Регистрируясь на нашей платформе Вы автоматически соглашаетесь политикой пользователя.
            </p>
        </div>
        <div class="right-pannel"></div>
    </div>
</template>

<style scoped>
    .error {
        color: red;
        text-align: left;
        margin-top: 20px;
        width: 15vw;
        align-self: center;
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
        width: 100%;
        height: 50vh; /* Увеличил высоту для формы регистрации */
    }

    label,
    button {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 10vh;
        color: #f3f0e9;
        font-size: 36px;
        background: #100e0e;
    }

    .content button {
        margin-top: auto;
        text-decoration: none;
        cursor: pointer;
    }

    .content button:hover {
        color: #100e0e;
        background: #f39e60;
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
    }

    .other-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 25vw;
        height: 30vh;
    }

    .other-content button:first-of-type {
        margin-top: 0;
    }

    .instruction-text {
        color: #100e0e;
        font-size: 16px;
        width: 15vw;
        align-self: center;
        margin-top: 20px; /* Отступ сверху для текста */
    }

    .other-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: 25vw;
        height: 30vh;
        justify-content: center;
    }

    .other-content p {
        margin: 0;
    }
</style>