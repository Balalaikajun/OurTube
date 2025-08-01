<script setup>
import { computed, nextTick, onMounted, onUnmounted, provide, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '@/assets/utils/api.js'
import MasterHead from '../components/Solid/MasterHead.vue'
import ConfirmPannel from '@/components/Solid/ConfirmPannel.vue'
import PlaylistOverlay from '@/components/Playlist/PlaylistsOverlay.vue'
import VideoPlayer from '@/components/Video/VideoPlayer.vue'
import LoadingState from '@/components/Solid/LoadingState.vue' // Импортируем компонент загрузки
import ShareOverlay from '@/components/Kebab/ShareOverlay.vue'
import CreateCommentBlock from '@/components/Comment/CreateCommentBlock.vue'
import VideosPresentation from '@/components/Video/VideosPresentation.vue'
import CommentsPresentation from '@/components/Comment/CommentsPresentation.vue'
import UserAvatar from '@/components/Solid/UserAvatar.vue'
import ReactionBlock from '@/components/Solid/ReactionBlock.vue'
import formatter from '@/assets/utils/formatter.js'
import { useFocusEngine } from '@/assets/utils/focusEngine.js'
import useTextOverflow from '@/assets/utils/useTextOverflow.js'

// const userStore = useUserStore();

  const route = useRoute();
  const videoPage = ref(null);
  const videoId = ref(null);
  const Player = ref(null);
  const addComment = ref(null);
  const videoData = ref(null);
  const hlsUrlFiles = ref([]);
  const isLoading = ref(true);
  const error = ref(null);

  const desktopRecommendations = ref(null);
  const mobileRecommendations = ref(null);
  const isMobileLayout = ref(window.innerWidth < 1000);
  const isRow = ref(false);
  const contentWrapper = ref(null);
  const resizeObserver = ref(null);

  const shareRef = ref(null);
  const confirmRef = ref(null);
  const commentsRef = ref(null);
  const playlistRef = ref(null);

  const confirmContext = ref("")

  const userData = JSON.parse(localStorage.getItem('userData'));
  const userVideoOwnerData = ref({});
  const currentUserId = ref(userData?.id);

  const isVideoOwner = computed(() => {
    return videoData.value?.user?.id && currentUserId.value === videoData.value.user.id;
  });

  provide('videoId', videoId);

  const showFullDescription = ref(false);
  const { isClamped: isDescriptionClamped, checkTextOverflow } = useTextOverflow()
  const descriptionElement = ref(null); 

  const { focusedElement } = useFocusEngine();

  const ensureHttpUrl = (url) => {
    if (!url) return '';
    return url.startsWith('http') ? url : `http://${url}`;
  };

  const addToHistory = async () => {
    try {
      // console.log(videoId.value);
      await api.post(`/users/me/watch-history/videos/${videoId.value}`, {
        endTime: "0"
      });
      // console.log('Added to history');
    } catch (error) {
      console.error('History error:', error);
    }
  };

  const handleKeyDown = (e) => {
    if (focusedElement.value || !Player.value) return;

    if (e.code === 'KeyF') {
      e.preventDefault();
      Player.value.toggleFullscreen();
    } else if (e.code === 'Space') {
      e.preventDefault();
      if (Player.value.videoPlayerRef) {
        Player.value.videoPlayerRef.paused
          ? Player.value.videoPlayerRef.play()
          : Player.value.videoPlayerRef.pause();
      }
    }
  };

  const fetchVideoData = async () => {
    isLoading.value = true;
    error.value = null;
    videoData.value = null;
    hlsUrlFiles.value = [];

    // console.log("Fetching video data for ID:", videoId.value);

    if (!videoId.value) {
      error.value = "Идентификатор видео не предоставлен.";
      isLoading.value = false;
      return;
    }

    try {
      // console.log(videoId.value, 1)
      const response = await api.get(`videos/${videoId.value}`);
      const data = response.data;

      // console.log(data, "Информация о видео");

      if (!data) {
        throw new Error("Получены пустые данные видео");
      }

      if (!data.files || data.files.length === 0) {
        console.warn(`Видео ${videoId.value} не содержит файлов.`);
      }

      videoData.value = data;

      if (data.files?.length) {
        const file = data.files[0];
        if (file.fileName) {
          // console.log()
          // hlsUrlFiles.value = ensureHttpUrl(`${import.meta.env.VITE_MINIO_BASE_URL}/videos/${file.fileName}`);
          hlsUrlFiles.value = data.files
          // console.log(hlsUrlFiles.value)
          // console.log("HLS URL:", hlsUrlFiles.value);
        } else {
          console.warn(`Файл для видео ${videoId.value} не имеет fileName.`);
          hlsUrlFiles.value = [];
        }
      } else {
        hlsUrlFiles.value = [];
      }

      document.title = videoData.value.title ? `${videoData.value.title}` : 'MyApp';

      nextTick(() => {
        checkTextOverflow(descriptionElement.value, "Описание к видео");
      });
    } catch (err) {
      if (err.response) {
        error.value = err.response.data?.title || 'Ошибка загрузки видео';
        console.error("Ошибка API:", err.response);
      } else {
        error.value = err.message || 'Ошибка при загрузке видео';
        console.error("Ошибка при загрузке видео:", err);
      }
      videoData.value = null;
      hlsUrlFiles.value = [];
    } finally {
      isLoading.value = false;
    }
  };

  const handleShareClick = () => {
    if (shareRef.value && typeof shareRef.value.openMenu === 'function') {
      shareRef.value.openMenu();
    } else {
      console.error('ShareOverlay ref is not properly set or missing openMenu method');
    }
  };

  const handleDeleteComment = () => {
      confirmContext.value = "Удаление комментария";
      confirmRef.value.openMenu();
  };

  const handleConfirmDelete = () => {
      if (commentsRef.value) {
          commentsRef.value.deleteComment();
      }
  };

  const saveOpen = (videoId) => {
      playlistRef.value.toggleMenu(videoId);
  }

  const debounce = (fn, delay) => {
    let timeoutId;
    return (...args) => {
      clearTimeout(timeoutId);
      timeoutId = setTimeout(() => fn.apply(this, args), delay);
    };
  };

  const checkLayout = debounce(() => {
    if (!contentWrapper.value) {
      console.warn("Content wrapper not available");
      return;
    }
    
    const width = contentWrapper.value.offsetWidth;
    
    if (width !== lastWidth.value) {
      isRow.value = width < 1200 && !isMobileLayout.value;
      isMobileLayout.value = width < 1000; // Например, для экранов уже 768px
      lastWidth.value = width;
    }
  }, 100);

  const lastWidth = ref(0);

  const initResizeObserver = async () => {
    await nextTick();
    
    if (!contentWrapper.value) {
      console.error("Content wrapper element not found!");
      return;
    }

    if (resizeObserver.value) {
      resizeObserver.value.disconnect();
    }

    resizeObserver.value = new ResizeObserver(checkLayout);
    resizeObserver.value.observe(contentWrapper.value);
    
    const width = contentWrapper.value.offsetWidth;
    isRow.value = 1200 < width;
    lastWidth.value = width;
  };

  onMounted(async () => {    
    isMobileLayout.value = window.innerWidth < 1000;
    isRow.value = window.innerWidth < 1200 && !isMobileLayout.value;
  
    videoId.value = route.params.id;

    document.addEventListener('keydown', handleKeyDown);
    window.addEventListener('resize', () => {
      checkTextOverflow(descriptionElement.value, "Описание к видео");
    });

    await initResizeObserver();
  });
  onUnmounted( async () => {
    // console.log("Размонтирование VideoPage");
    await addToHistory();
    document.removeEventListener('keydown', handleKeyDown);
    window.removeEventListener('resize', checkTextOverflow(descriptionElement.value, "Описание к видео")); // Удаляем при размонтировании

    if (Player.value) {
      // console.log("Player ref существует, попытка очистки");
      if (typeof Player.value.destroyPlayer === 'function') {
        // console.log("Вызов destroyPlayer");
        Player.value.destroyPlayer();
      }
      if (Player.value.videoPlayerRef) {
          // console.log("Ручная очистка video элемента");
          try {
              Player.value.videoPlayerRef.pause();
              // Важно: удаляем src, а не устанавливаем null или пустую строку,
              // чтобы полностью отсоединить ресурс. load() затем очистит буфер.
              Player.value.videoPlayerRef.removeAttribute('src');
              Player.value.videoPlayerRef.load();
          } catch (e) {
              console.error("Ошибка при ручной очистке video элемента:", e);
          }
      }
    } else {
      // console.log("Player ref уже null или недоступен, очистка не требуется");
    }
    if (window.controlPanelTimeout) {
      clearTimeout(window.controlPanelTimeout);
    }

    if (resizeObserver.value && contentWrapper.value) {
      resizeObserver.value.unobserve(contentWrapper.value);
    }

    videoData.value = null;
    hlsUrlFiles.value = [];
  });
  const commentsCount = computed(() => {
    // console.log(videoData.value?.commentsCount || 0)
    return videoData.value?.commentsCount || 0;
  });

  // Добавляем watcher для videoId
  watch(videoId, (newVideoId, oldVideoId) => {
      // console.log(`videoId changed from ${oldVideoId} to ${newVideoId}`);
      // Если новый videoId отличается от старого и не пустой, загружаем новые данные
      if (newVideoId && newVideoId !== oldVideoId) {
          fetchVideoData();
      } else if (!newVideoId) {
          // Обработка случая, когда videoId становится пустым (например, при ошибке маршрутизации)
          videoData.value = null;
          hlsUrlFiles.value = [];
          isLoading.value = false;
          error.value = "Идентификатор видео отсутствует.";
      }
  });

  // Дополнительный watcher для videoData, чтобы обновить описание после загрузки
  watch(videoData, (newData) => {
      if (newData) {
          nextTick(() => {
              checkTextOverflow(descriptionElement.value, "Описание к видео");
          });
      }
  });
</script>

<template ref="videoPage">
  <!-- <MasterHead /> -->
  <ConfirmPannel 
    ref="confirmRef" 
    :action="confirmContext"
    @confirm="handleConfirmDelete"
  />
  <PlaylistOverlay ref="playlistRef" 
  />
  <!-- :video-id="String(videoId)" -->
  <ShareOverlay
    v-if="videoId"
    ref="shareRef" 
    :video-id="videoId"
  >
  </ShareOverlay>
  <main class="video-page" ref="contentWrapper">
    <LoadingState v-if="isLoading"></LoadingState>
    
    <div v-else-if="error" class="error-message">
      {{ error }}
    </div>
    
    <template v-else-if="videoData">
      <!-- <div class="video-page"> -->
        <div class="content-wrapper">
          <section>
            <VideoPlayer
              ref="Player"
              v-if="hlsUrlFiles.length" 
              :video-files="hlsUrlFiles" 
              :poster="videoData?.thumbnailUrl"
              :key="hlsUrlFiles"
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
                <UserAvatar :user-info="videoData.user" />
                <div class="channel-data">
                  <p>{{videoData.user?.userName}}</p>
                  <p class="subscribers-count">{{formatter.countFormatter(videoData.user?.subscribersCount, 'subs')}}</p>
                </div>
                <button v-auth="true" v-if="!isVideoOwner"  style="color: #100E0E; font-size: 0.9rem; cursor: default;" :class="[videoData.user.isSubscribed ? 'unsub-button' : 'sub-button', 'control-button']">
                  {{ videoData.user.isSubscribed ? 'Отписаться' : 'Подписаться' }}
                </button>
              </div>
              <div class="actions-wrapper">
                <ReactionBlock
                  :context="'video'"
                  :reaction-status="videoData?.vote"
                  :likes-count="videoData?.likesCount" 
                  :dislikes-count="videoData?.dislikesCount"
                />
                  <!-- @update-reaction="updateReaction" -->
                <div class="secondary-actions">
                  <button class="control-button" @click.stop="handleShareClick">
                    Поделиться
                  </button>
                  <button class="control-button" @click.stop="saveOpen(videoId)">
                    Сохранить
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
                :class="{ 'clamped': !showFullDescription}"
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

          <aside v-if="isMobileLayout">
            <VideosPresentation
              ref="mobileRecommendations"
              request="recomend"
              context="aside-recomend"
              :video-id="String(videoId)"
              @add-to-playlist="saveOpen"
              :is-mobile="isMobileLayout"
              :row-layout="isRow"
              :is-infinite-scroll="false"
              style="
                padding: 2rem 0;
              "
            />
            <button 
              v-if="isMobileLayout && mobileRecommendations?.hasMore" 
              @click="mobileRecommendations.loadMore()"
              class="reusable-button"
              style="
                background-color: #2D2D2D;
              "
            >
              Загрузить еще
            </button>
          </aside>

          <CreateCommentBlock :video-id="String(videoId)" style="margin-top: 40px;" ref="addComment"/>
          <CommentsPresentation
            v-if="commentsCount !== 0"
            ref="commentsRef"
            :video-id="String(videoId)"
            @delete="handleDeleteComment"
          />
        </div>
         
        <aside class="side-recomendation" v-if="!isMobileLayout">
          <VideosPresentation
            ref="desktopRecommendations"
            request="recomend"
            context="aside-recomend"
            :video-id="String(videoId)"
            @add-to-playlist="saveOpen"
            :is-mobile="isMobileLayout"
            :row-layout="isRow"
            :is-infinite-scroll="true"
          />
        </aside>
      <!-- </div> -->

    </template>
  </main>
</template>

<style scoped>

 /* @media (min-width: 1920px) {
    .video-page {
      width: 200px;
    }
  }

  @media (min-width: 1200px) and (max-width: 1920px) {
    .video-card {
      width: 24%;
    }
  }

  @media (max-width: 1200px) {
    .video-card {
      width: 32%;
    }
  }

  @media (max-width: 800px) {
    .video-card {
      width: 49%;
    }
  } */

.video-page {
  display: flex;
  flex-direction: row;
  gap: 1vw;
  box-sizing: border-box;
  width: 100%;  
  margin-top: 70px;
  padding: 20px 100px;
}

.content-wrapper {
  width: 70%;
  container-type: inline-size;
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

.load-more-button {
  display: block;
  width: 100%;
  padding: 12px;
  margin-top: 10px;
  background: #2D2D2D;
  color: #f3f0e9;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background 0.3s;
}

.load-more-button:hover {
  background: #4A4947;
}

@media (max-width: 1000px) {
  .video-page {
    flex-direction: column;
    padding: 20px;
  }
  
  .content-wrapper, .side-recomendation, .mobile-recomendation {
    width: 100% !important;
  }
  
  .side-recomendation {
    display: none;
  }
  
  .mobile-recomendation {
    display: block;
    margin-top: 20px;
  }
}

@media (min-width: 769px) {
  .mobile-recomendation {
    display: none;
  }
}
</style>