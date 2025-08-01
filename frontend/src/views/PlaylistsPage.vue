<script setup>
import { onMounted, ref } from 'vue'

import MasterHead from '../components/Solid/MasterHead.vue'
import ConfirmPannel from '@/components/Solid/ConfirmPannel.vue'
import PlaylistsPresentation from '@/components/Playlist/PlaylistsPresentation.vue'

const confirmRef = ref(null);
    const confirmContext = ref("")
    const playlistsRef = ref(null);
    const currentPlaylistId = ref(null); 

    const handleRenamePlaylist = async() => {

    }
    const handleDeletePlaylist = async(playlistId) => {
        confirmContext.value = "Удаление плейлиста";
        currentPlaylistId.value = playlistId;
        confirmRef.value.openMenu();
    }
    const handleConfirmDelete = () => {
        if (playlistsRef.value) {
            playlistsRef.value.deletePlaylist(currentPlaylistId.value);
        }
    };

    onMounted(async () => {
        // document.title = "Страница пользователя";
    });
</script>
<template>
    <ConfirmPannel
        ref="confirmRef" 
        :action="confirmContext"
        @confirm="handleConfirmDelete"
    />
    <PlaylistsPresentation
        ref="playlistsRef"
        :is-infinite-scroll="true"
        @rename="handleRenamePlaylist"
        @delete="handleDeletePlaylist"
    />
</template>
<style scoped>

</style>