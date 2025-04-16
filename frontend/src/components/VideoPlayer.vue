<script setup>
import { ref, onMounted, onBeforeUnmount, computed } from 'vue';
import Hls from 'hls.js';

const props = defineProps({
  videoSrc: {
    type: String,
    required: true
  }
});

const videoPlayer = ref(null);
const isPlaying = ref(false);
const currentTime = ref(0);
const videoDuration = ref(0);
const volume = ref(0.2);
const hls = ref(null);
const playerError = ref(null);
const userInteracted = ref(false);

// Вычисляемые свойства остаются без изменений
const progressBarStyle = computed(() => ({
  background: `linear-gradient(to right, #F39E60 ${(currentTime.value / (videoDuration.value || 1)) * 100}%, #ddd ${(currentTime.value / (videoDuration.value || 1)) * 100}%)`
}));

const volumeBarStyle = computed(() => ({
  background: `linear-gradient(to right, #F39E60 ${volume.value * 100}%, #F3F0E9 ${volume.value * 100}%)`
}));

const initHls = () => {
  if (!props.videoSrc) {
    playerError.value = 'Источник видео не указан';
    return;
  }

  try {
    debugger;
    if (Hls.isSupported() && videoPlayer.value) {
      debugger;
      hls.value = new Hls({
        xhrSetup: function(xhr, url) {
          // Добавляем http:// если отсутствует
          const fullUrl = url.startsWith('http') ? url : `http://${url}`;
          debugger;
          xhr.open('GET', fullUrl, true);
        }
      });
      debugger;
      hls.value.loadSource(props.videoSrc);
      debugger;
      hls.value.attachMedia(videoPlayer.value);

      // ДОБАВЛЕНО: Обработчик для события загрузки метаданных
      hls.value.on(Hls.Events.MANIFEST_PARSED, () => {
        videoPlayer.value.addEventListener('loadedmetadata', () => {
          // Обновляем длительность видео при загрузке метаданных
          videoDuration.value = videoPlayer.value.duration;
        });
      });

      hls.value.on(Hls.Events.ERROR, (event, data) => {
        if (data.fatal) {
          switch(data.type) {
            case Hls.ErrorTypes.NETWORK_ERROR:
              playerError.value = 'Ошибка сети при загрузке видео';
              break;
            case Hls.ErrorTypes.MEDIA_ERROR:
              playerError.value = 'Ошибка медиа-данных';
              break;
            default:
              playerError.value = 'Неизвестная ошибка воспроизведения';
              break;
          }
        }
      });
    } 
    else if (videoPlayer.value?.canPlayType('application/vnd.apple.mpegurl')) {
      // Для Safari
      videoPlayer.value.src = props.videoSrc;
      // ДОБАВЛЕНО: Обработчик метаданных для Safari
      videoPlayer.value.addEventListener('loadedmetadata', () => {
        videoDuration.value = videoPlayer.value.duration;
      });
    } 
    else {
      playerError.value = 'Ваш браузер не поддерживает воспроизведение HLS';
    }
  } catch (error) {
    playerError.value = 'Ошибка инициализации плеера';
    console.error('HLS init error:', error);
  }
};

const handlePlayClick = async () => {
  try {
    // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayer
    if (videoPlayer.value) {
      await videoPlayer.value.play();
      userInteracted.value = true;
      isPlaying.value = true;
    }
  } catch (error) {
    playerError.value = 'Не удалось начать воспроизведение';
  }
};

const togglePlay = () => {
  // ИЗМЕНЕНО: Улучшена проверка на существование videoPlayer
  if (!videoPlayer.value) return;
  
  if (isPlaying.value) {
    videoPlayer.value.pause();
  } else {
    videoPlayer.value.play().catch(e => {
      playerError.value = 'Требуется взаимодействие пользователя';
    });
  }
  isPlaying.value = !isPlaying.value;
};

const seek = (event) => {
  // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayer
  if (videoPlayer.value) {
    videoPlayer.value.currentTime = event.target.value;
  }
};

const changeVolume = (event) => {
  volume.value = event.target.value;
  // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayer
  if (videoPlayer.value) {
    videoPlayer.value.volume = volume.value;
  }
};

// ИЗМЕНЕНО: Полностью переработан хук onMounted
onMounted(() => {
  initHls();
  
  // Выносим обработчик в отдельную функцию для последующего удаления
  const updateTime = () => {
    // Добавляем проверку на существование videoPlayer
    if (videoPlayer.value) {
      currentTime.value = videoPlayer.value.currentTime;
    }
  };
  
  // Добавляем обработчик только если videoPlayer существует
  if (videoPlayer.value) {
    videoPlayer.value.addEventListener('timeupdate', updateTime);
  }
  
  // ДОБАВЛЕНО: Правильное удаление обработчиков при размонтировании
  onBeforeUnmount(() => {
    if (videoPlayer.value) {
      videoPlayer.value.removeEventListener('timeupdate', updateTime);
    }
    if (hls.value) {
      hls.value.destroy();
    }
  });
});
</script>

<template>
  <div class="video-container" @click="handlePlayClick">
    <video 
      ref="videoPlayer" 
      class="player"
      :volume="volume"
      playsinline
    ></video>

    <div v-if="!userInteracted" class="play-overlay">
      <button @click.stop="handlePlayClick">▶</button>
    </div>

    <div v-if="playerError" class="error-message">
      {{ playerError }}
    </div>

    <div class="controls-overlay" @click.stop>
      <div class="progress-bar-container">
        <input 
          class="seek-bar"
          type="range"
          min="0"
          :max="videoDuration"
          :value="currentTime"
          @input="seek"
          :style="progressBarStyle"
        />
      </div>

      <div class="control-panel">
        <button @click="togglePlay" class="control-button">
          <svg width="17" height="20" fill="#F3F0E9">
            <path v-if="!isPlaying" d="M17 10 0 20V0l17 10Z" />
            <path v-if="isPlaying" d="M.5 19.5V.5h2.886v19H.5Zm16 0h-2.886V.5H16.5v19Z"/>
          </svg>
        </button>

        <div class="volume-control">
          <input 
            class="volume-bar"
            type="range"
            min="0"
            max="1"
            step="0.01"
            :value="volume"
            @input="changeVolume"
            :style="volumeBarStyle"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.video-container {
  position: relative;
  width: 880px;
  height: 510px;
  background: #4A4947;
}

.player {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.play-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  background: rgba(0,0,0,0.5);
}

.play-overlay button {
  background: rgba(255,255,255,0.2);
  border: none;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  font-size: 24px;
  color: white;
  cursor: pointer;
}

.error-message {
  position: absolute;
  top: 10px;
  left: 10px;
  color: #ff4d4f;
  background: rgba(0,0,0,0.7);
  padding: 5px 10px;
  border-radius: 4px;
}

.controls-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(to top, rgba(0,0,0,0.7), transparent);
  padding: 10px 15px;
}

.seek-bar {
  width: 100%;
  height: 5px;
  -webkit-appearance: none;
  appearance: none;
  cursor: pointer;
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

.volume-bar {
  width: 80px;
  height: 5px;
  -webkit-appearance: none;
  appearance: none;
  cursor: pointer;
}

.volume-bar::-webkit-slider-thumb {
  -webkit-appearance: none;
  appearance: none;
  width: 12px;
  height: 12px;
  background: #F39E60;
  border-radius: 50%;
  cursor: pointer;
}

.control-panel {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 5px;
}

.volume-control {
  display: flex;
  align-items: center;
}
</style>