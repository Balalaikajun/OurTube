<script setup>
    import { ref, onMounted, onUnmounted } from "vue";
    

    const props = defineProps({
    videoId: {
        type: String,
        required: true,
    },
    buttonRef: {
        type: HTMLElement,
        required: true,
    },
    });

    const isMenuOpen = ref(false);
    const menuPosition = ref({ top: 0, left: 0 });

    const calculateMenuPosition = () => {
    if (props.buttonRef) {
        const rect = props.buttonRef.getBoundingClientRect();
        menuPosition.value = {
        top: rect.bottom + window.scrollY,
        left: rect.left + window.scrollX,
        };
    }
    };

    const toggleMenu = () => {
    isMenuOpen.value = !isMenuOpen.value;
    if (isMenuOpen.value) {
        calculateMenuPosition();
    }
    };

    const handleClickOutside = (event) => {
    if (isMenuOpen.value && !event.target.closest(".kebab-menu")) {
        isMenuOpen.value = false;
    }
    };

    onMounted(() => {
    document.addEventListener("click", handleClickOutside);
    });

    onUnmounted(() => {
    document.removeEventListener("click", handleClickOutside);
    });

    // Экспортируем toggleMenu, чтобы её можно было использовать в VideoCard.vue
    defineExpose({ toggleMenu });
</script>

<template>
    <div class="kebab-menu-container">
        <!-- Меню -->
        <div
        v-if="isMenuOpen"
        class="kebab-menu"
        :style="{ top: `${menuPosition.top}px`, left: `${menuPosition.left}px` }"
        >
        <button>Добавить в плейлист</button>
        <button>Смотреть позже</button>
        <button>Поделиться</button>
        </div>
    </div>
</template>

<style scoped>
    .kebab-menu-container {
    position: relative;
    }

    .kebab-menu {
    position: absolute;
    background: #333;
    border: 1px solid #555;
    border-radius: 4px;
    padding: 8px;
    z-index: 1000;
    display: flex;
    flex-direction: column;
    gap: 8px;
    }

    .kebab-menu button {
    background: none;
    border: none;
    color: #f3f0e9;
    cursor: pointer;
    padding: 4px 8px;
    text-align: left;
    }

    .kebab-menu button:hover {
    background: #555;
    }
</style>