<script setup>
import { computed, ref, watch } from 'vue'

const props = defineProps({
  userAvatarPath: {
    type: String,
    default: ''
  },
  userInfo: {
    type: Object,
    default: () => ({
      id: '',
      userAvatar: null
    })
  },
  contextUse: {
    type: String,
    default: null
  }
})

const avatarUrl = ref(null)

const updateAvatarUrl = () => {
  console.log('Изменение аватара из UserAvatar')
  if (props.userAvatarPath) {
    avatarUrl.value = props.userAvatarPath
    return
  }

  if (props.userInfo?.userAvatar) {
    console.log('$$userInfo', props.userInfo)
    avatarUrl.value = `${import.meta.env.VITE_MINIO_BASE_URL}/${props.userInfo.userAvatar.bucket}/${props.userInfo.userAvatar.fileName}`
    return
  }

  avatarUrl.value = null
}

// Следим за изменениями пропсов
watch(() => props.userInfo, updateAvatarUrl, { immediate: true, deep: true })
watch(() => props.userAvatarPath, updateAvatarUrl, { immediate: true })
const viewBox = computed(() => {
  switch (props.contextUse) {
    case 'small':
      return '0 0 20 20'
    case 'large':
      return '0 0 60 60'
    case 'profile':
      return '0 0 80 80'
    default:
      return '0 0 40 40'
  }
})

// Вычисляемые свойства для CSS переменных
const avatarStyles = computed(() => {
  switch (props.contextUse) {
    case 'small':
      return {
        '--avatar-size': '20px',
        '--avatar-radius': '2px'
      }
    case 'large':
      return {
        '--avatar-size': '60px',
        '--avatar-radius': '8px'
      }
    case 'profile':
      return {
        '--avatar-size': '100px',
        '--avatar-radius': '12px'
      }
    default:
      return {
        '--avatar-size': '40px',
        '--avatar-radius': '4px'
      }
  }
})
</script>

<template>
  <div class="avatar-container" :style="avatarStyles">
    <!-- Показываем изображение только если есть путь И не было ошибки -->
    <svg
        v-if="avatarUrl == null"
        class="user-avatar"
        :viewBox="viewBox"
        style="fill: #F3F0E9"
    >
      <circle cx="50%" cy="50%" r="10%" fill="#100E0E"/>
      <text x="50%" y="50%" text-anchor="middle" dominant-baseline="middle" fill="#100E0E">?</text>
    </svg>
    <img
        v-else
        class="user-avatar"
        :src="avatarUrl"
        alt="User avatar"
    >
  </div>
</template>

<style scoped>
.avatar-container {
  border-radius: var(--avatar-radius, 4px);
  width: var(--avatar-size, 40px);
  height: var(--avatar-size, 40px);
  background: #F3F0E9;
}

@container (max-width: 500px) {
  .avatar-container {
    --avatar-size: 30px !important;
  }
}

@container (max-width: 300px) {
  .avatar-container {
    --avatar-size: 20px !important;
  }
}

.user-avatar {
  width: 100%;
  height: 100%;
  border-radius: var(--avatar-radius, 4px);
}

img.user-avatar {
  display: block;
  object-fit: cover;
}

svg.user-avatar {
  display: block;
  object-fit: cover;
  box-sizing: border-box;
  justify-content: center;
  align-items: center;
  width: var(--avatar-size, 40px);
  height: var(--avatar-size, 40px);
  background: transparent;
}
</style>