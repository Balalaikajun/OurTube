<script setup>
    const emit = defineEmits(["close"]);

    const playlists = ref([
    { id: 1, name: "Плейлист 1" },
    { id: 2, name: "Плейлист 2" },
    ]);

    const addToPlaylist = (playlistId) => {
    alert(`Добавлено в плейлист ${playlistId}`);
    emit("close");
    };

    const createNewPlaylist = () => {
    const newPlaylistName = prompt("Введите название нового плейлиста");
    if (newPlaylistName) {
        playlists.value.push({ id: playlists.value.length + 1, name: newPlaylistName });
    }
    };
</script>

<template>
    <div class="overlay">
        <div class="overlay-content">
        <h3>Добавить в плейлист</h3>
        <ul>
            <li v-for="playlist in playlists" :key="playlist.id">
            <button @click="addToPlaylist(playlist.id)">{{ playlist.name }}</button>
            </li>
        </ul>
        <button @click="createNewPlaylist">Создать новый плейлист</button>
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

    .overlay-content ul {
        list-style: none;
        padding: 0;
    }

    .overlay-content li {
        margin: 8px 0;
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