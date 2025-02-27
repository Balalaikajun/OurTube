<script setup>
    import { ref, computed, onMounted } from "vue";

    const videoPlayer = ref(null);
    const isPlaying = ref(false);
    const currentTime = ref(0);
    const videoDuration = ref(0);
    const volume = ref(0.2);

    onMounted(() => {
    if (videoPlayer.value) {
        videoPlayer.value.onloadedmetadata = () => {
        videoDuration.value = videoPlayer.value.duration;
        };
    }

    setInterval(() => {
        if (videoPlayer.value && isPlaying.value) {
        currentTime.value = videoPlayer.value.currentTime;
        }
    }, 1000);
    });

    const togglePlay = () => {
        if (videoPlayer.value) {
            if (isPlaying.value) {
            videoPlayer.value.pause();
            } else {
            videoPlayer.value.play();
            }
            isPlaying.value = !isPlaying.value;
        }
    };

    const seek = (event) => {
        if (videoPlayer.value) {
            videoPlayer.value.currentTime = event.target.value;
        }
    };

    const changeVolume = (event) => {
        volume.value = event.target.value;
        if (videoPlayer.value) {
            videoPlayer.value.volume = volume.value;
        }
    };

    const progressBarStyle = computed(() => ({
        background: `linear-gradient(to right, #F39E60 ${(currentTime.value / videoDuration.value) * 100}%, #ddd ${(currentTime.value / videoDuration.value) * 100}%)`
    }));

    const progressStyle = computed(() => ({
        background: `linear-gradient(to right, #F39E60 ${volume.value * 100}%, #F3F0E9 ${volume.value * 100}%)`,
    }));
</script>

<template>
    <div class="video-container">
        <video ref="videoPlayer" class="player">
            <source src="/videos/1669155855826539402_640x400.mp4" type="video/mp4">
            Ваш браузер не поддерживает видео.
        </video>

        <div class="bottom-container">
            <div class="progress-bar-container">
            <input 
                class="seek-bar"
                type="range"
                min="0"
                :max="videoDuration"
                v-model="currentTime"
                @input="seek"
                :style="progressBarStyle"
                />
            </div>

            <div class="control-panel">
                <button @click="togglePlay" class="control-button">
                    <svg v-if="!isPlaying" width="17" height="20" fill="#F3F0E9">
                        <path class="button-fill" d="M17 10 0 20V0l17 10Z" />
                    </svg>
                    <svg v-if="isPlaying" width="17" height="20" fill="#F3F0E9">
                        <path fill="#F3F0E9" stroke="#F3F0E9" d="M.5 19.5V.5h2.886v19H.5Zm16 0h-2.886V.5H16.5v19Z"/>
                    </svg>
                </button>

                <div class="volume">
                    <button class="control-button">
                        <svg width="13" height="20">
                        <path fill="#F3F0E9" stroke="#F3F0E9" stroke-width="2"
                            d="m6.326 5.122 4.735-2.809-2.76 7.335-.133.352.133.352 2.813 7.475-4.818-2.639-.123-.067-.137-.03L1 13.954V5.811l4.928-.556.213-.024.185-.11Z" />
                        </svg>
                    </button>

                    <div class="volume-bar">
                    <input 
                        type="range"
                        min="0"
                        max="1"
                        step="0.01"
                        v-model="volume"
                        @input="changeVolume"
                        :style="progressStyle"
                    />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .video-container {
        display: flex;
        top: 100px;
        left: 50px;
        position: relative;
        flex-direction: column;
        width: 880px;
        height: 510px;
    }

    .player {
        width: 880px;
        height: 510px;
        background: #4A4947;
    }

    .bottom-container {
        position: absolute;
        bottom: 0;
        left: 12px;
        width: calc(100% - 24px);
        background: transparent;
    }

    .progress-bar {
        width: 100%;
        height: 3px;
        background: #F39E60;
    }

    .control-panel {
        display: flex;
        flex-wrap: wrap;
        flex-direction: row;
        width: 100%;
        height: 50px;
        background: transparent;
        align-content: center;
    }

    .control-button {
        display: flex;
        border: 0;
        padding: 0;
        margin: 0 15px;
        width: fit-content;
        height: fit-content;
        cursor: pointer;
        background: transparent;
    }

    .volume {
        display: flex;
        flex-direction: row;
        justify-content: center;
    }

    .volume-bar {
        display: none;
        align-items: center; /* Вертикальное выравнивание ползунка */
    }

    .volume:hover .volume-bar
    {
        display: flex;
        align-items: center;
    }

    .volume-bar input {
        display: flex;
        width: 90px;
        height: 5px;
        cursor: pointer;
        -webkit-appearance: none;
        appearance: none;
        background: linear-gradient(to right, #F39E60 var(--progress), #F3F0E9 var(--progress));
        border-radius: 0px;
        outline: none;
        cursor: pointer;
    }

    .seek-bar input {
        display: flex;
        width: 100%;
        height: 5px;
        cursor: pointer;
        -webkit-appearance: none;
        appearance: none;
        background: linear-gradient(to right, #F39E60 var(--progress), #F3F0E9 var(--progress));
        border-radius: 0px;
        outline: none;
        cursor: pointer;
    }

    .seek-bar {
        width: 100%;
        height: 3px;
        cursor: pointer;
        -webkit-appearance: none;
        appearance: none;
        background: #F3F0E9; /* Вставка фонового цвета */
        outline: none;
    }

    .seek-bar::-webkit-slider-runnable-track {
        background: transparent;
    }

    .seek-bar::-webkit-slider-thumb {
        -webkit-appearance: none;
        appearance: none;
        width: 12px;
        height: 12px;
        background: #F39E60;
        border-radius: 50%;
        cursor: pointer;
    }

    .volume-bar input::-webkit-slider-runnable-track {
        background: transparent;
    }

    .volume-bar input::-webkit-slider-thumb {
        -webkit-appearance: none;
        appearance: none;
        width: 12px;
        height: 12px;
        background: #F39E60;
        border-radius: 50%;
        cursor: pointer;
    }
</style>