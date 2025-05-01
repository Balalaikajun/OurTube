<script setup>
import { ref, onMounted, onUnmounted, computed, nextTick } from "vue";
import { useRoute } from "vue-router";
import MasterHead from "../components/MasterHead.vue";
import VideoPlayer from "@/components/VideoPlayer.vue";
import LoadingState from "@/components/LoadingState.vue"; // Импортируем компонент загрузки
import ShareOverlay from "@/components/ShareOverlay.vue";
import CreateCommentBlock from "@/components/CreateCommentBlock.vue";
import { API_BASE_URL } from "@/assets/config.js";
import { MINIO_BASE_URL } from "@/assets/config.js";
import formatter from "@/assets/utils/formatter.js";

const route = useRoute();
const videoPage = ref(null);
const videoId = route.params.id;
const Player = ref(null);
const addComment = ref(null);
const videoData = ref(null);
const hlsUrl = ref("");
const isLoading = ref(true); // Добавляем состояние загрузки
const error = ref(null); // Добавляем обработку ошибок
const shareRef = ref(null);

const showFullDescription = ref(false);
const isDescriptionClamped = ref(false);

const ensureHttpUrl = (url) => {
  if (!url) return '';
  return url.startsWith('http') ? url : `http://${url}`;
};

const handleKeyDown = (e) => {
  if(addComment.value.showButtons) return;
  if (e.code === 'KeyF') {
    e.preventDefault(); // Вызываем на событии (e), а не на videoPage
    Player.value?.toggleFullscreen();
  } else if (e.code === 'Space') {
    e.preventDefault(); // Вызываем на событии (e)
    Player.value?.togglePlay();
  }
};

const fetchVideoData = async () => {
  isLoading.value = true;
  error.value = null;
  
  try {
    const response = await fetch(`${API_BASE_URL}/api/Video/${videoId}`);
    if (!response.ok) throw new Error("Ошибка загрузки видео");
    
    const data = await response.json();
    videoData.value = data;
    
    if (data.files?.length) {
      // Используем реальные данные из API
      const file = data.files[0];
      hlsUrl.value = ensureHttpUrl(`${MINIO_BASE_URL}/videos/${file.fileName}`);
    }
    nextTick(() => {
      checkTextOverflow();
    });
  } catch (err) {
    error.value = err.message;
    console.error("Ошибка загрузки данных видео:", err);
  } finally {
    isLoading.value = false;
  }
  console.log(videoData.value)
};

const handleShareClick = () => {
    // Проверяем, что ссылка существует и имеет метод
    if (shareRef.value && typeof shareRef.value.openMenu === 'function') {
    shareRef.value.openMenu();
  } else {
    console.error('ShareOverlay ref is not properly set or missing openMenu method');
  }
};

const checkTextOverflow = () => {
  nextTick(() => {
    const descElement = document.querySelector('.video-description');
    if (descElement) {
      isDescriptionClamped.value = descElement.scrollHeight > descElement.clientHeight;
    }
  });
};

onMounted(() => {
  fetchVideoData();
  document.addEventListener('keydown', handleKeyDown);
});
onUnmounted(() => {
  if (Player.value) {
    Player.value.destroyPlayer();
    document.removeEventListener('keydown', handleKeyDown);
  }
});
</script>

<template ref="videoPage">
  <MasterHead />
  <ShareOverlay
    ref="shareRef" 
    :videoId="videoId"
  />
  <main class="video-page">
    <LoadingState v-if="isLoading" />
    
    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    
    <template v-else>
      <div class="content-wrapper">
        <section>
          <VideoPlayer
            ref="Player"
            v-if="hlsUrl" 
            :video-src="hlsUrl" 
            :poster="videoData?.thumbnailUrl"
          />
          <div v-else class="no-video">
            Видео недоступно
          </div>
        </section>
        <section 
          v-if="videoData" 
          class="video-info"
        >
          <h1 class="video-title">{{ videoData.title }}</h1>
          <section class="channel-row">
            <div class="channel-block">
              <img @error="event => event.target.style.display = 'none'" class="user-avatar" :src="videoData.user?.userAvatar?.fileDirInStorage" alt="Channel avatar">
              <div class="channel-data">
                <p>{{videoData.user?.userName}}</p>
                <p class="subscribers-count">{{formatter.countFormatter(videoData.user?.subscribersCount, 'subs')}}</p>
              </div>
              <button v-if="true" style="color: #100E0E; font-size: 0.9rem; cursor: default;" :class="[videoData.user.isSubscribed ? 'unsub-button' : 'sub-button', 'control-button']">
              {{ videoData.user.isSubscribed ? 'Отписаться' : 'Подписаться' }}</button>
            </div>
            <div class="actions-wrapper">
              <div class="control-button">
                <button class="reaction-btn like control-button">
                  <svg xmlns="http://www.w3.org/2000/svg" width="10" height="30" viewBox="-1 -1 12 32">
                    <path d="M10 0 L10 30 L5 30 L5 15 L0 15 L10 0 Z"/>
                  </svg>
                  <span class="grade-count" aria-hidden="true">{{ formatter.countFormatter(videoData.likesCount) }}</span>
                </button>
                <button class="reaction-btn dislike control-button">
                  <svg xmlns="http://www.w3.org/2000/svg" width="10" height="30" viewBox="-1 -1 12 32">
                    <path d="M10 0 L10 30 L5 30 L5 15 L0 15 L10 0 Z" transform="rotate(180 5 14)"/>
                  </svg>
                  <span class="grade-count" aria-hidden="true">{{ formatter.countFormatter(videoData.dislikeCount) }}</span>
                </button>
              </div>
              <div class="secondary-actions">
                <button class="control-button" @click.stop="handleShareClick">
                  Поделиться
                </button>
                <button class="control-button">
                  Сорханить
                </button>
              </div>

            </div>
          </section>
          <div class="video-meta">
            <div class="video-statistic">
              <span v-if="videoData.viewsCount != null">{{formatter.countFormatter(videoData.viewsCount, 'views')}}</span>
              <span v-if="videoData.created">{{formatter.formatRussianDate(videoData.created)}}</span>
            </div>
            <p 
              class="video-description" 
              :class="{ 'clamped': !showFullDescription }"
              ref="descriptionElement"
            >
              {{ videoData.description }}
            </p>            
            <button 
                v-if="isDescriptionClamped" 
                @click="showFullDescription = !showFullDescription"
                class="show-more-button"
              >
                {{ showFullDescription ? 'Скрыть' : 'Показать больше' }}
            </button>
          </div>

          <p style="font-size: 20px; line-height: initial;">{{formatter.countFormatter(videoData.commentsCount, 'comments')}}</p>          
        </section>
        <CreateCommentBlock style="margin-top: 40px;" ref="addComment"/>
      </div>
         
      <aside class="side-recomendation">

      </aside>
    </template>
  </main>
</template>

<style scoped>
.video-page {
  display: flex;
  flex-direction: row;
  gap: 1vw;
  box-sizing: border-box;
  width: 100%;  
  margin-top: 70px;
}

.content-wrapper {
  width: 70%;
}

.side-recomendation {
  width: 30%;
}

.error-message {
  color: #ff4d4f;
  text-align: center;
  padding: 40px 0;
}

.no-video {
  color: #f39e60;
  text-align: center;
  padding: 40px 0;
}

.channel-row {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  width: 100%;
}

.channel-row button {
  height: 35px;
  padding: 10px;
}

.channel-block {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex: 1 1 auto; 
  min-width: min-content;
}

.channel-data {
  display: flex;
  flex-direction: column;
  align-content: center;
  gap: 5px;
  font-size: 1rem;
}

.subscribers-count {
  font-size: 0.875rem;
  color: var(--text-secondary);
}

.sub-button {
  background: #f39e60;
}

.sub-button:hover {
  opacity: 0.8;
}

.unsub-button {
  background: #f3f0e9;
}

.unsub-button:hover {
  opacity: 0.8;
}


.actions-wrapper {
  display: flex;
  gap: 1rem;
  flex: 0 1 auto; /* Не растягивается, но может сжиматься */
  align-items: center;
}

.secondary-actions {
  display: flex;
  gap: 1rem;
}

.actions-wrapper button {
  background: #2D2D2D;
}

.actions-wrapper button:hover {
  background: #4A4947;
}

.reaction-btn {
  padding: 0 0 0 10px;
}

.reaction-btn span {
  display: inline-block;
  padding-left: 10px;
  font-size: 0.9rem;
  line-height: 1.3;
}

.reaction-btn:hover {
  background: #4A4947;
}

.reaction-btn:first-of-type::after {
  content: "";
  position: absolute;
  right: 0;
  top: 10%;
  height: 80%;
  width: 1px;
  background-color: #f3f0e9;
}

.reaction-btn:first-of-type {
  position: relative;
  border-radius: 4px 0 0 4px;
}

.reaction-btn:last-of-type {
  border-radius: 0 4px 4px 0;
}

.video-info {
  display: flex;
  flex-direction: column;
  gap: 10px;
  color: #f3f0e9;
  overflow: hidden;
}

.video-info h1 {
  margin-bottom: 10px;
  font-size: 1.5rem;
}

.video-meta {
  box-sizing: border-box;
  padding: 10px 20px;
  background: #2D2D2D;
  border-radius: 4px;
  line-height: 1.5;
}

.video-statistic {
  display: flex;
  font-size: 0.875rem;
  gap: 1rem;
  -webkit-text-stroke: 0.3px currentColor;
}

.video-description {
  white-space: pre-line;
  word-wrap: break-word;
  margin: 10px 0;
  transition: all 0.3s ease;
  font-size: 0.875rem;
}

.video-description.clamped {
  display: -webkit-box;
  -webkit-line-clamp: 1; /* Количество строк до обрезки */
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}

.show-more-button {
  background: none;
  border: none;
  color: #f3f0e9;
  cursor: pointer;
  padding: 5px 0;
  font-size: 0.875rem;
  margin-top: 5px;
}

.show-more-button:hover {
  text-decoration: underline;
}
</style>