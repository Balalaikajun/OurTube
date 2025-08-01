<script setup>
import { onMounted } from 'vue'
import KebabButton from '../Kebab/KebabButton.vue'

const props = defineProps({
        playlist: {
            type: Object,
            required: true,
            default: () => ({
            id: 0,
            title: '',
            count: 0})
        }
    });

    const emit = defineEmits(['click', 'kebab-click', 'share', 'delete']);

    const handleCardClick = (e) => {
        e.stopPropagation();
        emit('click', props.playlist);
    };

    const handleKebabButtonClick = (event) => {
        event.stopPropagation();
        // console.log(props.playlist.id, props.playlist.title)
        emit('kebab-click', {
            playlist: {
                playlistId: props.playlist.id,
                playlistTitle: props.playlist.title,
            },
            buttonElement: event.currentTarget
        });
    };

    onMounted(async () => {
        // console.log(props.playlist)
    });
</script>
<template>
    <div 
        class="playlist-card" 
        @click="handleCardClick"
    >
        <div class="filler-block">
            <p>{{ playlist.count }}</p>
        </div>
    
        <div class="bottom-block">
            <div class="playlist-info">
                <h3 class="playlist-title">{{ playlist.title}}</h3>
            </div>
            
            <KebabButton @kebab-click="handleKebabButtonClick" v-if="!['Понравившееся', 'Смотреть позже'].includes(playlist.title)"/>        
        </div>
    </div>
</template>
<style scoped>
    .playlist-card {
        display: block;
        cursor: pointer;
        pointer-events: auto !important;
    }
    .playlist-card:hover .filler-block {
        background: #4A4947;
        transition: background 0.3s ease;
    }
    .playlist-card:hover .filler-block p {
        color: #f39e60;
        transition: background 0.3s ease;
    }
    .filler-block {
        display: flex;
        justify-content: center;
        align-items: center;
        position: relative;
        width: 100%;
        height: auto;
        overflow: hidden;
        aspect-ratio: 16/9;
        background: #f39e60;
    }
    .filler-block p {
        color: #100E0E;
        font-size: 2rem;
        -webkit-text-stroke: 3px currentColor;
    }

    .bottom-block {
        display: flex;
        justify-content: space-between;
        width: 100%;
        height: calc(2em * 1.4);
        line-height: 1.4;
    }

    .playlist-info {
        display: flex;
        margin-top: 1vh;
        width: 80%;
        flex-direction: column;
        color: #f3f0e9;
        overflow: hidden;
        word-wrap: break-word;
        white-space: normal;
    }

    .playlist-card.row-layout .playlist-info {
        width: 100%;
        margin-top: 0;
    }

    .playlist-title {
        display: -webkit-box;
        -webkit-line-clamp: 1;
        -webkit-box-orient: vertical;
        overflow: hidden;
        text-overflow: ellipsis;
        text-indent: 10px;
        
        line-height: 1.4em;
        max-height: 1.4em;
        word-break: break-word;
    }
</style>