<script setup>
    import { ref, onMounted, onBeforeUnmount, computed, watch } from "vue";
    import axios from 'axios';
    import CommentBlock from "./CommentBlock.vue";
    import LoadingState from "./LoadingState.vue";
    import { API_BASE_URL } from "@/assets/config.js";
    import formatter from "@/assets/utils/formatter.js";

    const props = defineProps({
        videoId: {
            type: Number,
            required: true,
            default: 0
        },
        initialLimit: {
            type: Number,
            default: 20
        }
    });

    const commentsData = ref([]);
    const isLoading = ref(false);
    const error = ref(null);
    const lastCommentId = ref(null);
    const hasMore = ref(true);
    const nextAfter = ref(0);

    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true, // Важно для передачи кук
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    const fetchComments = async (reset = false) =>
    {
        if (isLoading.value || !hasMore.value) return;
        try {
            isLoading.value = true;
            error.value = null;

            if (reset) {
                commentsData.value = [];
                lastCommentId.value = null;
                hasMore.value = true;
            }

            const response = await api.get(`/api/Video/Comment/${props.videoId}?limit=${props.initialLimit}&after=${nextAfter.value}`);
            const newComments = response.data?.comments || [];
            
            if (newComments.length > 0) {
                commentsData.value = reset ? newComments : [...commentsData.value, ...newComments];
                lastCommentId.value = newComments[newComments.length - 1].id;
                console.log(commentsData.value)
            }

            hasMore.value = newComments.length >= props.initialLimit;

        } catch (err) {
            error.value = err.response?.data?.message || 
                        err.message || 
                        'Ошибка при загрузке комментариев';
            console.error("Ошибка при загрузке комментариев:", err);
        } finally {
            isLoading.value = false;
        }
    };

    onMounted(fetchComments);

    watch(() => props.videoId, (newId) => {
        if (newId) fetchComments(true);
    });

    const refreshComments = () => {
        fetchComments(true);
    };
    defineExpose({
        refreshComments
    });
</script>

<template>
    <div class="comments-wrapper">
        <LoadingState v-if="isLoading" />
        <template v-if="!isLoading">
            <CommentBlock
                v-for="comment in commentsData"
                :key="comment.id"
                :video-id="props.videoId"
                :parent-id="comment.parentId || null"
                :id="comment.id"
                :comment-text="comment.text"
                :create-date="formatter.formatRussianDate(comment.created)"
                :update-date="formatter.formatRussianDate(comment.updated)"
                :likes-count="comment.likesCount"
                :dislikes-count="comment.dislikesCount"
                :user-info="comment.user"
                :childs="comment.childs || []"
            />
        </template>
    </div>
</template>

<style scoped>
    .comments-wrapper {
        display: flex;
        flex-direction: column;
        margin-top: 40px;
        gap: 10px;
    }
</style>
