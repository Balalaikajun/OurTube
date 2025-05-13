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

            // const params = {
            // limit: props.initialLimit
            // };

            // if (lastCommentId.value && !reset) {
            //     params.after = lastCommentId.value;
            // }

            const response = await api.get(`/api/Video/Comment/${props.videoId}?limit=${props.initialLimit}&after=${nextAfter.value}`);
            const newComments = response.data?.comments || [];
            
            if (newComments.length > 0) {
            commentsData.value = reset ? newComments : [...commentsData.value, ...newComments];
            lastCommentId.value = newComments[newComments.length - 1].id;
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
    <div>
        <LoadingState v-if="isLoading" />
        <CommentBlock
            v-else
          v-for="comment in commentsData"
          :key="comment.id"
          :video-id="props.videoId"
          :parent-id="comment.parentId || null"
          :comment-text="comment.text"
          :create-date="comment.createdAt"
          :update-date="comment.updatedAt"
          :likes-count="comment.likesCount"
          :dislikes-count="comment.dislikesCount"
          :user-info="comment.user"
          :childs="comment.childs || []"
        />
    </div>
</template>

<style scoped>
    div {
        margin-top: 40px;
    }
</style>
