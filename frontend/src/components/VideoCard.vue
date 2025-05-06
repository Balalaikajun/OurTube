<script setup>
import { ref } from "vue";
import KebabButton from "../components/KebabButton.vue";
import { MINIO_BASE_URL } from "@/assets/config.js";
import formatter from "@/assets/utils/formatter.js";

const props = defineProps({
  video: {
    type: Object,
    required: true,
    default: () => ({
      id: 0,
      title: '',
      viewsCount: 0,
      vote: false,
      duration: "00:00:00",
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

const emit = defineEmits(['click', 'kebab-click', 'share']);

const handleCardClick = (e) => {
  e.stopPropagation();
  emit('click');
};

const handleKebabButtonClick = (event) => {
  event.stopPropagation();
  emit('kebab-click', {
    videoId: props.video.id,
    buttonElement: event.currentTarget
  });
};

const handleImageError = (event) => {
  event.target.src = '/path/to/default-thumbnail.jpg';
};

const getPreviewUrl = (fileName) => {
  if (!fileName) return '';
  return `${MINIO_BASE_URL}/videos/${fileName}`;
};

</script>

<template>
  <div 
    class="video-card" 
    :class="{ 'row-layout': rowLayout }"
    @click="handleCardClick"
  >
    <div class="video-block">
      <div class="thumbnail-overlay-badge">
        <div class="badge-text">{{ formatter.formatDuration(video.duration) }}</div>
        <!-- <div class="badge" role="img" :aria-label="`${video.duration} секунд`">
          <div class="badge-text">{{ video.duration }}</div>
        </div> -->
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
          <span class="views-count">{{ formatter.countFormatter(props.video.viewsCount,'views')}}</span>
          <span class="upload-time">{{ formatter.formatRussianDate(props.video.created)}}</span>
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
      pointer-events: auto !important;
  }

  .video-card.row-layout {
      display: flex;
      width: 80%;
      gap: 20px;
      transition: background 1s ease;
  }

  .video-block {
      position: relative;
      width: 100%;
      height: auto;
      overflow: hidden;
      aspect-ratio: 16/9;
  }

  .video-card.row-layout .video-block {
      flex-grow: 1;
      flex-shrink: 2;
      flex-basis: 30%;
      /* width: 20%; */
      flex-shrink: 0;
  }

  
  .bottom-block {
      display: flex;
      justify-content: space-between;
      width: 100%;
  }

  .video-card.row-layout .bottom-block {
      flex-grow: 2;
      flex-shrink: 1;
      flex-basis: 70%;
      /* flex-direction: row; */
      justify-content: flex-start;
      gap: 10px;
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

  .video-info {
      display: flex;
      margin-top: 1vh;
      width: 80%;
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
      /* margin: 0;
      overflow: hidden;
      text-overflow: ellipsis;
      display: -webkit-box;
      -webkit-line-clamp: 1;
      -webkit-box-orient: vertical;
      line-height: 1.4;
      max-height: calc(1 * 1.4em);
      word-break: break-word; */

      display: -webkit-box;
      -webkit-line-clamp: 1; /* Ограничиваем одной строкой */
      -webkit-box-orient: vertical;
      overflow: hidden;
      text-overflow: ellipsis;
      
      /* Дополнительные свойства для лучшего отображения */
      line-height: 1.4em;
      max-height: 1.4em; /* Высота одной строки */
      word-break: break-word;
  }

  .video-card.row-layout .video-title {
    font-size: 18px;
    -webkit-line-clamp: 1; /* Сохраняем одну строку */
    max-height: 1.4em;
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