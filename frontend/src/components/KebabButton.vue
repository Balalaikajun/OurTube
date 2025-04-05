<script setup>
    import { ref, onMounted, onBeforeUnmount } from "vue";

    const props = defineProps({
      onClick: {
        type: Function,
        required: true
      }
    });

    const buttonRef = ref(null);

    defineExpose({ buttonRef});
</script>

<template>
  <button 
    class="control-button" 
    ref="buttonRef" 
    @click.stop="onClick($event)"
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
        justify-self: center;
        justify-content: center;
        align-items: center;
        width: clamp(24px, 1.5vw, 36px);
        height: clamp(24px, 1.5vw, 36px);
        border: 0;
        padding: 0;
        cursor: pointer;
        background: transparent;
        transition: background 0.3s ease;
        border-radius: 50%;
        margin-top: 1vh;
    }

    .control-button:hover {
        background: #4A4947;
    }

    .kebab-dots {
        display: flex;
        width: clamp(24px, 1.5vw, 36px);
        height: clamp(24px, 1.5vw, 36px);
        justify-content: center;
        align-items: center;
        width: 100%;
        height: 100%;
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
