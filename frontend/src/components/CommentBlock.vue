<script setup>
    import { ref, onMounted, onUnmounted, watch, nextTick, toRef } from "vue";
    import KebabButton from "./KebabButton.vue";
    import UserAvatar from "./UserAvatar.vue";
    import CreateCommentBlock from "./CreateCommentBlock.vue";
    import useTextOverflow from "@/assets/utils/useTextOverflow";

    const props = defineProps(
        {
            videoId: {
                type: Number,
                required: true,
                default: 0
            },
            parentId: {
                type: [Number, null],
                required: true,
                default: null
            },
            commentText: {
                type: String,
                required: true,
                default: "Не удалось загрузить комментарий."
            },
            createDate: {
                type: Number,
                required: true,
                default: 0
            },
            updateDate: {
                type: Number,
                required: true,
                default: 0
            },
            likesCount: {
                type: Number,
                required: true,
                default: 0
            },
            dislikesCount: {
                type: Number,
                required: true,
                default: 0
            },
            userInfo: {
                type: Object,
                required: true,
                default: {
                    "id": "unknow",
                    "userName": "unknown",
                    "isSubscribed": false,
                    "subscribersCount": 0,
                    "userAvatar": null
                }
            },
            childs: {
                type: Array,
                required: true,
                default: () => []
            }

        }
    )

    const showFullText = ref(false);
    const { isClamped: isTextClamped, checkTextOverflow } = useTextOverflow();
    const commentTextRef = ref(null)
    const commentText = toRef(props, 'commentText');
    const showCreateCommentBlock = ref(false);
    const addComment = ref(null);

    watch(() => commentText, () => {
        nextTick(() => {
            if (commentTextRef.value) {
            checkTextOverflow(commentTextRef.value, "CommentBlock text update")
            }
        })
    })
    
    onMounted(() => {
        nextTick(() => {
            if (commentTextRef.value) {
                checkTextOverflow(commentTextRef.value, "CommentBlock");
            }
        });
    });

</script>

<template>
    <div class="comment-create">
        <UserAvatar/>
        <div class="comment-container">
            <p
                class="comment-text" 
                :class="{ 'clamped': !showFullText && isTextClamped }"
                ref="commentTextRef"
            >
                {{ commentText }}
            </p>
            <button 
                v-if="isTextClamped" 
                @click="showFullText = !showFullText"
                class="show-more-button"
            >
                {{ showFullText ? 'Скрыть' : 'Показать больше' }}
            </button>
            <div class="functional-buttons-block">
            <!-- реакции -->
            <button 
                class="control-button comment-button"
                :class="{ 
                    'disabled-button': !commentText || !commentText.trim(), 
                    'comment-isFilled': commentText && commentText.trim() 
                }"
                :disabled="!commentText || !commentText.trim()"
                @click="showCreateCommentBlock = true" 
            >
                Ответить
            </button>
            </div>
            <!-- создание комментария -->
            <CreateCommentBlock 
                v-if="showCreateCommentBlock" 
                :video-id="Number(videoId)" 
                :parent-id="parentId"
                style="margin-top: 40px;" 
                ref="addComment"
            />
        </div>
      <KebabButton @kebab-click="handleKebabButtonClick"/>  
    </div>
</template>

<style scoped>
    .comment-create {
        color: #f3f0e9;
    }
    .comment-text {
        white-space: pre-line;
        word-wrap: break-word;
        margin: 10px 0;
        transition: all 0.3s ease;
        font-size: 0.875rem;
    }
    .comment-text.clamped {
        display: -webkit-box;
        -webkit-line-clamp: 1; /* Количество строк до обрезки */
        -webkit-box-orient: vertical;
        overflow: hidden;
        text-overflow: ellipsis;
    }
    .functional-buttons-block {
        display: flex;
        gap: 10px;
        flex-direction: row;
        justify-content: end;
    }
    .comment-button {
        box-sizing: border-box;
        border-radius: 4px;
        padding: 10px;
    }
    .show-more-button {
        background: none;
        border: none;
        color: #f3f0e9;
        cursor: pointer;
        padding: 5px 0;
        font-size: 0.875rem;
        margin-top: 5px;
    }

    .show-more-button:hover {
        text-decoration: underline;
    }
</style>