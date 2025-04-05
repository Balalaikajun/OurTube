<script setup>
    import { defineProps, ref } from 'vue';
    import { useRouter } from 'vue-router';
    import { API_BASE_URL } from '@/assets/config.js';
    import KebabButton from './KebabButton.vue';

    const router = useRouter();
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
    }
    });

    const buttonRef = ref(null);

    const navigateToVideo = () => {
    router.push(`/video/${props.video.id}`);
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
    <div class="video-card" @click="navigateToVideo">
        <div class="video-preview">
            <!-- <img 
                class="preview-image"
                v-if="video.preview.fileName"
                :src="`${API_BASE_URL}/${video.preview.fileDirInStorage}/${video.preview.fileName}`"
                :alt="video.title"
                @error="(e) => e.target.src = '/placeholder-video.jpg'"
            >
                <div v-else class="preview-placeholder"></div>
                <div class="video-duration">{{ video.endTime }}</div> -->
        </div>

        <div class="video-info">
            <div class="video-meta">
                <h3 class="video-title">{{ video.title }}</h3>
                <div class="video-stats">
                <span class="views-count">{{ formatViews(video.viewsCount) }} просмотров</span>
                <span class="upload-time">{{ formatDate(video.created) }}</span>
                </div>
            </div>

            <div class="channel-info">
                <img 
                    v-if="video.user.userAvatar?.fileName"
                    :src="`${API_BASE_URL}/${video.user.userAvatar.fileDirInStorage}/${video.user.userAvatar.fileName}`"
                    :alt="video.user.userName"
                    class="channel-avatar"
                    @error="(e) => e.target.src = '/placeholder-avatar.jpg'"
                >
                <div v-else class="avatar-placeholder"></div>
                <div class="channel-details">
                    <span class="channel-name">{{ video.user.userName }}</span>
                    <!-- <div class="channel-meta">
                        <span class="subscribers-count">{{ formatViews(video.user.subscribersCount) }} подписчиков</span>
                        <button 
                        class="subscribe-button"
                        :class="{ 'subscribed': video.user.isSubscribed }"
                        @click.stop="$emit('toggle-subscribe', video.user.id)"
                        >
                        {{ video.user.isSubscribed ? 'Вы подписаны' : 'Подписаться' }}
                        </button>
                    </div> -->
                </div>
            </div>
        </div>

        <KebabButton 
            ref="buttonRef" 
            :onClick="handleKebabClick">
        </KebabButton>  
  </div>
</template>

<style scoped>
    .video-card {
        display: flex;
        gap: 20px;
        cursor: pointer;
        transition: background 0.3s ease;
        background: transparent;
        overflow: hidden;
        width: 100%;
    }

    .video-card:hover {
        background: #4A4947;
    }

    .video-preview {
        width: 20vw;
        height: auto;
        position: relative;
        flex-shrink: 0;
        overflow: hidden;
        aspect-ratio: 16/9;
        background: #252525;
    }

    .preview-image {
        width: 20vw;
        height: auto;
        object-fit: cover;
    }

    .preview-placeholder {
        width: 20vw;
        height: auto;
        background: #100E0E;
    }

    .video-duration {
        position: absolute;
        bottom: 8px;
        right: 8px;
        background: rgba(0, 0, 0, 0.7);
        color: #F3F0E9;
        padding: 3px 6px;
        border-radius: 4px;
        font-size: 12px;
    }

    .video-title {
        display: -webkit-box;
        margin: 0;
        font-size: 18px;
        color: #F3F0E9;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        overflow: hidden;
        line-height: 1.4;
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
        background: #252525;
    }

    .channel-details {
        flex-grow: 1;
    }

    .channel-name {
        font-size: 14px;
        color: #F3F0E9;
        display: block;
        margin-bottom: 2px;
    }

    .channel-meta {
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    .subscribers-count {
        font-size: 12px;
        color: #aaa;
    }

    .subscribe-button {
        padding: 4px 10px;
        border-radius: 4px;
        border: none;
        background: #F39E60;
        color: #100E0E;
        font-size: 12px;
        font-weight: bold;
        cursor: pointer;
        transition: all 0.2s ease;
    }

    .subscribe-button:hover {
        background: #e68a4f;
    }

    .subscribe-button.subscribed {
        background: #666;
        color: #ccc;
    }
</style>