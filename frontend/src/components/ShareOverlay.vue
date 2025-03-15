<script setup>
    const emit = defineEmits(["close"]);

    const copyLink = () => {
    const link = "https://example.com/video"; // Замените на реальную ссылку
    navigator.clipboard.writeText(link);
    alert("Ссылка скопирована!");
    emit("close");
    };
</script>

<template>
    <div class="overlay">
        <div class="overlay-content">
        <h3>Поделиться</h3>
        <p>Ссылка на видео:</p>
        <input type="text" value="https://example.com/video" readonly />
        <button @click="copyLink">Копировать ссылку</button>
        <button @click="$emit('close')">Закрыть</button>
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
        background: #333;
        padding: 20px;
        border-radius: 8px;
        text-align: center;
    }

    .overlay-content input {
        width: 100%;
        padding: 8px;
        margin: 8px 0;
        background: #444;
        border: 1px solid #555;
        color: #f3f0e9;
    }

    .overlay-content button {
        margin: 8px;
        padding: 8px 16px;
        background: #555;
        border: none;
        color: #f3f0e9;
        cursor: pointer;
    }

    .overlay-content button:hover {
        background: #666;
    }
</style>