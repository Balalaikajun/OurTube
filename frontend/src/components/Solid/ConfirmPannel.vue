<script setup>
import { onBeforeUnmount, ref } from 'vue'

const props = defineProps({
        action: {
            type: String,
            required: true,
            default: "Подтверждение"
        }
    });

    const emit = defineEmits(['confirm']);

    const isOpen = ref(false);
    const overlayRef = ref(null);
    const overlayContentRef = ref(null);

    const openMenu = () => {
        isOpen.value = true;
        document.addEventListener('mousedown', handleClickOutside);
    };

    const closeMenu = () => {
        isOpen.value = false;
        document.removeEventListener('mousedown', handleClickOutside);
    };
    
    const handleClickOutside = (event) => {
        if (overlayContentRef.value && !overlayContentRef.value.contains(event.target)) {
            closeMenu();
        }
    };
    
    const handleConfirm = () => {
        emit('confirm');
        closeMenu();
    };

    onBeforeUnmount(() => {
        document.removeEventListener('click', handleClickOutside);
    });

    defineExpose({
        openMenu,
        closeMenu
    });
</script>

<template>
    <div class="overlay"
        v-if="isOpen"
        ref="overlayRef"
    >
        <div ref="overlayContentRef" class="panel">
            <p>{{ action }}</p>
            <div class="functions">
                <button class="control-button" @click="handleConfirm">Подтвердить</button>
                <button class="control-button" @click="closeMenu">Отмена</button>
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
    .panel
    {
        box-sizing: border-box;
        padding: 20px;
        border-radius: 4px;
        width: min-content;
        height: min-content;
        
        display: flex;
        flex-direction: column;
        align-content: start;
        gap: 2rem;
        color: #f3f0e9;
        white-space: nowrap;
        background-color: #4A4947;
    }
    .panel p
    {
        font-size: 1.2rem;
    }
    .functions
    {
        display: flex;
        flex-direction: row;
        font-size: 0.875rem;
        gap: 10px;
        justify-content: space-between;
    }
    button
    {
        height: 35px;
        padding: 10px;
        background: #252525;
    }
    button:hover 
    {
        background: #100E0E;
    }
</style>