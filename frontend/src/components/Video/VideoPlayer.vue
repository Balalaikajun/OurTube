<script setup>
import { computed, onBeforeUnmount, onMounted, ref } from 'vue'
import Hls from 'hls.js'

const props = defineProps({
  videoSrc: {
    type: String,
    required: true
  }
});

const videoPlayerRef = ref(null);
const videoContainer = ref(null);
const isFullscreen = ref(false);
const controlPannerVisible = ref(false);
const hls = ref(null);
const isPlaying = ref(false);
const currentTime = ref(0);
const videoDuration = ref(0);
const volume = ref(0.2);
const playerError = ref(null);
const videoDimensions = ref({ width: 0, height: 0 });
const aspectRatio = ref(16/9); // Соотношение по умолчанию 16:9

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
    if (Hls.isSupported() && videoPlayerRef.value) {
      hls.value = new Hls({
        xhrSetup: function(xhr, url) {
        // Проверяем, является ли URL относительным
        if (!url.startsWith('http')) {
          // Получаем базовый путь из videoSrc (удаляем имя файла)
          const basePath = props.videoSrc.substring(0, props.videoSrc.lastIndexOf('/') + 1);
          // Собираем полный URL
          const fullUrl = `${basePath}${url}`;
          xhr.open('GET', fullUrl, true);
        } else {
          xhr.open('GET', url, true);
        }
      }
      });
      hls.value.loadSource(props.videoSrc);
      hls.value.attachMedia(videoPlayerRef.value);

      // ДОБАВЛЕНО: Обработчик для события загрузки метаданных
      hls.value.on(Hls.Events.MANIFEST_PARSED, () => {
        videoPlayerRef.value.addEventListener('loadedmetadata', () => {
          // Обновляем длительность видео при загрузке метаданных
          videoDuration.value = videoPlayerRef.value.duration;
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
    else if (videoPlayerRef.value?.canPlayType('application/vnd.apple.mpegurl')) {
      // Для Safari
      videoPlayerRef.value.src = props.videoSrc;
      // ДОБАВЛЕНО: Обработчик метаданных для Safari
      videoPlayerRef.value.addEventListener('loadedmetadata', () => {
        videoDuration.value = videoPlayerRef.value.duration;
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
const showControlPannel = async () => {
  controlPannerVisible.value = true;

  if (window.controlPanelTimeout) {
    clearTimeout(window.controlPanelTimeout);
  }

  window.controlPanelTimeout = setTimeout(() => {
    controlPannerVisible.value = false;
  }, 2000);
}

const toggleFullscreen = () => {
  if (!document.fullscreenElement) {
    videoContainer.value.requestFullscreen()
      .then(() => {
        isFullscreen.value = true;
        videoContainer.value.classList.add('fullscreen');
      })
      .catch(err => {
        console.error('Error attempting to enable fullscreen:', err);
      });
  } else {
    document.exitFullscreen()
      .then(() => {
        isFullscreen.value = false;
        videoContainer.value.classList.remove('fullscreen');
      });
  }
};

const handleFullscreenChange = () => {
  isFullscreen.value = !!document.fullscreenElement;
};


  // Выносим обработчик в отдельную функцию для последующего удаления
  const updateTime = () => {
    // Добавляем проверку на существование videoPlayerRef
    if (videoPlayerRef.value) {
      currentTime.value = videoPlayerRef.value.currentTime;
    }
  };

const togglePlay = async () => {
  if (!videoPlayerRef.value) return;
  
  try {
    if (videoPlayerRef.value.paused) {
      await videoPlayerRef.value.play().catch(e => {
        throw e; // Перебрасываем ошибку для обработки
      });
      isPlaying.value = true;
    } else {
      videoPlayerRef.value.pause();
      isPlaying.value = false;
    }
  } catch (error) {
    console.error('Ошибка воспроизведения:', error);
    playerError.value = 'Ошибка воспроизведения. Кликните для повторной попытки.';
  }
};

const seek = (event) => {
  // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayerRef
  if (videoPlayerRef.value) {
    videoPlayerRef.value.currentTime = event.target.value;
  }
};

const changeVolume = (event) => {
  volume.value = event.target.value;
  // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayerRef
  if (videoPlayerRef.value) {
    videoPlayerRef.value.volume = volume.value;
  }
};


const destroyPlayer = () => {
  try {
    console.log("Destroying player...");
    
    if (hls.value) {
      console.log("Destroying HLS instance");
      hls.value.detachMedia();
      hls.value.destroy();
      hls.value = null;
    }

    if (videoPlayerRef.value) {
      console.log("Cleaning up video element");
      videoPlayerRef.value.pause();
      videoPlayerRef.value.removeAttribute('src');
      videoPlayerRef.value.load();
    }
  } catch (error) {
    console.error('Error destroying player:', error);
  }
};
// ИЗМЕНЕНО: Полностью переработан хук onMounted
onMounted(() => {
  initHls(); // HLS сам управляет источниками
  
  if (videoPlayerRef.value) {
    videoPlayerRef.value.addEventListener('play', () => isPlaying.value = true);
    videoPlayerRef.value.addEventListener('pause', () => isPlaying.value = false);
    videoPlayerRef.value.addEventListener('timeupdate', updateTime);
    // videoPlayerRef.value.addEventListener('loadedmetadata', updateVideoDimensions);
  }
  
  document.addEventListener('fullscreenchange', handleFullscreenChange);
});

onBeforeUnmount(() => {
  destroyPlayer(); // Вызываем метод очистки
  
  if (videoPlayerRef.value) {
    videoPlayerRef.value.removeEventListener('play', () => isPlaying.value = true);
    videoPlayerRef.value.removeEventListener('pause', () => isPlaying.value = false);
    videoPlayerRef.value.removeEventListener('timeupdate', updateTime);
  }
  document.removeEventListener('fullscreenchange', handleFullscreenChange);
});

defineExpose({
  destroyPlayer,
  toggleFullscreen,
  togglePlay,
  videoPlayerRef
});
</script>

<template>
  <div ref="videoContainer" class="video-container" 
    @click="togglePlay" @mousemove="showControlPannel">
    <video 
      ref="videoPlayerRef" 
      class="player"      
      :volume="volume"
      playsinline
    ></video>

    <div v-if="!isPlaying" class="play-overlay">
      <button>▶</button>
    </div>

    <div v-if="playerError" class="error-message">
      {{ playerError }}
    </div>

    <div v-if="controlPannerVisible || !isPlaying" class="controls-overlay" @click.stop >
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
        <div class="left-block">
          <button @click="togglePlay" class="control-button">
            <svg width="17" height="20" style="fill: #F3F0E9 !important;">
              <path style="fill: inherit;" v-if="!isPlaying" d="M17 10 0 20V0l17 10Z" />
              <path style="fill: inherit;" v-if="isPlaying" d="M.5 19.5V.5h2.886v19H.5Zm16 0h-2.886V.5H16.5v19Z"/>
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
        <div class="right-block">
          <button @click="toggleFullscreen" class="control-button screen-button">
              <div class="corner-1"></div>
              <div class="corner-2"></div>
              <div class="corner-3"></div>
              <div class="corner-4"></div>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.video-container {
  position: relative;
  width: 100%;
}

.video-container:fullscreen {
  width: 100vw !important;
  height: 100vh !important;
  background: black;
  display: flex;
  justify-content: center;
  align-items: center;
}

.left-block {
  display: flex;
  height: 100%;
  flex-direction: row;
  align-content: center;
  flex-wrap: wrap;
  column-gap: 1vw;
}

.player {
  width: 100%;
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
  color: #F3F0E9;
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
  padding: 0px 15px 10px;
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

.screen-button {
  position: relative;
  background: transparent;
  cursor: pointer;
  width: 24px;
  height: 24px;
}

.screen-button div {
  position: absolute;
  box-sizing: border-box;
  width: 6px;
  height: 6px;
}

.corner-1 {
  top: 0;
  left: 0;
  border-left: 2px solid #F3F0E9;
  border-top: 2px solid #F3F0E9;
  border-bottom: 0 none transparent;
  border-right: 0 none transparent;
}
.corner-2 {
  top: 0;
  right: 0;
  border-left: 0 none transparent;
  border-top: 2px solid #F3F0E9;
  border-bottom: 0 none transparent;
  border-right: 2px solid #F3F0E9;
}
.corner-3 {
  bottom: 0;
  left: 0;
  border-left: 2px solid #F3F0E9;
  border-top: 0 none transparent;
  border-bottom: 2px solid #F3F0E9;
  border-right: 0 none transparent;
}
.corner-4 {
  bottom: 0;
  right: 0;
  border-left: 0 none transparent;
  border-top: 0 none transparent;
  border-bottom: 2px solid #F3F0E9;
  border-right: 2px solid #F3F0E9;
} 
</style>