<script setup>
    import { ref, onBeforeUnmount, watch, nextTick } from "vue";
    import { useMenuManager } from '@/assets/utils/useMenuManager.js';
    import { onClickOutside } from '@vueuse/core';

    const emit = defineEmits(['delete', 'edit-click', 'share']);
    const isOpen = ref(false);
    const position = ref({ top: '0px', left: '0px' });
    const menuRef = ref(null);

    const { registerMenu, unregisterMenu } = useMenuManager();

    let cleanupListeners = null;

    // const handleClick = (event) => {
    //     emit('kebab-click', { 
    //         buttonElement: event.currentTarget 
    //     });
    // };
    const handleEdit = () => {
        console.log("menu")
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

            // Закрываем текущее меню перед открытием нового
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
                
                // Проверяем, чтобы меню не выходило за пределы экрана
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
            
            // Настраиваем обработчик кликов снаружи
            onClickOutside(menuRef, (event) => {
                // Игнорируем клики по кнопке меню
                if (event.target.closest('.kebab-button')) return;
                
                closeMenu();
            }, { capture: true });
            
            // Обработчик закрытия по Esc
            const handleKeyDown = (e) => e.key === 'Escape' && closeMenu();
            document.addEventListener('keydown', handleKeyDown);
            
            return () => {
                document.removeEventListener('keydown', handleKeyDown);
            };
        } 
        catch (error) {
            console.error('Error opening menu:', error);
        }
    };

    const closeMenu = async () => {
        if (!isOpen.value) return;
        
        isOpen.value = false;
        unregisterMenu({ closeMenu });
        
        // Даем время на анимацию закрытия
        await nextTick();
    };

    const setupEventListeners = () => {
        const handleClickOutside = (event) => {
            // Игнорируем клики по самой кнопке меню
            if (event.target.closest('.kebab-button')) {
                return;
            }
            
            console.log('closeMenuClickOutside');
            if (menuRef.value && !menuRef.value.contains(event.target)) {
                closeMenu();
            }
        };

        // Остальные обработчики без изменений
        const handleScroll = () => closeMenu();
        const handleKeyDown = (e) => e.key === 'Escape' && closeMenu();

        // Используем capture phase для более надежного обнаружения кликов
        document.addEventListener('click', handleClickOutside, true);
        window.addEventListener('scroll', handleScroll, { passive: true });
        document.addEventListener('keydown', handleKeyDown);

        return () => {
            document.removeEventListener('click', handleClickOutside, true);
            window.removeEventListener('scroll', handleScroll);
            document.removeEventListener('keydown', handleKeyDown);
        };
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
    }
</style>