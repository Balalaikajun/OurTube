<script setup>
import { computed, nextTick, onBeforeUnmount, ref } from 'vue'
import { useMenuManager } from '@/assets/utils/useMenuManager'
import { onClickOutside } from '@vueuse/core'
import { useVideoStore } from '@/assets/store/videoSettings'

const props = defineProps({
  videoResolution: {
    type: Array,
    required: true
  }
})

const videoStore = useVideoStore()

const emit = defineEmits(['delete', 'edit-click', 'share'])
const isOpen = ref(false)
const currentMenu = ref('main')
const position = ref({ bottom: '0px', right: '0px' })

// const seekBarInput = ref(0.2);

const menuRef = ref(null)
const mainRef = ref(null)
const resolutionRef = ref(null)
const speedRef = ref(null)

const { registerMenu, unregisterMenu } = useMenuManager()

const userInputBarStyle = computed(() => ({
  background: `linear-gradient(to right, #F39E60 ${videoStore.speed / 3 * 100}%, #F3F0E9 ${videoStore.speed / 3 * 100}%)`
}))

// const changeUserInput = (event) => {
//     seekBarInput.value = event.target.value;
//     // ИЗМЕНЕНО: Добавлена проверка на существование videoPlayerRef
//     if (videoPlayerRef.value) {
//         videoPlayerRef.value.volume = seekBarInput.value;
//     }
// };

const openMenu = async (buttonElement) => {
  // console.log(props.videoResolution)
  try {
    if (!buttonElement?.getBoundingClientRect) {
      console.error('Invalid button element')
      return
    }

    if (isOpen.value) {
      await closeMenu()
      return
    }

    registerMenu({ closeMenu })

    isOpen.value = true

    await nextTick()

    onClickOutside(menuRef, closeMenu, { capture: true })
  } catch (error) {
    console.error('Error opening menu:', error)
  }
}

const closeMenu = async () => {
  if (!isOpen.value) return
  currentMenu.value = 'main'
  isOpen.value = false
  unregisterMenu({ closeMenu })
  await nextTick()
}

const handleToMain = async () => {
  currentMenu.value = 'main'
}

const handleResolution = async () => {
  currentMenu.value = 'resolution'
}

const handleSpeed = async () => {
  currentMenu.value = 'speed'
}

onBeforeUnmount(() => {
  closeMenu()
})
defineExpose({
  openMenu,
  closeMenu,
})
</script>

<template>
  <div
      v-if="isOpen"
      ref="menuRef"
      class="kebab-menu-wrapper"
      :style="position"
      @click.stop
  >
    <div class="kebab-menu" ref="mainRef" v-if="currentMenu == 'main'">
      <button @click="handleResolution">Качество <span>{{ videoStore.resolution }}p</span></button>
      <button @click="handleSpeed">Скорость <span>{{ videoStore.speed }}x</span></button>
    </div>
    <div class="kebab-menu" ref="resolutionRef" v-if="currentMenu == 'resolution'">
      <button @click="handleToMain">< Качество</button>
      <span class="line"></span>
      <button
          v-for="res in videoResolution"
          :key="res.resolution"
          @click.stop="videoStore.setResolution(res.resolution)"
          :value="res.resolution"
          :class="{ active: videoStore.resolution == res.resolution }"
          style="justify-content: end;"
      >
        <span v-if="videoStore.resolution == res.resolution">v</span>
        {{ res.resolution }}p
      </button>
    </div>
    <div class="kebab-menu" ref="speedRef" v-if="currentMenu == 'speed'">
      <button @click="handleToMain">< Скорость</button>
      <span class="line"></span>
      <div class="volume-control">
        <span class="speed-value">{{ videoStore.speed }}x</span>
        <input
            v-model="videoStore.speed"
            class="user-set-bar"
            type="range"
            min="0.25"
            max="3"
            step="0.25"
            :style="userInputBarStyle"
        />
        <!-- @input="changeUserInput" -->
      </div>
    </div>

  </div>
</template>

<style scoped>
.kebab-menu-wrapper {
  position: relative;
  justify-self: end;
  width: fit-content;
  background: #4A4947;
  border: none;
  border-radius: 4px;
  z-index: 1000;
}

.kebab-menu {
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.kebab-menu button {
  display: flex;
  width: 100%;
  gap: 10px;
  justify-content: space-between;
  background: none;
  border: none;
  color: #f3f0e9;
  cursor: pointer;
  padding: 10px 12px;
  text-align: left;
  transition: background-color 1s ease;
}

.kebab-menu button:first-child {
  border-top-left-radius: 4px;
  border-top-right-radius: 4px;
  padding-bottom: 15px;
}

.kebab-menu button:last-child {
  border-bottom-left-radius: 4px;
  border-bottom-right-radius: 4px;
  padding-bottom: 15px;
}

.kebab-menu button:hover {
  background: #F39E60;
  color: #100E0E;
}

.kebab-menu button:hover span {
  color: #100E0E;
}

.volume-control {
  display: flex;
  flex-direction: column;
  gap: 10px;
  align-items: center;
  justify-content: space-between;
  padding: 15px;
}

.active {
  background-color: #F39E60 !important;
  color: #100E0E !important;
  justify-content: space-between !important;
}

.line {
  align-self: center;
  width: 90%;
  height: 1px;
  background-color: #f3f0e9;
  margin: 4px 0;
}
</style>