<script setup>
    import { onMounted, ref } from 'vue';

    const props = defineProps(
        {
            userAvatarPath: String,
            required: true,
            default: ""
        }
    )

    const showImage = ref(false);

    onMounted(() => {
        showImage.value = !!props.userAvatarPath; // Показываем только если путь не пустой
    });
</script>

<template>
    <div class="avatar-container">
        <!-- Показываем изображение только если есть путь И не было ошибки -->
        <img
        v-if="showImage"
        class="user-avatar"
        :src="props.userAvatarPath"
        alt="User avatar"
        >
    
        <!-- Fallback SVG -->
        <svg 
        v-else
            class="user-avatar"
            viewBox="0 0 40 40"
            style="fill: #F3F0E9"
        >
        <circle cx="20" cy="20" r="5" fill="#100E0E"/>
        <text x="20" y="20" text-anchor="middle" fill="#100E0E">?</text>
        </svg>
  </div>
</template>

<style scoped>
    :root {
        --avatar-size: 40px;
        --avatar-radius: 4px;
    }
    .avatar-container {
        display: inline-block;
        border-radius: 4px;
        /* overflow: hidden; */
        width: var(--avatar-size, 40px);
        height: var(--avatar-size, 40px);
        background: #F3F0E9;
    }

    
    @container (max-width: 500px) {
        .avatar-container {
            --avatar-size: 30px;
        }
    }
    @container (max-width: 300px) {
        .avatar-container {
            --avatar-size: 20px;
        }
    }

    .user-avatar {
        width: 100%;
        height: 100%;
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
        border-radius: var(--avatar-radius);
        /* background: #F3F0E9; */
        background: transparent;
    }
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


</style>