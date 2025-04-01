<script setup>
import { ref } from "vue";

const props = defineProps({
  onClick: {
    type: Function,
    required: true,
  },
});

const buttonRef = ref(null);

defineExpose({ buttonRef });
</script>

<template>
  <button 
    class="control-button" 
    ref="buttonRef" 
    @click.stop="onClick"
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
    .control-button {
        display: flex;
        position: relative;
        justify-content: center;
        align-items: center;
        border: 0;
        padding: 0;
        cursor: pointer;
        background: transparent;
        transition: background 0.3s ease;
        border-radius: 50%;
        width: 2vw;
        height: 2vw;
        margin-left: auto;
    }

    .control-button:hover {
        background: #4A4947;
    }

    .kebab-dots {
        display: flex;
        flex-direction: column;
        gap: 0.5vh;
        align-items: center;
        justify-content: center;
        width: 100%;
        height: 100%;
    }

    .dot {
        display: block;
        width: 0.3vh;
        height: 0.3vh;
        border-radius: 50%;
        background-color: #F3F0E9;
    }
</style>