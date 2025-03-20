<script setup>
    import { ref, onMounted } from "vue";
    import { useRouter, useRoute } from "vue-router";
    import { API_BASE_URL } from "@/assets/config.js"

    const router = useRouter();
    const route = useRoute();

    const code = ref("");
    const newPassword = ref("");
    const confirmPassword = ref("");
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
        if (newPassword.value !== confirmPassword.value) {
            errorMessage.value = "Пароли не совпадают";
            return;
        }

    loading.value = true;
    errorMessage.value = "";

    try {
        const response = await fetch(`${API_BASE_URL}/identity/resetPassword`, {
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

        if (!response.ok) {
            throw new Error("Ошибка при смене пароля");
        }

        // Перенаправление на страницу авторизации
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
    <div class="content">
        <div class="reg">
            <form @submit.prevent="submitForm" class="main-page-block">
                <label>Смена пароля</label>
                <input
                    type="text"
                    v-model="code"
                    placeholder="Временный код"
                    required
                    autocomplete="off"
                />
                <input
                    type="password"
                    v-model="newPassword"
                    placeholder="Новый пароль"
                    required
                    autocomplete="new-password"
                />
                <input
                    type="password"
                    v-model="confirmPassword"
                    placeholder="Подтвердите пароль"
                    required
                    autocomplete="new-password"
                />
                <button type="submit" :disabled="loading">
                    Сменить пароль
                </button>
            </form>
            <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
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

    .main-page-block input:not(:first-child)
    {
        margin-top: 3vh;
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