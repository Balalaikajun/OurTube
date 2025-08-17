<script setup>
import { ref } from 'vue'

const emit = defineEmits(['kebab-click'])

const buttonRef = ref(null)

const handleClick = (event) => {
  event.stopPropagation()
  emit('kebab-click', event)
}

defineExpose({ buttonRef })
</script>

<template>
  <button
      class="kebab-button-type control-button"
      ref="buttonRef"
      @click.stop="handleClick"
      aria-label="Меню действий"
  >
    <span class="kebab-dots">
      <span class="dot"></span>
      <span class="dot"></span>
      <span class="dot"></span>
    </span>
  </button>
</template>

<style scoped>
/* .kebab-button {
    display: flex;
    position: relative;
    justify-self: center;
    justify-content: center;
    align-items: center;
    width: clamp(24px, 1.5vw, 36px);
    height: clamp(24px, 1.5vw, 36px);
    border: 0;
    padding: 0;
    cursor: pointer;
    background: transparent;
    transition: background 1s ease;
    border-radius: 50%;
    margin-top: 1vh;
}

.kebab-button:hover {
    background: #4A4947;
} */

.kebab-dots {
  display: flex;
  width: clamp(24px, 1.5vw, 36px);
  height: clamp(24px, 1.5vw, 36px);
  justify-content: center;
  align-items: center;
  flex-direction: column;
  gap: 3px;
}

.dot {
  width: 2px;
  height: 2px;
  border-radius: 50%;
  background-color: #F3F0E9;
}
</style>
