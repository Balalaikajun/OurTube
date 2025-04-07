<script setup>
import { ref, onMounted, onUnmounted, computed } from "vue";
const props = defineProps({
  rowLayout: {
    type: Boolean,
    default: false
  }
});

const layoutClass = computed(() => ({
  'row-layout': props.rowLayout
}));
</script>


<template>
    <div class="video-card-skeleton" :class="layoutClass">
      <div class="thumbnail"></div>
      <div class="info">
        <div class="avatar"></div>
        <div class="details">
          <div class="title"></div>
          <div class="channel"></div>
          <div v-if="rowLayout" class="stats"></div>
        </div>
      </div>
    </div>
  </template>
  
  <style scoped>
  .video-card-skeleton {
    background: transparent;
  }
  
  .video-card-skeleton.row-layout {
    display: flex;
    gap: 20px;
  }
  
  .thumbnail {
    width: 100%;
    height: 0;
    padding-bottom: 56.25%; /* 16:9 aspect ratio */
    background-color: #4A4947;
    animation: pulse 3s infinite ease-in-out;
  }
  
  .row-layout .thumbnail {
    padding-bottom: 22.5%; /* 40% width with 16:9 ratio */
  }
  
  .info {
    display: flex;
    margin-top: 12px;
  }
  
  .row-layout .info {
    flex: 1;
    margin-top: 0;
  }
  
  .avatar {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    background-color: #4A4947;
    margin-right: 12px;
    animation: pulse 3s infinite ease-in-out;
  }
  
  .row-layout .avatar {
    display: none;
  }
  
  .details {
    flex: 1;
  }
  
  .title {
    width: 80%;
    height: 16px;
    background-color: #4A4947;
    margin-bottom: 6px;
    border-radius: 4px;
    animation: pulse 3s infinite ease-in-out;
  }
  
  .row-layout .title {
    width: 100%;
    height: 20px;
  }
  
  .channel {
    width: 60%;
    height: 14px;
    background-color: #4A4947;
    margin-bottom: 4px;
    border-radius: 4px;
    animation: pulse 3s infinite ease-in-out;
  }
  
  .row-layout .channel {
    display: none;
  }
  
  .stats {
    width: 40%;
    height: 14px;
    background-color: #4A4947;
    border-radius: 4px;
    animation: pulse 3s infinite ease-in-out;
  }
  
  @keyframes pulse {
    0%, 100% {
      opacity: 1;
    }
    50% {
      opacity: 0.5;
    }
  }
  </style>