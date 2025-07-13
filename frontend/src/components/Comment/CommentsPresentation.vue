<script setup>
import { ref, watch } from 'vue'
import api from '@/assets/utils/api.js'
import CommentBlock from './CommentBlock.vue'
import LoadingState from '../Solid/LoadingState.vue'

import formatter from '@/assets/utils/formatter.js'
import useInfiniteScroll from '@/assets/utils/useInfiniteScroll.js'

const props = defineProps({
        videoId: {
            type: Number,
            required: true,
            default: 0
        },
        initialLimit: {
            type: Number,
            default: 20
        },
        isInfiniteScroll: {
            type: Boolean,
            default: true
        }
    });

    const emit = defineEmits(['delete', 'edit']);
    const currentCommentId = ref(0);

    // Используем композицию бесконечной прокрутки
    const { 
        data: commentsData, 
        observerTarget,
        hasMore,
        isLoading, 
        error, 
        loadMore,
        reset: resetComments
    } = useInfiniteScroll({
        fetchMethod: async (after) => {
            try {
                const response = await api.get(
                    `Video/Comment/${props.videoId}?limit=${props.initialLimit}&after=${after}`
                );
                // console.log(response)
                // console.log("response hasMore", response.data?.hasMore)
                if(response.data?.hasMore)
                {
                    
                }
                return {
                    items: response.data?.comments || [],
                    nextAfter: response.data?.comments[response.data?.comments.length - 1]?.id || 0,
                    hasMore: response.data?.hasMore
                };
            } catch (err) {
                throw err;
            }            
        },
        initialLoad: true,
        context: 'коммент'
    });

    const handleKebabClick = (event) => {
        currentCommentId.value = event.commentId;
    };

    const deleteComment = async () => {
        try {
            if (!currentCommentId.value) return;
            
            const userData = JSON.parse(localStorage.getItem('userData'));
            const comment = commentsData.value.find(c => c.id === currentCommentId.value);
            
            if (comment && comment.user.id !== userData?.id) {
                throw new Error("Вы не можете удалить чужой комментарий");
            }
            
            await api.delete(`Video/Comment/${currentCommentId.value}`);
            await resetComments();
        } catch (err) {
            error.value = err.response?.data?.message || err.message || 'Ошибка при удалении комментария';
        } finally {
            currentCommentId.value = 0;
        }
    };

    const handleEditComment = async (newText) => {
        try {
            await api.patch(`Video/Comment`, {
                "id": currentCommentId.value,
                "text": newText.text
            });
            await resetComments();
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
            :childs-count="comment.childsCount"
            :likes-count="comment.likesCount"
            :dislikes-count="comment.dislikesCount"
            :user-info="comment.user"
            @kebab-click="handleKebabClick"
            @edit="handleEditComment"
            @delete="handleDelete"
        />
        <LoadingState v-if="isLoading"/>
        <div ref="observerTarget" class="observer-target" v-if="isInfiniteScroll && hasMore">{{ hasMore }}</div>
    </div>
</template>

<style scoped>
    .comments-wrapper {
        display: flex;
        flex-direction: column;
        margin-top: 40px;
        gap: 10px;
    }
    .observer-target {
        width: 100%;
        height: 1px;
        margin: 0;
        padding: 0;
        background: #f39e60;
    }
</style>
