<script setup>
import { onMounted, ref, watch, computed, nextTick } from "vue";
import { useRoute, useRouter } from "vue-router";
import api from "@/assets/utils/api.js";
import ConfirmPannel from "@/components/Solid/ConfirmPannel.vue";
import PlaylistOverlay from "@/components/Playlist/PlaylistsOverlay.vue";
import VideosPresentation from "@/components/Video/VideosPresentation.vue";

const router = useRouter();
const route = useRoute();

// Используем ref вместо computed для isAuthorized
const isAuthorized = ref(Boolean(localStorage.getItem("userData")));

const videoId = ref(null);
const confirmRef = ref(null);
const confirmContext = ref("");
const playlistRef = ref(null);
const queryText = ref("");
const searchQuery = ref("");
const isInputFocused = ref(false);

// Добавляем функцию adaptiveView
const adaptiveView = async () => {
  // Ваша реализация адаптивного представления
  // console.log("Adaptive view logic here");
};

onMounted(async () => {
  // Обновляем авторизацию при монтировании
  isAuthorized.value = Boolean(localStorage.getItem("userData"));
  
  if (isAuthorized.value) {
    await nextTick();
    await adaptiveView();
    window.addEventListener("resize", adaptiveView);
  }
});

const saveOpen = (videoId) => {
  if (!isAuthorized.value) return;
  playlistRef.value.toggleMenu(videoId);
};

const handleSearch = (event) => {
  event?.preventDefault();
  if (!isAuthorized.value) return;
  
  if (queryText.value.trim()) {
    searchQuery.value = queryText.value.trim();
    router.push({
      path: "/history",
      query: { q: queryText.value.trim() },
    });
  } else {
    clearSearch();
  }
};

const clearSearch = () => {
  queryText.value = "";
  searchQuery.value = "";
};

const handleDeleteHistory = () => {
  if (!isAuthorized.value) return;
  confirmContext.value = "Удаление истории";
  confirmRef.value.openMenu();
};

const handleDeleteFromHistory = async (videoId) => {
  if (!isAuthorized.value) return;
  
  try {
    await api.delete(`History/${videoId}`);
  } catch (error) {
    console.error("History error:", error);
  } finally {
    clearSearch();
  }
};

const clearHistory = async () => {
  if (!isAuthorized.value) return;
  
  try {
    await api.delete("History");
  } catch (error) {
    console.error("History error:", error);
  } finally {
    clearSearch();
  }
};

watch(
  () => route.query.q,
  (newQuery) => {
    if (!isAuthorized.value) return;
    if (newQuery !== undefined && newQuery !== queryText.value) {
      queryText.value = newQuery;
      searchQuery.value = newQuery;
    }
  },
  { immediate: true }
);
</script>

<template>
  <PlaylistOverlay v-if="isAuthorized" ref="playlistRef" />
  <ConfirmPannel v-if="isAuthorized" ref="confirmRef" :action="confirmContext" @confirm="clearHistory" />

  <main class="main-wrapper">
    <h1>История просмотра</h1>

    <div class="columns-wrapper">
      <div v-if="!isAuthorized" class="unauthorized-message" v-auth="false">
        <p>История просмотра недоступна. Войдите в учётную запись.</p>
      </div>

      <template v-if="isAuthorized">
        <VideosPresentation 
          request="history" 
          context="aside-recomend" 
          :search-query="searchQuery"
          :is-infinite-scroll="true" 
          :row-layout="true" 
          @add-to-playlist="saveOpen"
          @delete="handleDeleteFromHistory" 
        />

        <aside class="history-functional" v-auth="true">
          <div class="history-menu">
            <div class="enter-query">
              <div class="enter-line">
                <button class="control-button enter-query-btn" @click="handleSearch" v-auth="true">
                  <!-- SVG иконка поиска -->
                </button>
                <textarea 
                  class="component-input" 
                  placeholder="Поиск по истории" 
                  v-model="queryText"
                  @keydown.enter.prevent="handleSearch" 
                  rows="1" 
                  wrap="off" 
                  v-auth="true"
                ></textarea>
                <button v-if="queryText" class="control-button enter-query-btn" @click="clearSearch" v-auth="true">
                  <!-- SVG иконка очистки -->
                </button>
              </div>
              <div class="line-container">
                <span class="line-decoration" :class="{ active: isInputFocused }"></span>
              </div>
            </div>

            <button class="control-button delete-btn" @click.stop="handleDeleteHistory" v-auth="true">
              Очистить историю
            </button>
          </div>
        </aside>
      </template>
    </div>
  </main>
</template>

<style scoped>
.main-wrapper {
    display: flex;
    flex-direction: column;
    gap: 5vh;
    box-sizing: border-box;
    width: 100%;
    margin-top: 70px;
    padding: 20px 100px;
}

.main-wrapper h1 {
    -webkit-text-stroke: 0.3px currentColor;
}

.columns-wrapper {
    display: flex;
    width: 100%;
    flex-direction: row;
    gap: 10vw;
}

.history-functional {
    position: relative;
    width: 30%;
    box-sizing: border-box;
    padding: 0 30px 0 30px;
}

.history-menu {
    display: flex;
    flex-direction: column;
    position: sticky;
    top: 90px;
    gap: 2vh;
}

.enter-query {
    display: flex;
    flex-direction: column;
}

.enter-line {
    display: flex;
    flex-direction: row;
    align-items: center;
}

.enter-query-btn {
    width: 40px;
    height: 40px;
}

.enter-query-btn:hover {
    background-color: #4a4947;
}

.component-input {
    width: 100%;
    opacity: 0.7;
    min-height: 15px;
    color: #f3f0e9;
    line-height: 15px;
    font-size: 14px;
    outline: none;
    resize: none;
    /* Запрещаем изменение размера */
    box-sizing: border-box;
    background: transparent;
    border: none;
    white-space: nowrap;
    /* Запрещаем перенос строк */
    overflow-x: hidden;
    /* Горизонтальный скролл при переполнении */
    overflow-y: hidden;
    /* Скрываем вертикальный скролл */
}

.line-container {
    position: relative;
    width: 100%;
    height: 2px;
    /* Фиксированная высота контейнера */
}

.line-decoration {
    position: absolute;
    left: 0;
    right: 0;
    height: 2px;
    background-color: #f3f0e9;
    transform: scaleX(0);
    transform-origin: center;
    transition: transform 0.3s ease;
}

.line-decoration.active {
    width: 100%;
    left: 0;
    transform: none;
}

.delete-btn {
    width: 100%;
    height: 40px;
    border: 1px solid #f3f0e9;
    box-sizing: border-box;
}

.delete-btn:hover {
    border: 1px solid #4a4947;
    background-color: #4a4947;
}

.delete-btn svg {
    position: absolute;
    left: 0;
}

.component-input:focus {
    opacity: 1;
}

.component-input:focus::-webkit-input-placeholder {
    /* Chrome/Opera/Safari */
    opacity: 0;
}

.component-input::-webkit-input-placeholder {
    /* Chrome/Opera/Safari */
    display: -webkit-box;
    -webkit-line-clamp: 1;
    -webkit-box-orient: horizontal;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: normal;
    /* min-height: 17px; */
}

.component-input:focus::-moz-placeholder {
    /* Firefox 19+ */
    opacity: 0;
}

.component-input:focus:-ms-input-placeholder {
    /* IE 10+ */
    opacity: 0;
}

.component-input:focus:-moz-placeholder {
    /* Firefox 18- */
    opacity: 0;
}

@media (max-width: 1000px) {
    .delete-btn svg {
        display: none;
    }

    .columns-wrapper {
        gap: 1vw;
    }

    .history-functional {
        padding-right: 0;
    }
}
</style>
