<script setup>
import { ref } from "vue";
import KebabButton from "../components/KebabButton.vue";
import { MINIO_BASE_URL } from "@/assets/config.js";

const props = defineProps({
  video: {
    type: Object,
    required: true,
    default: () => ({
      id: 0,
      title: '',
      viewsCount: 0,
      vote: false,
      endTime: '',
      created: '',
      preview: {
        fileName: '',
        fileDirInStorage: '',
        bucket: ''
      },
      user: {
        id: '',
        userName: '',
        isSubscribed: false,
        subscribersCount: 0,
        userAvatar: {
          fileName: '',
          fileDirInStorage: '',
          bucket: ''
        }
      }
    })
  },
  rowLayout: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['click', 'kebab-click']);

const handleCardClick = () => {
  emit('click', props.video.id);
};

const handleKebabButtonClick = ({ buttonElement }) => {
  emit('kebab-click', {
    videoId: props.video.id,
    buttonElement
  });
};

const handleImageError = (event) => {
  event.target.src = '/path/to/default-thumbnail.jpg';
};

const getPreviewUrl = (fileName) => {
  if (!fileName) return '';
  return `${MINIO_BASE_URL}/videos/${fileName}`;
};

const formatViews = (count) => {
  if (count >= 1000000) {
    return `${(count / 1000000).toFixed(1)}M`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1)}K`;
  }
  return count;
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('ru-RU', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};
</script>

<template>
  <div 
    class="video-card" 
    @click="handleCardClick"
    :class="{ 'row-layout': rowLayout }"
  >
    <div class="video-block">
      <div class="thumbnail-overlay-badge">
        <div class="badge" role="img" :aria-label="`${video.duration} секунд`">
          <div class="badge-text">{{ video.duration }}</div>
        </div>
      </div>
      <img 
        class="video-thumbnail" 
        :src="getPreviewUrl(video.preview?.fileName)" 
        :alt="video.title"
        @error="handleImageError" 
      />
    </div>
    
    <div class="bottom-block">
      <div class="video-info">
        <h3 class="video-title">{{ video.title }}</h3>
        
        <div v-if="rowLayout" class="video-stats">
          <span class="views-count">{{ formatViews(video.viewsCount) }} просмотров</span>
          <span class="upload-time">{{ formatDate(video.created) }}</span>
        </div>
        
        <div v-if="rowLayout" class="channel-info">
          <img 
            v-if="video.user.userAvatar?.fileName"
            :src="`${MINIO_BASE_URL}/${video.user.userAvatar.fileDirInStorage}/${video.user.userAvatar.fileName}`"
            :alt="video.user.userName"
            class="channel-avatar"
            @error="(e) => e.target.src = '/placeholder-avatar.jpg'"
          >
          <div v-else class="avatar-placeholder"></div>
          <span class="channel-name">{{video.user.userName}}</span>
        </div>
        
        <p v-else class="channel-name">{{video.user.userName}}</p>
      </div>
      
      <KebabButton @kebab-click="handleKebabButtonClick"/>        
    </div>
  </div>
</template>

<style scoped>
  .video-card {
      display: block;
      cursor: pointer;
  }

  .video-card.row-layout {
      display: flex;
      width: 100%;
      gap: 20px;
      margin-top: 10px;
      transition: background 0.3s ease;
  }

  .video-block {
      position: relative;
      width: 100%;
      height: auto;
      overflow: hidden;
      aspect-ratio: 16/9;
  }

  .video-card.row-layout .video-block {
      width: 20%;
      flex-shrink: 0;
  }

  /* .video-card.row-layout .video-block {
    width: 20vw;
    flex-shrink: 0;
  } */

  .thumbnail-overlay-badge {
      position: absolute;
      bottom: 5px;
      right: 5px;
      background: rgba(0, 0, 0, 0.8);
      border-radius: 3px;
      padding: 2px 4px;
      z-index: 1;
      color: #f3f0e9;
      font-size: 12px;
  }

  .video-thumbnail {
      display: block;
      width: 100%;
      height: 100%;
      object-fit: cover;
      background: #f39e60;
  }

  .bottom-block {
      display: flex;
      justify-content: space-between;
      width: 100%;
  }

  .video-card.row-layout .bottom-block {
      flex-direction: row;
      justify-content: flex-start;
      gap: 10px;
  }

  .video-info {
      display: flex;
      margin-top: 1vh;
      width: 90%;
      flex-direction: column;
      color: #f3f0e9;
      overflow: hidden;
      word-wrap: break-word;
      white-space: normal;
  }

  .video-card.row-layout .video-info {
      width: 100%;
      margin-top: 0;
  }

  .video-title,
  .channel-name {
      margin: 0;
      overflow: hidden;
      text-overflow: ellipsis;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      line-height: 1.4;
      max-height: calc(2 * 1.4em);
      word-break: break-word;
  }

  .video-card.row-layout .video-title {
      font-size: 18px;
      -webkit-line-clamp: 2;
  }

  .video-stats {
      display: flex;
      gap: 10px;
      margin-top: 6px;
      font-size: 12px;
      color: #F3F0E9;
  }

  .channel-info {
      display: flex;
      align-items: center;
      gap: 10px;
      margin-top: 12px;
  }

  .channel-avatar {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      object-fit: cover;
  }

  .avatar-placeholder {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      background: #4A4947;
  }
</style>