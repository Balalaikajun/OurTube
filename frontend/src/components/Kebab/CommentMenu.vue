<script setup>
    import { ref, onBeforeUnmount, watch, nextTick } from "vue";
    import { useMenuManager } from '@/assets/utils/useMenuManager.js';

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
        // console.log(buttonElement)
        try {
            if (!buttonElement?.getBoundingClientRect) {
                console.error('Invalid button element');
                return;
            }

            
            // Если меню уже открыто - сначала закрываем
            if (isOpen.value) {
                console.log(position.value, 'меню уже открыто')
                await closeMenu();
                return;
            }
            
            registerMenu({ closeMenu });
            // Получаем позицию кнопки
            const rect = buttonElement.getBoundingClientRect();
            const windowsScroll = window.scrollY;
            
            isOpen.value = true;
            
            // Ждем рендера меню
            await nextTick();
            
            // Корректируем позицию после рендера
            if (menuRef.value) {
                const menuRect = menuRef.value.getBoundingClientRect();
                position.value = {
                    left: `${rect.left - menuRect.width}px`,
                    top: `${windowsScroll + rect.top}px`
                };
                console.log(menuRef.value, position.value, 'изменение позиции меню')
            }
            
            // Устанавливаем обработчики
            cleanupListeners = setupEventListeners();
        } 
        catch (error) {
            console.error('Error opening menu:', error);
        }
    };

    const closeMenu = async () => {
        
        if (!isOpen.value) return;
        
        isOpen.value = false;
        unregisterMenu({ closeMenu });
        
        // Очищаем обработчики
        if (cleanupListeners) {
            cleanupListeners();
            cleanupListeners = null;
        }
        
        // Даем время на анимацию закрытия
        await nextTick();
    };

    const setupEventListeners = () => {
        const handleClickOutside = (event) => {
            console.log('closeMenuClickOutside')
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