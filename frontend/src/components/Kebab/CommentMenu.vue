<script setup>
    import { ref, onBeforeUnmount, watch, nextTick, computed } from "vue";
    import { useMenuManager } from '@/assets/utils/useMenuManager.js';
    import { onClickOutside } from '@vueuse/core';

    const emit = defineEmits(['delete', 'edit-click', 'share']);
    const isOpen = ref(false);
    const position = ref({ top: '0px', left: '0px' });
    const menuRef = ref(null);

    const { registerMenu, unregisterMenu } = useMenuManager();

    const handleEdit = () => {
        emit('edit-click');
        closeMenu();
    };

    const handleDelete = () => {
        emit('delete');
        closeMenu();
    };

    const openMenu = async (buttonElement) => {
        try {
            if (!buttonElement?.getBoundingClientRect) {
                console.error('Invalid button element');
                return;
            }

            if (isOpen.value) {
                await closeMenu();
                return;
            }

            registerMenu({ closeMenu });
            
            const rect = buttonElement.getBoundingClientRect();
            const windowsScroll = window.scrollY;
            
            isOpen.value = true;
            
            await nextTick();
            
            if (menuRef.value) {
                const menuRect = menuRef.value.getBoundingClientRect();
                
                let left = rect.left - menuRect.width;
                if (left < 0) left = rect.left + rect.width;
                
                let top = windowsScroll + rect.top;
                if (top + menuRect.height > window.innerHeight + window.scrollY) {
                    top = windowsScroll + rect.top - menuRect.height;
                }
                
                position.value = {
                    left: `${left}px`,
                    top: `${top}px`
                };
            }
            
            onClickOutside(menuRef, closeMenu, { capture: true });
        } 
        catch (error) {
            console.error('Error opening menu:', error);
        }
    };

    const closeMenu = async () => {
        if (!isOpen.value) return;
        isOpen.value = false;
        unregisterMenu({ closeMenu });
        await nextTick();
    };

    onBeforeUnmount(() => {
        closeMenu();
    });
    defineExpose({ 
        openMenu, 
        closeMenu,
    });
</script>
<template>
    <div
        v-if="isOpen"
        ref="menuRef"
        class="kebab-menu"
        :style="position"
        @click.stop
    >
        <button @click="handleEdit">Редактировать</button>
        <button @click="handleDelete">Удалить</button>
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
        color: #100E0E;
    }
</style>