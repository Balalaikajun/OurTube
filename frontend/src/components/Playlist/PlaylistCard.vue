<script setup>
    import { ref, onMounted, onUnmounted, computed, nextTick, provide, watch  } from "vue";
    import { useRoute } from "vue-router";
    import KebabButton from "../Kebab/KebabButton.vue";

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
        console.log(props.playlist.id, props.playlist.title)
        emit('kebab-click', {
            playlist: {
                playlistId: props.playlist.id,
                playlistTitle: props.playlist.title,
            },
            buttonElement: event.currentTarget
        });
    };

    onMounted(async () => {
        console.log(props.playlist)
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
            
            <KebabButton @kebab-click="handleKebabButtonClick"/>        
        </div>
    </div>
</template>
<style scoped>
    .playlist-card {
        display: block;
        cursor: pointer;
        pointer-events: auto !important;
        /* border: 1px solid #f39e60; */
    }
    .playlist-card:hover .filler-block {
        background: #4A4947; /* Или любой другой цвет, который вы хотите использовать */
        transition: background 0.3s ease; /* Плавное изменение цвета */
    }
    .playlist-card:hover .filler-block p {
        color: #f39e60;
        transition: background 0.3s ease; /* Плавное изменение цвета */
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
    /* .video-card.row-layout .video-block {
      flex-grow: 1;
      flex-shrink: 2;
      flex-basis: 30%;
      flex-shrink: 0;
    }     */
    /* width: 20%; */
    .bottom-block {
        display: flex;
        justify-content: space-between;
        width: 100%;
        height: calc(2em * 1.4);
        line-height: 1.4;
    }

    /* .video-card.row-layout .bottom-block {
        flex-grow: 2;
        flex-shrink: 1;
        flex-basis: 70%;
        justify-content: flex-start;
        gap: 10px;
    } */
    /* flex-direction: row; */

    /* .video-card.row-layout .video-block {
        width: 20vw;
        flex-shrink: 0;
    } */

    /* .thumbnail-overlay-badge {
        position: absolute;
        bottom: 5px;
        right: 5px;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 3px;
        padding: 2px 4px;
        z-index: 1;
        color: #f3f0e9;
        font-size: 12px;
    }

    .video-thumbnail {
        display: block;
        width: 100%;
        height: 100%;
        object-fit: cover;
        background: #f39e60;
    } */

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
        /* margin: 0;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-line-clamp: 1;
        -webkit-box-orient: vertical;
        line-height: 1.4;
        max-height: calc(1 * 1.4em);
        word-break: break-word; */

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

    /* .video-card.row-layout .video-title {
        font-size: 18px;
        -webkit-line-clamp: 1;
        max-height: 1.4em;
    } */

    /* .video-stats {
        display: flex;
        gap: 10px;
        margin-top: 6px;
        font-size: 12px;
        color: #F3F0E9;
    } */

    /* .channel-info {
        display: flex;
        align-items: center;
        gap: 10px;
        margin-top: 12px;
    }

    .channel-avatar {
        width: 36px;
        height: 36px;
        border-radius: 50%;
        object-fit: cover;
    }

    .avatar-placeholder {
        width: 36px;
        height: 36px;
        border-radius: 50%;
        background: #4A4947;
    } */
</style>