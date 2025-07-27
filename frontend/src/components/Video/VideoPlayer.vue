<script setup>
import { computed, onBeforeUnmount, onMounted, ref, watch, nextTick } from 'vue'
import Hls from 'hls.js'
import VideoSettingsMenu from './VideoSettingsMenu.vue';
import { useVideoStore } from '@/assets/store/videoSettings';

const props = defineProps({
  videoFiles: {
    type: Array,
    required: true
  }
});

const videoStore = useVideoStore();

const videoPlayerRef = ref(null);
const videoContainer = ref(null);
const isFullscreen = ref(false);
const controlPannerVisible = ref(false);
const settingsMenu = ref(null);
const hls = ref(null);
const isPlaying = ref(false);
const currentTime = ref(0);
const videoDuration = ref(0);
const seekBarInput = ref(0.2);
const playerError = ref(null);
const videoDimensions = ref({ width: 0, height: 0 });
const aspectRatio = ref(16/9); // Соотношение по умолчанию 16:9

// Вычисляемые свойства остаются без изменений
const progressBarStyle = computed(() => ({
  background: `linear-gradient(to right, #F39E60 ${(currentTime.value / (videoDuration.value || 1)) * 100}%, #ddd ${(currentTime.value / (videoDuration.value || 1)) * 100}%)`
}));

const volumeBarStyle = computed(() => ({
  background: `linear-gradient(to right, #F39E60 ${seekBarInput.value * 100}%, #F3F0E9 ${seekBarInput.value * 100}%)`
}));

const initHls = async () => {
  console.log('$$Плеер', videoPlayerRef.value, 'hls', Hls.isSupported(), '&&', videoPlayerRef.value && Hls.isSupported());
  
  if (!Hls.isSupported()) {
    console.error('HLS is not supported in this browser');
    return;
  }

  if (!videoPlayerRef.value) {
    console.error('Video element is not available');
    return;
  }

  try {
    console.log('Initializing HLS...');
    
    hls.value = new Hls({
      enableWorker: true,
      debug: true, // Включаем логирование
    });

    // Обработчики событий
    hls.value.on(Hls.Events.MEDIA_ATTACHED, () => {
      console.log('Media attached');
      loadCurrentResolution();
    });

    hls.value.on(Hls.Events.MANIFEST_PARSED, (event, data) => {
      console.log('Manifest parsed, quality levels:', data.levels);
      // Обновляем длительность при парсинге манифеста
      videoDuration.value = videoPlayerRef.value.duration;
    });

    videoPlayerRef.value.addEventListener('loadedmetadata', () => {
      videoDuration.value = videoPlayerRef.value.duration;
    });

    hls.value.on(Hls.Events.ERROR, (event, data) => {
      console.error('HLS Error:', data);
    });

    console.log('Attaching media...');
    hls.value.attachMedia(videoPlayerRef.value);
    
  } catch (error) {
    console.error('HLS initialization failed:', error);
  }
};

// В onMounted:
onMounted(async () => {
  await nextTick(); // Ждём обновления DOM
  console.log('Component mounted, initializing HLS');
  initHls();
});

const loadCurrentResolution = () => {
  try {
    console.log(videoStore.resolution, props.videoFiles[0].resolution)
    const resolution = videoStore.resolution || props.videoFiles[0].resolution;
    const videoFile = props.videoFiles.find(f => f.resolution === +resolution);
    
    if (!videoFile) throw new Error('Resolution not found');
    
    const fullUrl = `${import.meta.env.VITE_MINIO_BASE_URL}/${videoFile.bucket}/${videoFile.fileName}`;
    console.log('Loading:', fullUrl);
    
    if (hls.value) {
      hls.value.loadSource(fullUrl);
    } else if (videoPlayerRef.value?.canPlayType('application/vnd.apple.mpegurl')) {
      videoPlayerRef.value.src = fullUrl;
    }
  } catch (error) {
    console.error('Error loading resolution:', error);
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
    if (videoPlayerRef.value) {
      currentTime.value = videoPlayerRef.value.currentTime;
      // Также обновляем длительность на случай, если она изменилась
      if (videoPlayerRef.value.duration !== videoDuration.value) {
        videoDuration.value = videoPlayerRef.value.duration;
      }
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

const changeUserInput = (event) => {
  seekBarInput.value = event.target.value;
  // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayerRef
  if (videoPlayerRef.value) {
    videoPlayerRef.value.volume = seekBarInput.value;
  }
};

const playerDimensions = () => {
  if (!videoPlayerRef.value || !videoContainer.value) {
    console.log('Элементы еще не доступны');
    return;
  }
  if (videoContainer.value.width < videoContainer.value.height)
  {
    videoDimensions.value.height = videoContainer.value.width / 16 * 9;
  }
  else 
  {
    videoDimensions.value.height = videoContainer.value.height;
  }
  videoDimensions.value.width = videoContainer.value.width;
  
}


const destroyPlayer = () => {
  try {
    // console.log("Destroying player...");
    
    if (hls.value) {
      // console.log("Destroying HLS instance");
      hls.value.detachMedia();
      hls.value.destroy();
      hls.value = null;
    }

    if (videoPlayerRef.value) {
      // console.log("Cleaning up video element");
      videoPlayerRef.value.pause();
      videoPlayerRef.value.removeAttribute('src');
      videoPlayerRef.value.load();
    }
  } catch (error) {
    console.error('Error destroying player:', error);
  }
};

const handleSettingsButtonClick = (event) => {
    event.stopPropagation();
    
    settingsMenu.value?.openMenu(event.currentTarget);
};

watch(() => videoStore.resolution, (newResolution) => {
  loadCurrentResolution();
});

// Следим за изменением скорости
watch(() => videoStore.speed, (newSpeed) => {
  if (videoPlayerRef.value) {
    videoPlayerRef.value.playbackRate = newSpeed;
  }
});

// ИЗМЕНЕНО: Полностью переработан хук onMounted
onMounted(async () => {
  await nextTick();
  initHls();
  playerDimensions();
  
  if (videoPlayerRef.value) {
    videoPlayerRef.value.addEventListener('play', () => isPlaying.value = true);
    videoPlayerRef.value.addEventListener('pause', () => isPlaying.value = false);
    videoPlayerRef.value.addEventListener('timeupdate', updateTime);
    videoPlayerRef.value.addEventListener('durationchange', () => {
      videoDuration.value = videoPlayerRef.value.duration;
    });
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
      :volume="seekBarInput"
      playsinline
    ></video>

    <div v-if="!isPlaying" class="play-overlay">
      <button>▶</button>
    </div>

    <div v-if="playerError" class="error-message">
      {{ playerError }}
    </div>

    <div v-if="controlPannerVisible || !isPlaying" class="controls-overlay" @click.stop >
      <VideoSettingsMenu
        ref="settingsMenu"
        :video-resolution="props.videoFiles"
      />
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
        <div class="control-block">
          <button @click="togglePlay" class="control-button">
            <svg width="17" height="20" style="fill: #F3F0E9 !important;">
              <path style="fill: inherit;" v-if="!isPlaying" d="M17 10 0 20V0l17 10Z" />
              <path style="fill: inherit;" v-if="isPlaying" d="M.5 19.5V.5h2.886v19H.5Zm16 0h-2.886V.5H16.5v19Z"/>
            </svg>
          </button>

          <div class="volume-control">
            <input 
              class="user-set-bar"
              type="range"
              min="0"
              max="1"
              step="0.01"
              :value="seekBarInput"
              @input="changeUserInput"
              :style="volumeBarStyle"
            />
          </div>
        </div>
        <div class="control-block">
          <button @click.stop="handleSettingsButtonClick" class="control-button screen-button">
            <svg width="26" height="26" viewBox="0 0 26 26" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M20.5 13C20.5 17.1421 17.1421 20.5 13 20.5C8.85787 20.5 5.5 17.1421 5.5 13C5.5 8.85786 8.85787 5.5 13 5.5C17.1421 5.5 20.5 8.85786 20.5 13Z" stroke="#F3F0E9" stroke-width="2"/>
              <path d="M15.4001 12.9996C15.4001 14.3251 14.3256 15.3996 13.0001 15.3996C11.6746 15.3996 10.6001 14.3251 10.6001 12.9996C10.6001 11.6741 11.6746 10.5996 13.0001 10.5996C14.3256 10.5996 15.4001 11.6741 15.4001 12.9996Z" stroke="white" stroke-width="2"/>
              <path d="M13.3536 0.646446C13.1583 0.451184 12.8417 0.451184 12.6464 0.646446L9.46447 3.82843C9.2692 4.02369 9.2692 4.34027 9.46447 4.53553C9.65973 4.7308 9.97631 4.7308 10.1716 4.53553L13 1.70711L15.8284 4.53553C16.0237 4.7308 16.3403 4.7308 16.5355 4.53553C16.7308 4.34027 16.7308 4.02369 16.5355 3.82843L13.3536 0.646446ZM13 5.5H13.5V1H13H12.5V5.5H13Z" fill="white"/>
              <path d="M12.6464 25.3536C12.8417 25.5488 13.1583 25.5488 13.3536 25.3536L16.5355 22.1716C16.7308 21.9763 16.7308 21.6597 16.5355 21.4645C16.3403 21.2692 16.0237 21.2692 15.8284 21.4645L13 24.2929L10.1716 21.4645C9.97631 21.2692 9.65973 21.2692 9.46447 21.4645C9.2692 21.6597 9.2692 21.9763 9.46447 22.1716L12.6464 25.3536ZM13 20.5H12.5V25H13H13.5V20.5H13Z" fill="white"/>
              <path d="M25.3536 13.3536C25.5488 13.1583 25.5488 12.8417 25.3536 12.6464L22.1716 9.46447C21.9763 9.2692 21.6597 9.2692 21.4645 9.46447C21.2692 9.65973 21.2692 9.97631 21.4645 10.1716L24.2929 13L21.4645 15.8284C21.2692 16.0237 21.2692 16.3403 21.4645 16.5355C21.6597 16.7308 21.9763 16.7308 22.1716 16.5355L25.3536 13.3536ZM20.5 13V13.5H25V13V12.5H20.5V13Z" fill="white"/>
              <path d="M0.646446 12.6464C0.451184 12.8417 0.451184 13.1583 0.646446 13.3536L3.82843 16.5355C4.02369 16.7308 4.34027 16.7308 4.53553 16.5355C4.7308 16.3403 4.7308 16.0237 4.53553 15.8284L1.70711 13L4.53553 10.1716C4.7308 9.97631 4.7308 9.65973 4.53553 9.46447C4.34027 9.2692 4.02369 9.2692 3.82843 9.46447L0.646446 12.6464ZM5.5 13V12.5H0.999999V13V13.5H5.5V13Z" fill="white"/>
              <path d="M22.0819 4.60025C22.0819 4.3241 21.858 4.10025 21.5819 4.10025H17.0819C16.8057 4.10025 16.5819 4.3241 16.5819 4.60025C16.5819 4.87639 16.8057 5.10025 17.0819 5.10025H21.0819V9.10025C21.0819 9.37639 21.3057 9.60025 21.5819 9.60025C21.858 9.60025 22.0819 9.37639 22.0819 9.10025V4.60025ZM18.3999 7.78223L18.7535 8.13578L21.9354 4.9538L21.5819 4.60025L21.2283 4.24669L18.0463 7.42867L18.3999 7.78223Z" fill="white"/>
              <path d="M4.1 21.5824C4.1 21.8585 4.32386 22.0824 4.6 22.0824H9.1C9.37614 22.0824 9.6 21.8585 9.6 21.5824C9.6 21.3062 9.37614 21.0824 9.1 21.0824H5.1L5.1 17.0824C5.1 16.8062 4.87614 16.5824 4.6 16.5824C4.32386 16.5824 4.1 16.8062 4.1 17.0824V21.5824ZM7.78198 18.4004L7.42843 18.0468L4.24645 21.2288L4.6 21.5824L4.95355 21.9359L8.13554 18.7539L7.78198 18.4004Z" fill="white"/>
              <path d="M21.5819 22.0824C21.858 22.0824 22.0819 21.8585 22.0819 21.5824V17.0824C22.0819 16.8062 21.858 16.5824 21.5819 16.5824C21.3057 16.5824 21.0819 16.8062 21.0819 17.0824V21.0824H17.0819C16.8057 21.0824 16.5819 21.3062 16.5819 21.5824C16.5819 21.8585 16.8057 22.0824 17.0819 22.0824H21.5819ZM18.3999 18.4004L18.0463 18.7539L21.2283 21.9359L21.5819 21.5824L21.9354 21.2288L18.7535 18.0468L18.3999 18.4004Z" fill="white"/>
              <path d="M4.62075 4.05825C4.34461 4.05825 4.12075 4.28211 4.12075 4.55825V9.05825C4.12075 9.3344 4.34461 9.55825 4.62075 9.55825C4.8969 9.55825 5.12075 9.3344 5.12075 9.05825L5.12075 5.05825H9.12075C9.3969 5.05825 9.62075 4.8344 9.62075 4.55825C9.62075 4.28211 9.3969 4.05825 9.12075 4.05825H4.62075ZM7.80273 7.74023L8.15629 7.38668L4.97431 4.2047L4.62075 4.55825L4.2672 4.91181L7.44918 8.09379L7.80273 7.74023Z" fill="white"/>
            </svg>
          </button>
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

.control-block {
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