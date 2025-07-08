<script setup>
import api from "@/assets/utils/api.js";
import { createKeyboardTrap } from "@/assets/utils/keyTrap.js";
import { nextTick, onBeforeUnmount, ref, provide } from "vue";

const isOpen = ref(false);
const shareRef = ref(null);
const overlayContentRef = ref(null);
const videoInputRef = ref(null);
const previewInputRef = ref(null);
const loadFieldRef = ref(null);
const previewFieldRef = ref(null);

const uploadResult = ref();

const videoTitle = ref("");
const videoDescription = ref("");
const videoFile = ref(null);
const previewFile = ref(null);
const isDraggingVideo = ref(false);
const isDraggingPreview = ref(false);

const keyboardTrap = createKeyboardTrap(overlayContentRef);

const openMenu = async () => {
  try {
    if (isOpen.value) {
      await closeMenu();
    }

    isOpen.value = true;

    await nextTick();

    document.addEventListener("click", handleClickOutside);
    keyboardTrap.setup(); // Активируем ловушку клавиатуры
  } catch (error) {
    console.error("Error opening menu:", error);
  }
};

const closeMenu = () => {
  isOpen.value = false;
  document.removeEventListener("click", handleClickOutside);
  resetForm();
  keyboardTrap.teardown(); // Деактивируем ловушку клавиатуры
};

const handleClickOutside = (event) => {
  if (
    overlayContentRef.value &&
    !overlayContentRef.value.contains(event.target)
  ) {
    closeMenu();
  }
};

const resetForm = () => {
  videoTitle.value = "";
  videoDescription.value = "";
  videoFile.value = null;
  previewFile.value = null;
  uploadResult.value = null;
};

const handleVideoInput = (e) => {
  const file = e.target.files[0];
  if (file && file.type.startsWith("video/")) {
    videoFile.value = file;
    updateVideoFieldDisplay();
  }
};

const handleVideoDrop = (e) => {
  e.preventDefault();
  isDraggingVideo.value = false;
  const file = e.dataTransfer.files[0];
  if (file && file.type.startsWith("video/")) {
    videoFile.value = file;
    updateVideoFieldDisplay();
  }
};

const handlePreviewInput = (e) => {
  const file = e.target.files[0];
  if (file && file.type.startsWith("image/")) {
    previewFile.value = file;
    updatePreviewFieldDisplay();
  }
};

const handlePreviewDrop = (e) => {
  e.preventDefault();
  isDraggingPreview.value = false;
  const file = e.dataTransfer.files[0];
  if (file && file.type.startsWith("image/")) {
    previewFile.value = file;
    updatePreviewFieldDisplay();
  }
};

const updateVideoFieldDisplay = () => {
  if (loadFieldRef.value && videoFile.value) {
    loadFieldRef.value.innerHTML = `
                <p>${
                  videoFile.value.name
                }</p>
                <p>
                    (${(videoFile.value.size / (1024 * 1024)).toFixed(2)} MB)
                </p>
            `;
  }
};

const updatePreviewFieldDisplay = () => {
  if (previewFieldRef.value && previewFile.value) {
    previewFieldRef.value.innerHTML = `
                <p style="color: #F3F0E9; margin: 0;">${
                  previewFile.value.name
                }</p>
                <p style="color: #F3F0E9; margin: 5px 0 0; font-size: 0.8em;">
                    (${(previewFile.value.size / (1024 * 1024)).toFixed(2)} MB)
                </p>
            `;
  }
};

const triggerVideoInput = () => videoInputRef.value.click();
const triggerPreviewInput = () => previewInputRef.value.click();

const uploadFiles = async () => {
  if (!videoFile.value || !videoTitle.value.trim()) return;

  const formData = new FormData();
  formData.append("Title", videoTitle.value);
  formData.append("Description", videoDescription.value);
  formData.append("VideoFile", videoFile.value);
  if (previewFile.value) {
    formData.append("PreviewFile", previewFile.value);
  }

  try {
    const response = await api.post('api/Video', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    uploadResult.value = "Видео загружено успешно.";
    closeMenu();
  } catch (error) {
    console.error("Error uploading video:", error);
    uploadResult.value = error.response?.data || error.message;
  }
};

onBeforeUnmount(() => {
  document.removeEventListener("click", handleClickOutside);
  resetForm(); // Очищаем данные формы
  keyboardTrap.teardown();
});

defineExpose({
  openMenu,
  closeMenu,
  isOpen,
});
</script>
<template>
  <div v-if="isOpen" ref="shareRef" class="overlay">
    <div ref="overlayContentRef" class="overlay-content" @click.stop>
      <h1 style="color: #f3f0e9; margin: 0; text-align: start; width: 100%">
        Загрузка видео
      </h1>
      <!-- <input type="text" :value="`https://localhost:5173/video/${videoId}`" readonly /> -->

      <input
        type="file"
        ref="videoInputRef"
        @change="handleVideoInput"
        accept="video/*"
        hidden
      />
      <input
        type="file"
        ref="previewInputRef"
        @change="handlePreviewInput"
        accept="image/*"
        hidden
      />

      <div class="files-wrapper" v-if="!uploadResult">
        <div
          ref="loadFieldRef"
          class="load-field"
          @click="triggerVideoInput"
          @dragover.prevent="isDraggingVideo = true"
          @dragleave.prevent="isDraggingVideo = false"
          @drop="handleVideoDrop"
          :style="{ borderColor: isDraggingVideo ? '#F39E60' : '#F3F0E9' }"
        >
          <div class="field-content">
            <p v-if="!videoFile">Перетащите видео или кликните для выбора</p>
            <div v-else>
              <p>{{ videoFile.name }}</p>
              <p class="file-size">
                ({{ (videoFile.size / (1024 * 1024)).toFixed(2) }} MB)
              </p>
            </div>
          </div>
        </div>
        <div
          ref="previewFieldRef"
          class="load-field"
          @click="triggerPreviewInput"
          @dragover.prevent="isDraggingPreview = true"
          @dragleave.prevent="isDraggingPreview = false"
          @drop="handlePreviewDrop"
          :style="{ borderColor: isDraggingPreview ? '#F39E60' : '#F3F0E9' }"
        >
          <div class="field-content">
            <p v-if="!previewFile">
              Перетащите изображение или кликните для выбора
            </p>
            <div v-else>
              <p>{{ previewFile.name }}</p>
              <p class="file-size">
                ({{ (previewFile.size / (1024 * 1024)).toFixed(2) }} MB)
              </p>
            </div>
          </div>
        </div>
      </div>
      <div class="files-wrapper" v-else>
        {{ uploadResult }}
      </div>

      <textarea
        class="standart-input"
        ref="textareaRef"
        @input="adjustHeight"
        v-model="videoTitle"
        placeholder="Заголовок"
        rows="1"
      ></textarea>
      <textarea
        class="standart-input"
        ref="textareaRef"
        @input="adjustHeight"
        v-model="videoDescription"
        placeholder="Описание (опционально)"
        rows="3"
      ></textarea>
      <div class="buttons-wrapper" style="width: 100%">
        <button
            class="reusable-button fit"
            @click.stop="uploadFiles"
            :class="{
                'disabled': !videoTitle.trim() || !videoFile,
                'isFilled': videoTitle.trim() && videoFile
            }"
            :disabled="!videoTitle.trim() || !videoFile"
            >
            <span>Загрузить</span>
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
  width: 30%;
  /* min-width: 500px; Добавлено минимальная ширина */
  display: flex;
  flex-direction: column;
  background: #4a4947;
  padding: 20px;
  gap: 20px;
  border-radius: 4px;
  align-items: center;
  text-align: center;
}

.files-wrapper {
  display: flex;
  flex-direction: row;
  width: 100%;
}

.load-field {
  border: 2px dashed #f3f0e9;
  box-sizing: border-box;
  width: 50%;
  height: 200px;
  padding: 15px;
  min-width: 0; /* Разрешаем сжатие */
  text-overflow: ellipsis;
  overflow: hidden;
}

.field-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  width: 100%;
  text-align: center;
  color: #f3f0e9;
  min-width: 0; /* Важно: разрешаем сжатие */
}

.field-content p {
  display: inline-block; /* Для правильной работы text-overflow */
  margin: 0;
  width: 100%;
  word-break: break-word; /* Перенос длинных слов */
  overflow: hidden;
  text-overflow: ellipsis; /* Добавляем троеточие при переполнении */
  white-space: normal;
  padding: 0 5px;
  box-sizing: border-box; /* Учитываем padding в ширине */
}

.file-size {
  font-size: 0.8em;
  margin-top: 5px !important;
  color: #f3f0e9;
}

.field-content p.word-break {
  white-space: normal;
  word-break: break-all;
}
</style>
