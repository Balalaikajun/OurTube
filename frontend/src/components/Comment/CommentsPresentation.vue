<script setup>
    import { ref, onMounted, computed, watch } from "vue";
    import axios from 'axios';
    import CommentBlock from "./CommentBlock.vue";
    import LoadingState from "../Solid/LoadingState.vue";
    import { API_BASE_URL } from "@/assets/config.js";
    import formatter from "@/assets/utils/formatter.js";
    import useInfiniteScroll from "@/assets/utils/useInfiniteScroll.js";

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

    const emit = defineEmits(['delete', 'edit']);
    const currentCommentId = ref(0);

    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true,
        headers: {
            'Content-Type': 'application/json'
        }
    });

    // Используем композицию бесконечной прокрутки
    const { 
        data: commentsData, 
        observerTarget, 
        isLoading, 
        error, 
        loadMore,
        reset: resetComments
    } = useInfiniteScroll({
        fetchMethod: async (after) => {
            try {
                const response = await api.get(
                    `/api/Video/Comment/${props.videoId}?limit=${props.initialLimit}&after=${after}`
                );
                return {
                    videos: response.data?.comments || [],
                    nextAfter: response.data?.comments[response.data?.comments.length - 1]?.id || 0
                };
            } catch (err) {
                throw err;
            }
        },
        initialLoad: true
    });

    const handleKebabClick = (event) => {
        currentCommentId.value = event.commentId;
    };

    const deleteComment = async () => {
        try {
            await api.delete(`/api/Video/Comment/${currentCommentId.value}`);
            resetComments();
        } catch (err) {
            error.value = err.response?.data?.message || err.message || 'Ошибка при удалении комментария';
        } finally {
            currentCommentId.value = 0;
        }
    };

    const handleEditComment = async (newText) => {
        try {
            await api.patch(`/api/Video/Comment`, {
                "id": currentCommentId.value,
                "text": newText.text
            });
            resetComments();
        } catch (error) {
            console.error("Ошибка редактирования:", error);
        }
    };

    const handleDelete = () => {
        emit("delete");
    };

    // При изменении videoId сбрасываем и загружаем заново
    watch(() => props.videoId, () => {
        resetComments();
    });

    defineExpose({
        refreshComments: resetComments,
        deleteComment
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
                :isDeleted="comment.isDeleted"
                :comment-text="comment.text"
                :create-date="formatter.formatRussianDate(comment.created)"
                :update-date="formatter.formatRussianDate(comment.updated)"
                :reaction-status="comment.vote"
                :likes-count="comment.likesCount"
                :dislikes-count="comment.dislikesCount"
                :user-info="comment.user"
                :childs="comment.childs || []"
                @kebab-click="handleKebabClick"
                @edit="handleEditComment"
                @delete="handleDelete"
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
