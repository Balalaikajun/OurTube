<script setup>
    import { ref, onMounted, onUnmounted, watch, nextTick, toRef, provide, inject } from "vue";
    import CommentMenu from "../Kebab/CommentMenu.vue";
    import KebabButton from "../Kebab/KebabButton.vue";
    import UserAvatar from "../Solid/UserAvatar.vue";
    import ReactionBlock from "@/components/Solid/ReactionBlock.vue";
    import CreateCommentBlock from "./CreateCommentBlock.vue";
    import useTextOverflow from "@/assets/utils/useTextOverflow";
    import formatter from "@/assets/utils/formatter.js";
    import { injectFocusEngine } from '@/assets/utils/focusEngine.js';

    const props = defineProps(
        {
            videoId: {
                type: Number,
                required: true,
                default: 0
            },
            parentId: {
                type: [Number, null],
                default: 1
            },
            id: {
                type: [Number, null],
                required: true,
                default: null
            },
            isDeleted: {
                type: [Boolean, null],
                required: true,
                default: false
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
            reactionStatus: {
                type: [Boolean, null],
                required: true,
                default: null
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

    const emit = defineEmits(['kebab-click', 'edit', 'delete', 'close']);

    const { register, unregister } = injectFocusEngine();

    const kebabMenuRef = ref(null);

    const showFullText = ref(false);
    const showChilds = ref(false);
    const { isClamped: isTextClamped, checkTextOverflow } = useTextOverflow();
    const commentTextRef = ref(null)
    const commentText = toRef(props, 'commentText');
    const showCreateCommentBlock = ref(false);
    const addComment = ref(null);
    
    const textareaRef = ref(null);
    const isEditing = ref(false);
    const editedText = ref('');

    provide('commentId', props.id);

    const isRootComment = props.parentId === null;
    const rootParentId = isRootComment ? props.id : inject('rootParentId', null);
    provide('rootParentId', rootParentId);


    const handleEditClick = () => {
        isEditing.value = true;
        editedText.value = props.commentText;
    };

    const handleSave = () => {
        emit('edit', { text: editedText.value }); // Отправляем обновленный текст
        isEditing.value = false; // Закрываем режим редактирования
    };

    const handleDelete = () => {
        emit("delete");
    };


    const handleKebabButtonClick = (event) => {
        event.stopPropagation();
        console.log("Нажатие в handleKebabButtonClick")

        emit('kebab-click', {
            commentId: props.id
        });
        
        kebabMenuRef.value?.openMenu(event.currentTarget);
    };

    const handleChildKebabClick = (event) => {
        console.log("Нажатие в handleChildKebabClick")
        // if (payload?.event) {
        //     payload.event.stopPropagation();
        // }
        emit('kebab-click', {
            commentId: event.commentId
        });
        // kebabMenuRef.value?.openMenu(payload.buttonElement);
    };

    function adjustHeight() {
        if (textareaRef.value) {
            textareaRef.value.style.height = 'auto';
            textareaRef.value.style.height = `${textareaRef.value.scrollHeight + 1}px`;
        }
    }

    const handleFocus = () => {
        register('commentBlock');
    };

    const handleBlur = () => {
        setTimeout(() => {
            if (!document.activeElement?.closest('.comment-create')) {
            unregister('commentBlock');
            }
        }, 100);        
    };

    watch(() => commentText, () => {
        nextTick(() => {
            if (commentTextRef.value) {
                checkTextOverflow(commentTextRef.value, "CommentBlock text update")
            }
        })
    })


    
    onMounted(() => {
        // console.log(props.commentText, props.id, props.childs)
        // console.log(props.commentText + '|','parentID:', props.parentId, 'ID:', props.id, 'isRoot:', isRootComment, rootParentId, 'commentRoot')
        nextTick(() => {
            if (commentTextRef.value) {
                checkTextOverflow(commentTextRef.value, "CommentBlock");
            }
        });
    });

    defineExpose({
        handleEditClick
    });

</script>

<template>
    <CommentMenu
        ref="kebabMenuRef"
        @edit-click="handleEditClick"
        @delete="handleDelete"
    />
    <div class="comment-wrapper" :class="{'inner-class': !isRootComment}">
        <div class="comment-container">
            <UserAvatar/>
            <div v-if="!isEditing" class="comment-center">
                <div class="comment-header">
                    <p class="user-name">
                        {{ props.userInfo.userName }}
                    </p>
                    <p class="public-time">
                        {{ props.createDate }}
                    </p>
                </div>
                <p v-if="isDeleted === false"
                    class="comment-text" 
                    :class="{ 'clamped': !showFullText}"
                    ref="commentTextRef"
                >
                    {{ commentText }}
                </p>
                <p v-if="isDeleted"
                    class="comment-text" 
                    :class="{ 'clamped': !showFullText}"
                    ref="commentTextRef"
                >
                    Комментарий удалён.
                </p>

                <button 
                    v-if="isTextClamped" 
                    @click="showFullText = !showFullText"
                    class="show-more-button"
                >
                    {{ showFullText ? 'Скрыть' : 'Показать больше' }}
                </button>
                <div v-if="(!isDeleted && isRootComment === false) || (!isDeleted && isRootComment === true)" class="functional-buttons-block">
                    <ReactionBlock 
                        :reaction-status="props.reactionStatus"
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
                    style="margin-top: 10px;" 
                    ref="addComment"
                    @close="showCreateCommentBlock = false"
                />

            </div>
            <div v-if="isEditing" class="comment-center">
                <textarea
                    ref="textareaRef"
                    @focus="handleFocus"
                    @blur="handleBlur"
                    @input="adjustHeight"   
                    v-model="editedText"
                    class="component-input"    
                    rows="1"
                >

                </textarea>
                <div class="functional-buttons-block" style="justify-content: flex-end;">
                    <button 
                        @click="handleSave" 
                        class="control-button comment-button"
                        :class="{ 
                            'disabled-button': commentText === editedText, 
                            'comment-isFilled': commentText !== editedText
                        }"
                        :disabled="commentText === editedText"
                    >Сохранить</button>
                    <button @click="isEditing = false" class="control-button comment-button">Отмена</button>                    
                </div>
            </div>
            <KebabButton
                @kebab-click.stop="handleKebabButtonClick"
            />  
        </div>
        <div class="childs-comments"
            v-if="showChilds && childs && childs.length > 0"       
        >
            <CommentBlock
                v-for="child in props.childs"
                :key="child.id"
                :video-id="props.videoId"
                :id="child.id"
                :is-deleted="child.isDeleted"
                :comment-text="child.text"
                :create-date="formatter.formatRussianDate(child.created)"
                :update-date="formatter.formatRussianDate(child.updated)"
                :reaction-status="child.vote"
                :likes-count="child.likesCount"
                :dislikes-count="child.dislikesCount"
                :user-info="child.user"
                @kebab-click="() => handleChildKebabClick({
                    event: $event,
                    commentId: child.id,
                    buttonElement: $event?.currentTarget
                })"
            />
        </div>
    </div>
   
</template>

<style scoped>
    .component-input {
        width: 100%;
        min-height: 15px;
        color: #F3F0E9;
        line-height: 15px; 
        font-size: 14px; /* Размер шрифта */
        overflow-wrap: break-word;
        outline: none;
        resize: none;
        box-sizing: border-box;
        background: transparent;
        border: none;
        border-bottom: 1px solid #F3F0E9;
    }

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
        display: flex;
        flex-direction: column;
        transition: all 0.3s ease;
        gap: 10px;
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
        min-width: 0;
        flex: 1;        
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
        line-height: 0.9;
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
        width: 100%;
    }
    .disabled-button {
        cursor: default !important;
    }
    .comment-button {
        padding: 10px;
        font-size: 0.875rem;
    }
    .comment-button:hover{
        background-color: #4A4947;
    }
    .disabled-button:hover {
        background-color: #100E0E; /* Keep the same color on hover */
    }
    .comment-isFilled:hover {
        cursor: pointer !important;
        background-color: #F39E60 !important;
        color: #100E0E;
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