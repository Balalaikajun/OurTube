<script setup>
import { ref, onBeforeUnmount, watch, nextTick } from "vue";

const emit = defineEmits(['close', 'add-to-playlist', 'watch-later', 'share']);

const isOpen = ref(false);
const position = ref({ top: '0px', left: '0px' });
const menuRef = ref(null);
let cleanupListeners = null;

const handleClick = (event) => {
  emit('kebab-click', { 
    buttonElement: event.currentTarget 
  });
};

const handleAddToPlaylist = () => {
  console.log(`Добавить видео ${props.videoId} в плейлист`);
  closeMenu();
};

const handleWatchLater = () => {
  console.log(`Отложить видео ${props.videoId}`);
  closeMenu();
};

const handleShare = () => {
  emit('share');
  closeMenu();
};

const setupEventListeners = () => {
  const handleClickOutside = (event) => {
    if (menuRef.value && !menuRef.value.contains(event.target)) {
      closeMenu();
    }
  };

  const handleScroll = () => closeMenu();
  const handleKeyDown = (e) => e.key === 'Escape' && closeMenu();

  document.addEventListener('click', handleClickOutside);
  window.addEventListener('scroll', handleScroll, { passive: true });
  document.addEventListener('keydown', handleKeyDown);

  return () => {
    document.removeEventListener('click', handleClickOutside);
    window.removeEventListener('scroll', handleScroll);
    document.removeEventListener('keydown', handleKeyDown);
  };
};

const openMenu = async (buttonElement) => {
  try {
    if (!buttonElement?.getBoundingClientRect) {
      console.error('Invalid button element');
      return;
    }
    
    // Если меню уже открыто - сначала закрываем
    if (isOpen.value) {
      await closeMenu();
    }
    
    // Получаем позицию кнопки
    const rect = buttonElement.getBoundingClientRect();
    
    isOpen.value = true;
    
    // Ждем рендера меню
    await nextTick();
    
    // Корректируем позицию после рендера
    if (menuRef.value) {
      const menuRect = menuRef.value.getBoundingClientRect();
      position.value = {
        left: `${rect.left - menuRect.width}px`,
        top: `${rect.top}px`
      };
    }
    
    // Устанавливаем обработчики
    cleanupListeners = setupEventListeners();
  } catch (error) {
    console.error('Error opening menu:', error);
  }
};

const closeMenu = async () => {
  if (!isOpen.value) return;
  
  isOpen.value = false;
  emit('close');
  
  // Очищаем обработчики
  if (cleanupListeners) {
    cleanupListeners();
    cleanupListeners = null;
  }
  
  // Даем время на анимацию закрытия
  await nextTick();
};

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
    @click.stop="handleClick"
  >
    <button @click="handleAddToPlaylist">Добавить в плейлист</button>
    <button @click="handleWatchLater">Смотреть позже</button>
    <span class="line"></span>
    <button @click.stop="handleShare">Поделиться</button>
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
  transition: background-color 1s ease;
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