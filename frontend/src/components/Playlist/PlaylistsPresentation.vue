<script setup>
import { computed, nextTick, onBeforeMount, onMounted, onUnmounted, ref, } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/assets/utils/api.js'

import KebabMenu from '../Kebab/KebabMenu.vue'
import RenamePlaylistOverlay from './RetitlePlaylistOverlay.vue'
import LoadingState from '@/components/Solid/LoadingState.vue'
import PlaylistCard from './PlaylistCard.vue'

const props = defineProps({
  errorMessage: {
    type: String,
    default: '',
  },
  blocksInRow: {
    type: Number,
    default: 4,
  },
  isInfiniteScroll: {
    type: Boolean,
    default: true,
  },
  scrollElement: {
    type: String,
    default: 'window',
  },
})

const emit = defineEmits(['load-more', 'rename', 'delete'])

const playlists = ref([])
const isLoading = ref(true)

const container = ref(props.scrollElement)

const isAuthorized = ref(false)
const router = useRouter()
const currentPlaylist = ref({})
const parentWidth = ref(0)
const kebabMenuRef = ref(null)
const retitlePlaylistRef = ref(null)
const error = ref(null)

onBeforeMount(async () => {
  isAuthorized.value = Boolean(localStorage.getItem('userData'))
  if (!isAuthorized.value) return

  await nextTick()
  await adaptiveView()
  window.addEventListener('resize', adaptiveView)
})

const handleKebabClick = ({ playlist, buttonElement }) => {
  currentPlaylist.value = playlist
  kebabMenuRef.value?.openMenu(buttonElement)
}

const handleRetitlePlaylist = (event) => {
  event?.stopPropagation() // Добавьте проверку на существование event
  // console.log("handleRetitlePlaylist", currentPlaylist.value.playlistTitle);
  retitlePlaylistRef.value?.toggleMenu(currentPlaylist.value.playlistTitle)
}

const handleDeletePlaylist = (event) => {
  event?.stopPropagation() // Добавьте проверку на существование event
  emit('delete', currentPlaylist.value.playlistId)
}

const retitlePlaylist = async (playlistTitle) => {
  try {
    // console.log(playlistTitle);
    // console.log(currentPlaylist.value);
    await api.patch(`/playlists/${currentPlaylist.value.playlistId}`,
        {
          title: playlistTitle,
        })
    await resetPlaylists()
  } catch (err) {
    error.value =
        err.response?.data?.message ||
        err.message ||
        'Ошибка при удалении плейлиста'
  }
}

const deletePlaylist = async (playlistId) => {
  try {
    await api.delete(`/playlists/${playlistId}`)
    await resetPlaylists()
  } catch (err) {
    error.value =
        err.response?.data?.message ||
        err.message ||
        'Ошибка при удалении плейлиста'
  }
}

const navigateToPlaylist = (playlist) => {
  // console.log('$$Playlist data from navigate', playlist)
  router.push({
    name: 'PlaylistPage',
    params: { id: playlist.id },
    query: { title: playlist.title, count: playlist.count }
  })
}

const fetchMethod = async (after) => {
  // Проверяем авторизацию перед запросом
  if (!isAuthorized.value) {
    playlists.value = []
  }

  try {
    const response = await api.get(`/users/me/playlists`)
    playlists.value = response.data
    isLoading.value = false
  } catch (error) {
    console.error('Ошибка получения плейлистов:', error)
    if (error.response?.status === 401) {
      localStorage.removeItem('userData')
      window.dispatchEvent(new CustomEvent('auth-update'))
      router.push('/login')
    }

    playlists.value = []
  }
}

const updateDimensions = () => {
  if (!container.value) return
  const rect = container.value.getBoundingClientRect()
  parentWidth.value = rect.width
}

const adaptiveView = async () => {
  await nextTick()
  updateDimensions()

  if (!container.value) return

  const gap =
      computedBlocksInRow.value > 1
          ? Math.max(
              10,
              Math.floor(
                  (parentWidth.value -
                      parseFloat(computedBlockWidth.value) *
                      computedBlocksInRow.value) /
                  (computedBlocksInRow.value - 1)
              )
          )
          : 0

  container.value.style.gap = `30px ${Math.floor(gap)}px`
}

const computedBlocksInRow = computed(() => {
  if (props.rowLayout || props.context === 'aside-recomend') return 1
  if (parentWidth.value < 600) return 1
  if (parentWidth.value < 800) return 2
  if (parentWidth.value < 1200) return 3
  if (parentWidth.value < 1920) return 5
  return 5
})

const computedBlockWidth = computed(() => {
  if (props.rowLayout || props.context === 'aside-recomend') return '100%'
  if (parentWidth.value < 600) return `${parentWidth.value}px`
  if (parentWidth.value < 800)
    return `${Math.floor(parentWidth.value * 0.49)}px`
  if (parentWidth.value < 1200)
    return `${Math.floor(parentWidth.value * 0.32)}px`
  if (parentWidth.value < 1920)
    return `${Math.floor(parentWidth.value * 0.19)}px`
  return `${Math.floor(parentWidth.value * 0.19)}px`
})

onMounted(async () => {
  await nextTick()
  await adaptiveView()
  await fetchMethod()
  window.addEventListener('resize', adaptiveView)
  // console.log("Request prop:", props.request);
})

onUnmounted(() => {
  window.removeEventListener('resize', adaptiveView)
})

defineExpose({
  deletePlaylist,
})
</script>
<template>
  <KebabMenu
      ref="kebabMenuRef"
      :context="'playlist'"
      @retitle="handleRetitlePlaylist"
      @delete="handleDeletePlaylist"
  />
  <RenamePlaylistOverlay ref="retitlePlaylistRef" @retitle="retitlePlaylist"/>
  <div v-if="isAuthorized" v-auth="true" class="container-wrapper">
    <div v-if="!isLoading && errorMessage.length > 0" class="results-grid">
      <div v-if="playlists.length === 0" class="empty-results">
        Плейлисты отсутствуют.
      </div>
    </div>
    <div
        v-else
        ref="container"
        class="container"
        style="width: 100%; color: aliceblue"
    >
      <PlaylistCard
          v-for="playlist in playlists"
          :playlist="playlist"
          :key="playlist.id"
          @click="navigateToPlaylist(playlist)"
          @kebab-click="handleKebabClick"
          :style="{ width: computedBlockWidth }"
      />
      <LoadingState v-if="isLoading"/>
    </div>
  </div>

  <div v-auth="false" class="unauthorized-message">
    <p>Для просмотра плейлистов необходимо авторизоваться.</p>
  </div>
</template>
<style scoped>
.container-wrapper {
  display: flex;
  flex-wrap: wrap;
  box-sizing: border-box !important;
  container-type: inline-size;
  container-name: playlists-container;
}

.container-wrapper {
  width: 100%;
  padding: 20px 100px;
  margin-top: 70px;
}

.container-wrapper.aside-recomend {
  width: 100%;
  padding: 0px;
}

.container {
  display: flex;
  flex-wrap: wrap;
}

.row-layout {
  display: grid;
  columns: 1;
}

.error-state {
  color: #f39e60;
  text-transform: uppercase;
  text-align: center;
  padding: 20px;
  background: rgba(255, 0, 0, 0.05);
  border-radius: 8px;
  margin: 20px 0;
}

.empty-results {
  text-align: center;
  color: #f39e60;
  font-size: 18px;
  padding: 40px 0;
}

.results-grid {
  display: grid;
  gap: 20px;
}

.observer-target {
  width: 100%;
  height: 1px;
  margin: 0;
  padding: 0;
  background: #f39e60;
}

@media (max-width: 800px) {
  .container-wrapper {
    padding: 15px;
  }
}

/* @media (max-width: 600px) {
  .container-wrapper {
      padding: 10px;
  }
} */
</style>