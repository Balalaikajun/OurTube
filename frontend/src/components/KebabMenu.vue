<script setup>
import { ref, onBeforeUnmount, watch, nextTick } from "vue";

const emit = defineEmits(['close', 'add-to-playlist', 'watch-later', 'share']);

const isOpen = ref(false);
const position = ref({ top: '0px', left: '0px' });
const menuRef = ref(null);

// Единый обработчик для всех событий закрытия
const setupEventListeners = () => {
  // Обработчик кликов вне меню
  const handleClickOutside = (event) => {
    if (menuRef.value && !menuRef.value.contains(event.target)) {
      closeMenu();
    }
  };

  // Обработчик скролла
  const handleScroll = () => {
    closeMenu();
  };

  // Обработчик клавиши ESC
  const handleKeyDown = (e) => {
    if (e.key === 'Escape') {
      closeMenu();
    }
  };

  // Добавляем все обработчики
  document.addEventListener('click', handleClickOutside);
  window.addEventListener('scroll', handleScroll, { passive: true });
  document.addEventListener('keydown', handleKeyDown);

  // Функция для очистки
  return () => {
    document.removeEventListener('click', handleClickOutside);
    window.removeEventListener('scroll', handleScroll);
    document.removeEventListener('keydown', handleKeyDown);
  };
};

let cleanupListeners = null;

const openMenu = async (buttonElement) => {
  if (!buttonElement?.getBoundingClientRect) return;
  
  // Закрываем предыдущее меню, если было открыто
  if (isOpen.value) {
    closeMenu();
  }
  
  // Рассчитываем позицию
  const rect = buttonElement.getBoundingClientRect();
  position.value = {
    top: `${rect.bottom + window.scrollY}px`,
    left: `${rect.left + window.scrollX}px`
  };
  
  isOpen.value = true;
  
  // Ждем обновления DOM и добавляем обработчики
  await nextTick();
  cleanupListeners = setupEventListeners();
};

const closeMenu = () => {
  if (!isOpen.value) return;
  
  isOpen.value = false;
  emit('close');
  
  // Удаляем все обработчики событий
  if (cleanupListeners) {
    cleanupListeners();
    cleanupListeners = null;
  }
};

// Автоматическая очистка при размонтировании
onBeforeUnmount(() => {
  closeMenu();
});

defineExpose({ openMenu, closeMenu });
</script>

<template>
  <div
    v-if="isOpen"
    ref="menuRef"
    class="kebab-menu"
    :style="position"
    @click.stop
  >
    <button @click="$emit('add-to-playlist')">Добавить в плейлист</button>
    <button @click="$emit('watch-later')">Смотреть позже</button>
    <span class="line"></span>
    <button @click="$emit('share')">Поделиться</button>
  </div>
</template>

<style scoped>
.kebab-menu {
  position: absolute;
  background: #4A4947;
  border: none;
  border-radius: 4px;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-width: 160px;
}

.kebab-menu button {
  background: none;
  border: none;
  color: #f3f0e9;
  cursor: pointer;
  padding: 10px 12px;
  text-align: left;
  transition: background-color 0.2s ease;
}

.kebab-menu button:first-child {
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
}

.kebab-menu button:last-child {
  border-bottom-left-radius: 4px;
  border-bottom-right-radius: 4px;
}

.kebab-menu button:hover {
  background: #F39E60;
}

.line {
  align-self: center;
  width: 90%;
  height: 1px;
  background-color: #f3f0e9;
  margin: 4px 0;
}
</style>