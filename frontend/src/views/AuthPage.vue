<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/assets/utils/api.js'
import { saveUserDataToLocalStorage } from '@/assets/store/userServiсe.js'

const email = ref('')
const password = ref('')
const loading = ref(false)
const errorMessage = ref('')

const router = useRouter()

const submitForm = async () => {
  errorMessage.value = ''
  loading.value = true

  try {
    const response = await api.post('/identity/login?useCookies=true&useSessionCookies=true', {
      email: email.value,
      password: password.value,
    })

    console.log(response)

    // Получаем и сохраняем данные пользователя
    const userData = await saveUserDataToLocalStorage(true)

    if (!userData) {
      throw new Error('Не удалось получить данные пользователя')
    }

    router.push('/')
  } catch (error) {
    handleAuthError(error)
  } finally {
    loading.value = false
  }
}
const handleAuthError = (error) => {
  if (error.response) {
    const errorData = error.response.data
    errorMessage.value = errorData.title || 'Ошибка авторизации'

    if (errorData.errors) {
      const errors = Object.values(errorData.errors).flat()
      errorMessage.value += ': ' + errors.join(', ')
    }
  } else {
    errorMessage.value = error.message || 'Ошибка соединения с сервером'
  }
}
</script>

<template>
  <div class="auth-page">
    <div class="auth-page__enter">
      <form @submit.prevent="submitForm" class="auth-form">
        <h1 class="auth-form__title">Авторизация</h1>
        <div class="auth-form__fields">
          <input
              class="auth-form__input"
              type="email"
              v-model="email"
              name="user_email"
              placeholder="Почта"
              required
              autocomplete="new-email"
          >
          <input
              class="auth-form__input"
              type="password"
              v-model="password"
              name="user_password"
              placeholder="Пароль"
              required
              autocomplete="new-password"
          >
        </div>

        <button
            class="auth-form__submit"
            type="submit"
            :disabled="loading">
          {{ loading ? 'Загрузка...' : 'Вход' }}
        </button>
        <p v-if="errorMessage"
           class="auth-form__error"
        >
          {{ errorMessage }}</p>
      </form>
    </div>
    <div class="auth-page__actions">
      <router-link to="/register" custom v-slot="{ navigate }">
        <button
            class="auth-page__action"
            @click="navigate">Регистрация
        </button>
      </router-link>
      <router-link to="/forgot-password" custom v-slot="{ navigate }">
        <button
            class="auth-page__action"
            @click="navigate">Восстановление доступа
        </button>
      </router-link>
    </div>
  </div>
</template>

<style scoped>

</style>