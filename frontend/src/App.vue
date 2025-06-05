<script setup>
    import { ref, onMounted, onUnmounted, watch, nextTick, toRef } from "vue";
    const token = localStorage.getItem("token")
    onMounted(
        () =>
        {
            console.log("Token ",token)
        }
    )
</script>

<template>
    <router-view></router-view>
</template>

<style scoped>
    
</style>