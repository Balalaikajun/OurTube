<script setup>
    import { ref, onBeforeUnmount, watch, nextTick } from "vue";
    import { scroll  } from '@/assets/utils/scroll.js';
    const props = defineProps({
        videoId: {
            type: [String, Number],
            required: true
        },
    })

    const isOpen = ref(false);
    const shareRef = ref(null);
    const overlayContentRef = ref(null);

    const openMenu = async () => {
        try {        
            // Если меню уже открыто - сначала закрываем
            if (isOpen.value) {
                await closeMenu();
            }
            
            isOpen.value = true;
            // scroll.lock();
            
            // Ждем рендера меню
            await nextTick();            
            
            // Устанавливаем обработчики
            document.addEventListener('click', handleClickOutside);
        } catch (error) {
            console.error('Error opening menu:', error);
        }
    };

    const closeMenu = () => {
        isOpen.value = false;
        // scroll.unlock();
        document.removeEventListener('click', handleClickOutside);
    };

    const handleClickOutside = (event) => {
        if (overlayContentRef.value && !overlayContentRef.value.contains(event.target)) {
            closeMenu();
        }
    };

    const copyLink = () => {
        // const link = `https://localhost:5173/video/${props.videoId}`; // Замените на реальную ссылку
        const link = `localhost:5173/video/${props.videoId}`;
        navigator.clipboard.writeText(link)
        .then(() => {
            // alert("Ссылка скопирована!");
            isOpen.value = !isOpen.value;
        })
        .catch(err => {
            console.error("Ошибка копирования:", err);
            alert("Не удалось скопировать ссылку");
        });
    };

    onBeforeUnmount(() => {
        document.removeEventListener('click', handleClickOutside);
    });

    defineExpose({
        openMenu,
        closeMenu,
        isOpen
    });
</script>

<template>
    <div 
    v-if="isOpen"
    ref="shareRef"
    class="overlay">
        <div 
            ref="overlayContentRef"
            class="overlay-content"
            @click.stop>
            <p style="color: #F3F0E9;">Ссылка на видео:</p>
            <!-- <input type="text" :value="`https://localhost:5173/video/${videoId}`" readonly /> -->
            <input type="text" :value="`localhost:5173/video/${videoId}`" readonly />
            <div class="buttons-wrapper">
                <button class="control-button" @click.stop="copyLink"><span>Копировать ссылку</span></button>
                <button class="control-button" @click.stop="closeMenu"><span>Закрыть</span></button>
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
        background: #4A4947;
        padding: 20px;
        border-radius: 4px;
        align-items: center;
        text-align: center;
    }

    .overlay-content input {
        box-sizing: border-box;
        width: 100%;
        padding: 8px;
        margin: 8px 0;
        background: #100E0E;
        border: 1px solid #4A4947;
        color: #F3F0E9;
    }

    .overlay-content button {
        padding: 8px 16px;
        background: #100E0E;
        border: none;
        color: #F3F0E9;
        cursor: pointer;
        transition: background 1s ease;
    }

    .overlay-content button span {
        position: relative;
        top: -2px;
    }

    .overlay-content button:hover {
        color: #100E0E;
        background: #F39E60;
    }

    .buttons-wrapper {
        display: flex;
        justify-content: center;
        gap: 0 10px;
    }
</style>