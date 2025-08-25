<script setup>
import { computed, nextTick, onMounted, onUnmounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import VideoCard from '@/components/Video/VideoCard.vue'
import KebabMenu from '../Kebab/KebabMenu.vue'
import ShareOverlay from '../Kebab/ShareOverlay.vue'
import LoadingState from '@/components/Solid/LoadingState.vue'
import useInfiniteScroll from '@/assets/utils/useInfiniteScroll.js'
import api from '@/assets/utils/api.js'
import { getScrollbarWidth } from "@/assets/utils/dom.js"

const props = defineProps({
  errorMessage: {
    type: String,
    default: ''
  },
  request: {
    type: String,
    required: true,
    validator: (value) => ['recomend', 'search', 'history', 'playlist'].includes(value)
  },
  context: {
    type: String,
    required: true,
    validator: (value) => ['recomend', 'aside-recomend', 'search'].includes(value)
  },
  searchQuery: {
    type: String,
    default: ''
  },
  videoId: {
    type: String,
    default: ''
  },
  playlistId: {
    type: Number,
    default: null
  },
  blocksInRow: {
    type: Number,
    default: 4
  },
  isInfiniteScroll: {
    type: Boolean,
    default: true
  },
  scrollElement: {
    type: String,
    default: 'window'
  },
  isMobile: {
    type: Boolean,
    default: false
  },
  rowLayout: {
    type: Boolean,
    default: false
  }
})

const router = useRouter()
const route = useRoute()
const currentVideoId = ref('')

const parentWidth = ref(0)
const scrollbarWidth = ref(0)
const kebabMenuRef = ref(null)
const shareRef = ref(null)

const emit = defineEmits(['load-more', 'add-to-playlist', 'delete'])

const {
  data: videos,
  observerTarget,
  hasMore,
  isLoading,
  error: scrollError,
  container,
  loadMore,
  reset: resetPlaylist
} = useInfiniteScroll({
  fetchMethod: async (after) => {
    const result = await fetchMethods[props.request](after)
    // emit('load-more');
    return result
  },
  scrollElement: props.scrollElement,
  isEnabled: props.isInfiniteScroll,
  initialLoad: true
})

const handleKebabClick = ({ videoId, buttonElement }) => {
  currentVideoId.value = videoId
  kebabMenuRef.value?.openMenu(buttonElement)
}

const handleShortDelete = (videoId) => {
  emit('delete', videoId)
}

const handleAddToPlaylist = () => {
  // console.log('save', currentVideoId.value)
  emit('add-to-playlist', currentVideoId.value)
}

const handleShareClick = () => {
  if (shareRef.value?.openMenu) {
    shareRef.value.openMenu()
  }
}

const navigateToVideo = (video) => {
  // console.log('to', video.id)
  router.push(`/video/${video.id}`)
}

// Логика бесконечной прокрутки
const fetchMethods = {
  async recomend (after) {
    const limit = computedBlocksInRow.value * 4

    try {
      const endpoint = props.context === 'aside-recomend'
          ? `/recommendations/videos/${props.videoId}`
          : '/recommendations'

      const response = await api.get(endpoint, {
        params: {
          Limit: limit,
          After: after || 0,
          Reload: false
        }
      })

      return {
        items: response.data?.elements || [],
        nextAfter: response.data?.nextAfter || 0,
        hasMore: response.data?.hasMore || false
      }

    } catch (error) {
      console.error('Ошибка при получении рекомендаций:', error)
      if (error.response?.status === 401) router.push('/login')
      return {
        items: [],
        nextAfter: 0,
        hasMore: false
      }
    }
  },

  async search (after) {
    if (!props.searchQuery.trim()) return { items: [], nextAfter: 0, hasMore: false }
    const limit = computedBlocksInRow.value * 4
    try {
      const response = await api.get(`/search`, {
        params: {
          query: props.searchQuery,
          limit: limit,
          after: after || 0
        }
      })
      return {
        items: response.data?.elements || response.data?.videos || [],
        nextAfter: response.data?.nextAfter || 0,
        hasMore: response.data?.hasMore || false
      }
    } catch (error) {
      console.error('Ошибка при выполнении поиска:', error)
      return { items: [], nextAfter: 0, hasMore: false }
    }
  },

  async history (after) {
    const limit = computedBlocksInRow.value * 4
    try {
      const response = await api.get(`/users/me/watch-history`, {
        params: {
          query: props.searchQuery,
          limit: limit,
          after: after || 0
        }
      })
      return {
        items: response.data?.elements || response.data?.videos || [],
        nextAfter: response.data?.nextAfter || 0,
        hasMore: response.data?.hasMore || false
      }
    } catch (error) {
      console.error('Ошибка при получении истории:', error)
      return { items: [], nextAfter: 0, hasMore: false }
    }
  },

  async playlist (after) {
    const playlistId = route.params.id
    if (!playlistId) return { items: [], nextAfter: 0, hasMore: false }

    const limit = computedBlocksInRow.value * 4

    try {
      const response = await api.get(`/playlists/${playlistId}/elements`, {
        params: { limit, after: after || 0 }
      })

      const items = response.data.elements.map(el => el.video)
      return {
        items,
        nextAfter: response.data.nextAfter,
        hasMore: items.length >= limit && response.data.nextAfter !== 0
      }
    } catch (error) {
      console.error('Playlist load error:', error)
      if (error.response?.status === 401) router.push('/login')
      return { items: [], nextAfter: 0, hasMore: false }
    }
  }
}

const updateDimensions = () => {
  if (!container.value) return
  const rect = container.value.getBoundingClientRect()
  parentWidth.value = rect.width - scrollbarWidth.value;
}

const adaptiveView = async () => {
  await nextTick()
  updateDimensions()

  if (!container.value) return

  const gap = computedBlocksInRow.value > 1
      ? Math.max(10, Math.floor((parentWidth.value - (parseFloat(computedBlockWidth.value) * computedBlocksInRow.value)) / (computedBlocksInRow.value - 1)))
      : 0

  container.value.style.gap = `30px ${Math.floor(gap)}px`
}

const computedBlocksInRow = computed(() => {
  if (props.rowLayout) return 1
  if (parentWidth.value < 600) return 1
  if (parentWidth.value < 800) return 2
  if (parentWidth.value < 1200) return 3
  if (parentWidth.value < 1920) return 4
  return 5
})

const computedBlockWidth = computed(() => {
  if (props.rowLayout) return '100%'
  if (parentWidth.value < 600) return `${parentWidth.value}px`
  if (parentWidth.value < 800) return `${Math.floor(parentWidth.value * 0.49)}px`
  if (parentWidth.value < 1200) return `${Math.floor(parentWidth.value * 0.32)}px`
  if (parentWidth.value < 1920) return `${Math.floor(parentWidth.value * 0.24)}px`
  return `${Math.floor(parentWidth.value * 0.19)}px`
})

onMounted(async () => {
  await nextTick()
  scrollbarWidth.value = getScrollbarWidth()
  await adaptiveView()
  window.addEventListener("resize", adaptiveView)
})

onUnmounted(() => {
  window.removeEventListener('resize', adaptiveView)
})

watch(() => props.request, () => loadMore(true))
watch(() => props.searchQuery, (newVal, oldVal) => {
  if (newVal !== oldVal) loadMore(true)
}, { immediate: true })

defineExpose({
  resetPlaylist,
  loadMore,
  hasMore
})
</script>

<template>
  <div style="width: 100%;">
    <KebabMenu
        ref="kebabMenuRef"
        @add-to-playlist="handleAddToPlaylist"
        @share="handleShareClick"
    />
    <ShareOverlay
        ref="shareRef"
        :videoId="currentVideoId"
    />
    <div
        class="container-wrapper"
        :class="[context == 'recomend' || context == 'search' ? 'standart-recomend' : `aside-recomend`,
      { 'row-layout': rowLayout || context == 'aside-recomend' }]">
      <div v-if="!isLoading && errorMessage.length > 0" class="results-grid">
        <div v-if="videos.length === 0" class="empty-results">
          Ничего не найдено
        </div>
      </div>
      <div v-if="scrollError" class="error-state">
        {{ scrollError }}
      </div>
      <div v-else ref="container" class="container">
        <VideoCard
            v-for="video in videos"
            :video="video"
            :key="video.id"
            :row-layout="rowLayout"
            :is-short-delete="['history', 'playlist'].includes(request)"
            @click="navigateToVideo(video)"
            @kebab-click="handleKebabClick"
            @delete="handleShortDelete"
            :style="{ width: computedBlockWidth }"
        />
        <LoadingState v-if="isLoading"/>
        <div ref="observerTarget" class="observer-target" v-if="isInfiniteScroll && hasMore"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container-wrapper {
  display: flex;
  flex-wrap: wrap;
  box-sizing: border-box !important;
  container-type: inline-size;
  container-name: recommendations-container;
  width: 100%;
}

.container-wrapper.standart-recomend {
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
  align-items: flex-start;
  width: 100%;
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

@media (max-width: 1200px) {
  .container-wrapper.standart-recomend {
    padding: 20px;
  }

}
</style>