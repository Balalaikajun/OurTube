<script setup>
    import { ref, onMounted, onUnmounted } from "vue";
    import MainMenu from "./MainMenu.vue"; // Импортируем компонент бокового меню

    const isSideMenuVisible = ref(false);

    const toggleSideMenu = () => {
        isSideMenuVisible.value = !isSideMenuVisible.value;
    };
    const handleClickOutside = (event) => {
        // const sideMenu = document.querySelector(".side-menu");
        if (isSideMenuVisible.value && !event.target.closest(".side-menu") && !event.target.closest("button")) 
        {
            isSideMenuVisible.value = false;
        }
    };

    onMounted(() => {
        document.addEventListener("click", handleClickOutside);
    });

    onUnmounted(() => {
        document.removeEventListener("click", handleClickOutside);
    });
</script>

<template>
    <div class="master-head-block" @click="toggleSideMenu">
        <button v-on:click="">
            <span></span>
            <span></span>
            <span></span>
        </button>
    </div>
    
    <MainMenu v-if="isSideMenuVisible"/>
</template>

<style scoped>
    .master-head-block {
        position: fixed;
        top: 0;
        height: 70px;
        align-content: center;
        padding: 0 25px;
        width: 100%; /* Чтобы блок занимал всю ширину */
        z-index: -1; /* Чтобы блок был поверх других элементов */
    }
    button {
        display: flex;
        flex-direction: column;
        gap: 15px;
        cursor: pointer;
        background: inherit;
    }
    button span {
        width: 50px;
        height: 2px;
        background-color: #F3F0E9;
    }
</style>