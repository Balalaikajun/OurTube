<script setup>
import api from '@/assets/utils/api.js'
import UserAvatar from '../Solid/UserAvatar.vue'
import { createKeyboardTrap } from '@/assets/utils/keyTrap.js'
import { computed, nextTick, onBeforeUnmount, ref } from 'vue'

const userData = computed(() => JSON.parse(localStorage.getItem('userData'))) //правки

const isOpen = ref(false)
const overlayContentRef = ref(null)

const fileInputRef = ref(null)

const avatar = ref(null)
const newAvatarFile = ref(null)
const avatarPreview = ref(null)
const alias = ref(null)
const newAlias = ref(null)

const isDragging = ref(false)

const keyboardTrap = createKeyboardTrap(overlayContentRef)
const initUserData = () => {
  alias.value = userData.value?.userName || ''
  newAlias.value = userData.value?.userName || ''
  avatar.value = userData.value?.userAvatar || null
  newAvatarFile.value = null // Сбрасываем файл при инициализации
}

const openMenu = async () => {
  console.log(userData.value)
  try {
    if (isOpen.value) {
      await closeMenu()
    }

    isOpen.value = true
    initUserData()

    await nextTick()

    document.addEventListener('click', handleClickOutside)
    keyboardTrap.setup() // Активируем ловушку клавиатуры
  } catch (error) {
    console.error('Error opening menu:', error)
  }
}

const closeMenu = () => {
  isOpen.value = false
  document.removeEventListener('click', handleClickOutside)
  keyboardTrap.teardown() // Деактивируем ловушку клавиатуры
}

const handleClickOutside = (event) => {
  console.log('handleClickOutside аккаунт')
  if (
      overlayContentRef.value &&
      !overlayContentRef.value.contains(event.target)
  ) {
    closeMenu()
  }
}

const handleDragOver = (e) => {
  e.preventDefault()
  isDragging.value = true
}

const handleDragLeave = () => {
  isDragging.value = false
}

const handleDrop = (e) => {
  e.preventDefault()
  isDragging.value = false

  if (e.dataTransfer.files && e.dataTransfer.files[0]) {
    handleFileSelect(e.dataTransfer.files[0])
  }
}

const openFileDialog = () => {
  fileInputRef.value.click()
}

const handleFileSelect = (file) => {
  if (!file.type.match('image.*')) {
    alert('Пожалуйста, выберите изображение')
    return
  }

  newAvatarFile.value = file

  const reader = new FileReader()
  reader.onload = (e) => {
    avatarPreview.value = e.target.result
  }
  reader.readAsDataURL(file)
}

const handleSaveChanges = () => {
  console.log(newAvatarFile.value)
  if (newAvatarFile.value !== null) {
    postAvatar()
  }

  if (newAlias.value !== alias.value) {
    patchAlias()
  }
}

const postAvatar = async () => {
  try {
    const formData = new FormData()
    formData.append('Image', newAvatarFile.value)

    console.log('$$avatar', avatar.value)
    if (avatar.value == null) {

    }

    const response = await api.post(`/users/me`, formData, {
      headers: {
        'accept': 'text/plain',
        'Content-Type': 'multipart/form-data'
      }
    })
    console.log(response)

    // Обновляем аватар в локальном хранилище
    const updatedUserData = { ...userData.value }
    updatedUserData.userAvatar = response.data.fileName // или полный URL, в зависимости от API
    localStorage.setItem('userData', JSON.stringify(updatedUserData))

    // Сбрасываем файл после успешной отправки
    newAvatarFile.value = null

    console.log('Аватар успешно обновлен:', response.data)
  } catch (error) {
    console.error('Ошибка изменения аватара:', error)
  }
}

const patchAlias = async () => {
  try {
    const response = await api.patch(`/users/me`, {
      userName: newAlias.value
    })

    // Обновляем имя в локальном хранилище
    const updatedUserData = { ...userData.value }
    updatedUserData.userName = newAlias.value
    localStorage.setItem('userData', JSON.stringify(updatedUserData))

    // Обновляем текущее имя
    alias.value = newAlias.value

    window.dispatchEvent(new Event('storage'))

    console.log('Псевдоним успешно обновлен:', response.data)
  } catch (error) {
    console.error('Ошибка изменения псевдонима:', error)
  }
}

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
  keyboardTrap.teardown()
})

defineExpose({
  openMenu,
  closeMenu,
  isOpen,
})

</script>

<template>
  <div v-if="isOpen" class="overlay">
    <div ref="overlayContentRef" class="overlay-content" @click.stop>
      <h1 style="color: #f3f0e9; margin: 0; text-align: start; width: 100%">
        Изменение данных аккаунта
      </h1>
      <div class="functional-wrapper">
        <div
            class="avatar-wrapper"
            @dragover="handleDragOver"
            @dragleave="handleDragLeave"
            @drop="handleDrop"
            @click="openFileDialog"
            :class="{ 'dragging': isDragging }"
        >
          <UserAvatar
              v-if="newAvatarFile == null"
              :user-info="{ userAvatar: avatar }"
              :context-use="'profile'"
          />
          <div v-if="newAvatarFile !== null">
            <img class="preview-img" :src="avatarPreview" alt="User avatar preview">
          </div>
          <input
              type="file"
              ref="fileInputRef"
              accept="image/*"
              @change="(e) => handleFileSelect(e.target.files[0])"
              style="display: none;"
          />
        </div>

        <!-- Скрытый input для выбора файла -->
        <div class="input-group">
                    <textarea
                        class="standart-input"
                        ref="textareaRef"
                        v-model="newAlias"
                        maxlength="100"
                        rows="1"
                    ></textarea>
        </div>
      </div>

      <div class="buttons-wrapper" style="width: 100%">
        <!-- <button
            class="reusable-button fit"
            @click.stop="uploadFiles"
            :class="{
                'disabled': !videoTitle.trim() || !videoFile,
                'isFilled': videoTitle.trim() && videoFile
            }"
            :disabled="!videoTitle.trim() || !videoFile"
            >
            <span>Загрузить</span>
        </button> -->
        <button
            class="reusable-button fit"
            :class="{ disabled: newAlias === alias && newAvatarFile === null}"
            :disabled="newAlias == alias && newAvatarFile === null"
            @click.stop="handleSaveChanges"
        >
          <span>Сохранить</span>
        </button>
        <button class="reusable-button fit" @click.stop="closeMenu">
          <span>Закрыть</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.overlay-content {
  display: flex;
  flex-direction: column;
  width: fit-content;
  background: #4a4947;
  padding: 20px;
  gap: 20px;
  border-radius: 4px;
}

.functional-wrapper {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  align-items: center;
  gap: 2vw;
}

.input-group {
  display: flex;
  flex-direction: row;
  align-content: center;
  width: 100%;
  height: fit-content;
}

.input-group textarea {
  box-sizing: border-box;
  width: 100%;
  text-wrap: nowrap;
}

.avatar-wrapper {
  width: fit-content;
  height: fit-content;
}

.avatar-wrapper:hover, .avatar-wrapper.dragging {
  opacity: 0.5;
}

.preview-img {
  width: 100px;
  height: 100px;
}

/* @media (max-width: 1000px) {
    .functional-wrapper {
        flex-direction: column;
        gap: 2vh;
    }
} */
</style>