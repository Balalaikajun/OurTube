<script setup>
    import { ref } from 'vue';

    const props = defineProps(
        {
            userAvatarPath: String,
            required: true,
            default: ""
        }
    )

    const isSVG = ref(false)
</script>

<template>
    <img
        v-if="!isSVG"
        @error="isSVG = true" 
        class="user-avatar" 
        :src="data" 
        alt="User avatar"
    >
    <svg v-if="isSVG" width="17" height="20" style="fill: #F3F0E9 !important;">
        <path style="fill: inherit;" v-if="!isPlaying" d="M17 10 0 20V0l17 10Z" />
        <path style="fill: inherit;" v-if="isPlaying" d="M.5 19.5V.5h2.886v19H.5Zm16 0h-2.886V.5H16.5v19Z"/>
    </svg>
</template>

<style scoped>
    .channel-avatar {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      object-fit: cover;
    }
    .video-thumbnail {
      display: block;
      width: 100%;
      height: 100%;
      object-fit: cover;
      background: #f39e60;
    }     
    .control-button svg {
    display: inherit;
    box-sizing: border-box;
    justify-content: center;
    align-items: center;
}

.control-button svg path {
    fill: none; 
    stroke: #f3f0e9; 
    stroke-width: 2;
}

.user-avatar {
    display: block;
    width: var(--avatar-size, 40px);
    height: var(--avatar-size, 40px);
    border-radius: var(--avatar-radius, 4px);
    background: #F3F0E9;
    object-fit: cover;
    color: #100E0E;
}
</style>