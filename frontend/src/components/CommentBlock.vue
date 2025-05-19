<script setup>
    import { ref, onMounted, onUnmounted, watch, nextTick, toRef, provide } from "vue";
    import KebabButton from "./KebabButton.vue";
    import UserAvatar from "./UserAvatar.vue";
    import ReactionBlock from "@/components/ReactionBlock.vue";
    import CreateCommentBlock from "./CreateCommentBlock.vue";
    import useTextOverflow from "@/assets/utils/useTextOverflow";
    import formatter from "@/assets/utils/formatter.js";

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
            id: {
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
                type: String,
                required: true,
                default: 0
            },
            updateDate: {
                type: String,
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
                default: () => []
            }

        }
    )

    const showFullText = ref(false);
    const showChilds = ref(false);
    const { isClamped: isTextClamped, checkTextOverflow } = useTextOverflow();
    const commentTextRef = ref(null)
    const commentText = toRef(props, 'commentText');
    const showCreateCommentBlock = ref(false);
    const addComment = ref(null);

    provide('commentId', props.id);

    watch(() => commentText, () => {
        nextTick(() => {
            if (commentTextRef.value) {
            checkTextOverflow(commentTextRef.value, "CommentBlock text update")
            }
        })
    })
    
    onMounted(() => {
        console.log(props.commentText, props.id, props.childs)
        nextTick(() => {
            if (commentTextRef.value) {
                checkTextOverflow(commentTextRef.value, "CommentBlock");
            }
        });
    });

</script>

<template>
    <div class="comment-wrapper" :class="{'inner-class': parentId != null}">
        <div class="comment-container">
            <UserAvatar/>
            <div class="comment-center">
                <div class="comment-header">
                    <p class="user-name">
                        {{ props.userInfo.userName }}
                    </p>
                    <p class="public-time">
                        {{ props.createDate }}
                    </p>
                </div>
                <p
                    class="comment-text" 
                    :class="{ 'clamped': !showFullText}"
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
                <!-- :reaction-status="vote" -->
                    <ReactionBlock 
                        :likes-count="props.likesCount" 
                        :dislikes-count="props.dislikesCount"
                    />
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
                <!-- комментарии комментария -->
                <button 
                    v-if="childs && childs.length > 0 && !showChilds" 
                    @click="showChilds = true"
                    class="control-button show-replies-btn"
                >
                    Ответы ({{ childs.length }})
                </button>
                <CreateCommentBlock
                    v-if="showCreateCommentBlock" 
                    :video-id="Number(videoId)" 
                    :parent-id="Number(id)"
                    style="margin-top: 10px;" 
                    ref="addComment"
                    @close="showCreateCommentBlock = false"
                />

            </div>
            <KebabButton class="kebab-button"
            
            />  
        </div>
        <div class="childs-comments"
            v-if="showChilds && childs && childs.length > 0"       
        >
            <CommentBlock
                v-for="child in props.childs"
                :key="child.id"
                :video-id="props.videoId"
                :parent-id="props.id"
                :id="props.id"
                :comment-text="child.text"
                :create-date="formatter.formatRussianDate(child.created)"
                :update-date="formatter.formatRussianDate(child.updated)"
                :likes-count="child.likesCount"
                :dislikes-count="child.dislikesCount"
                :user-info="child.user"
            />
        </div>
    </div>
   
</template>

<style scoped>
    .comment-wrapper {
        display: flex;
        flex-direction: column;
        gap: 10px;
        width: 100%;
        --left-side: 40px;
    }

    .inner-class {
        padding-left: calc(var(--left-side) + 1em);
        box-sizing: border-box;
    }

    @container (max-width: 500px) {
        .inner-class {
            --left-side: 30px;
        }
    }
    @container (max-width: 300px) {
        .inner-class {
            --left-side: 20px;
        }
    }


    .childs-comments {
        transition: all 0.3s ease;
        box-sizing: border-box;
        width: 100%;
    }
    .comment-container {
        display: flex;
        flex-direction: row;
        box-sizing: border-box;
        gap: 10px;
        width: 100%;
        color: #f3f0e9;
        position: relative;
        max-width: 100%; /* Ограничиваем максимальную ширину */
        overflow: hidden; /* Скрываем выходящее за пределы содержимое */
    }
    .comment-center {
        display: flex;
        flex-direction: column;
        align-items: start;
        width: calc(100% - 80px);
        min-width: 0;
    }
    .comment-header {
        display: inline-flex;
        gap: 1em;
    }
    .user-name {
        -webkit-text-stroke: 0.3px currentColor;
        font-size: 0.875rem;
    }
    .public-time {
        opacity: 0.8;
        font-size: 0.8rem;
        align-self: flex-end;
        line-height: 0.8;
    }
    .comment-text {
        width: 100%;
        margin-top: 15px;
        /* min-height: 17px; */
        max-width: 100%; /* Гарантируем, что текст не выйдет за пределы */
        white-space: pre-line;
        word-wrap: break-word;
        overflow-wrap: break-word; /* Принудительный перенос длинных слов */
        word-break: break-word; /* Дополнительное правило для переноса */
        transition: all 0.3s ease;
        font-size: 0.875rem;
        line-height: 1.2;        
    }
    .comment-text.clamped {
        display: -webkit-box;
        -webkit-line-clamp: 1; /* Количество строк до обрезки */
        -webkit-box-orient: vertical;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: normal;
        /* min-height: 17px; */
    }
    .functional-buttons-block {
        display: flex;
        margin-top: 5px;
        gap: 10px;
        flex-direction: row;
        justify-content: start;
    }
    .comment-button {
        padding: 10px;
        font-size: 0.875rem;
    }
    .comment-button:hover {
        background-color: #4A4947;
    }
    .kebab-button {
        position: absolute;
        top: 0;
        right: 0;
    }
    .show-more-button {
        background: none;
        border: none;
        color: #f3f0e9;
        cursor: pointer;
        padding: 5px 0;
        font-size: 0.875rem;
    }    
    .show-more-button:hover {
        text-decoration: underline;
    }

    .show-replies-btn {
        margin-top: 5px;
        font-size: 0.875rem;
    }
</style>