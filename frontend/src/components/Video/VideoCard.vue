<script setup>
import KebabButton from '../Kebab/KebabButton.vue'
import UserAvatar from '../Solid/UserAvatar.vue'
import formatter from '@/assets/utils/formatter.js'

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
  },
  isShortDelete: {
    type: Boolean,
    required: true,
    default: false
  }
});

const emit = defineEmits(['click', 'kebab-click', 'share', 'delete']);

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

const handleShortDelete = (event) => {
  event.stopPropagation();
  emit('delete', props.video.id);
};

const handleImageError = (event) => {
  console.error(`Uncurrent src preview`)
  event.target.src = '/path/to/default-thumbnail.jpg';
};

const getPreviewUrl = (fileName) => {
  console.log(fileName)
  if (!fileName) return '';
  console.log(`${import.meta.env.VITE_MINIO_BASE_URL}/videos/${fileName}}`)
  return `${import.meta.env.VITE_MINIO_BASE_URL}/videos/${fileName}`;
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
      </div>
      <img 
          class="video-thumbnail" 
          :src="getPreviewUrl(video.preview?.fileName)" 
          :alt="video.title"
        />
        <!-- @error="handleImageError"  -->
    </div>
    
    <div class="bottom-block">
      <div class="video-info">
        <h3 class="video-title">{{ video.title }}</h3>
        
        <div v-if="rowLayout" class="video-stats">
          <span class="views-count">{{ formatter.countFormatter(props.video.viewsCount,'views')}}</span>
          <span class="upload-time">{{ formatter.formatRussianDate(props.video.created)}}</span>
        </div>
        
        <div v-if="rowLayout" class="channel-info">
          <UserAvatar :user-info="video.user || {}" />
          <span class="channel-name">{{video.user.userName}}</span>
        </div>
        
        <p v-else class="channel-name">{{video.user.userName}}</p>
      </div>
      
      <button v-if="isShortDelete" class="control-button kebab-button-type" @click="handleShortDelete">
        <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"> 
            <path 
                d="M 10 10 L 30 30" 
                stroke="#F3F0E9"
                style="stroke-width: 1px !important;"
                fill="none"
            />
            <path 
                d="M 10 30 L 30 10" 
                stroke="#F3F0E9"
                style="stroke-width: 1px !important;"
                fill="none"
            />
        </svg>
      </button>
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
      width: 100%;
      gap: 20px;
      transition: background 1s ease;
  }

  .video-block {
      position: relative;
      width: 100%;
      height: auto;
      overflow: hidden;
      aspect-ratio: 16/9;
      min-height: 0; /* Важно для корректного расчета flex-размеров */
  }

  
  .video-card.row-layout .video-block {
      /* flex: 1 1 1; */
      width: 40%;
      min-width: 0;
      min-height: 0;
  }
  
  .bottom-block {
      display: flex;
      justify-content: space-between;
      width: 100%;
      min-height: 0; /* Важно для корректного расчета flex-размеров */
  }

  .video-card.row-layout .bottom-block {
      flex: 1;
      flex-direction: row;
      min-width: 0;
      min-height: 0;
      justify-content: flex-start;
      gap: 10px;
  }

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
      min-height: 0;
  }

  .video-card.row-layout .video-info {
      width: 100%;
      margin-top: 0;
      flex: 1;
      min-height: 0;
  }

  .video-title,
  .channel-name {
      display: -webkit-box;
      -webkit-line-clamp: 1;
      -webkit-box-orient: vertical;
      overflow: hidden;
      text-overflow: ellipsis;
      line-height: 1.4em;
      max-height: 1.4em;
      word-break: break-word;
      min-height: 0;
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
  @container recommendations-container (min-width: 1200px) {
    .video-card.row-layout .video-block{
      width: 20%;
    }
  }
  /* @container recommendations-container (max-width: 1200px) {
    .video-card.row-layout .video-block{
      width: 20%;
    }
  } */
  @container recommendations-container (max-width: 900px) {
    .video-card.row-layout .video-block{
      width: 40%;
    }
  }
  
  @container recommendations-container (max-width: 800px) {
    .video-card.row-layout .video-block{
      width: 50%;
    }
    /* .video-title {
      font-size: 14px;
    } */
  }

  @container recommendations-container (max-width: 500px) {
    .video-card.row-layout .video-block{
      width: 60%;
    }
  }

  @media (max-width: 480px) {
    
  }
</style>