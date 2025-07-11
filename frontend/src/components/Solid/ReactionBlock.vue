<script setup>
import { inject, ref, watch } from 'vue'
import api from '@/assets/utils/api.js'

const props = defineProps(
        {
            context: {
                type: String,
                default: 'comment',
                validator: (value) => ['comment', 'video'].includes(value)
            },
            reactionStatus: {
                type: [Boolean, null], // Accept both Boolean and null
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
                default: 10000
            },
        }
    )

    // const emit = defineEmits(['update-reaction']);

const localReaction = ref(props.reactionStatus);
const localLikesCount = ref(props.likesCount);
const localDislikesCount = ref(props.dislikesCount);
const videoId = inject('videoId', null); // Обязательно для видео и комментариев
const commentId = inject('commentId', null); // Необязательно, по умолчанию `null`
const isProcessing = ref(false);

// Синхронизация с пропсами
watch(() => props.reactionStatus, (newVal) => {
  localReaction.value = newVal;
});

watch(() => props.likesCount, (newVal) => {
  localLikesCount.value = newVal;
});

watch(() => props.dislikesCount, (newVal) => {
  localDislikesCount.value = newVal;
});

// const testButton = async () =>
// {
//     if (isProcessing.value) return;
//     isProcessing.value = true;
//     // console.log("Удаление")
//     try
//     {
//         const response = await api.delete(`Video/${videoId.value}/vote`);
//     }
    
//     catch (error) 
//     {
//         console.error('Reaction error:', error);
//         // console.log("В ошибке", localReaction.value)
//         // Откатываем изменения при ошибке
//         // localReaction.value = props.reactionStatus;
//         // localLikesCount.value = props.likesCount;
//         // localDislikesCount.value = props.dislikeCount;
//     } 
//     finally {
//         isProcessing.value = false;
//     }
// }

const handleReaction = async (isLike) => {
  if (isProcessing.value) return;
  isProcessing.value = true;

  try {
    switch(isLike)
    {
        case true:
            if(localReaction.value)
            {
                localLikesCount.value--;
                localReaction.value = null
                // console.log("Удаление лайка", localReaction.value, commentId)
                const responseContext = props.context == `comment` ? `api/Video/Comment/${commentId}/vote` : `api/Video/${videoId.value}/vote`;
                const response = await api.delete(responseContext);
            }
            else
            {
                if (localReaction.value === false)
                {
                    localDislikesCount.value--;
                }
                localLikesCount.value++;
                localReaction.value = true
                // console.log("Смена дизлайка на лайк или добавление лайка", localReaction.value, commentId)
                const responseContext = props.context == `comment` ? `api/Video/Comment/${commentId}/vote` : `api/Video/${videoId.value}/vote`;
                const response = await api.post(responseContext, 
                    true
                );
            }
        break
        case false:
            if(localReaction.value === false)
            {
                localDislikesCount.value--;
                localReaction.value = null;
                // console.log("Удаление дизлайка", localReaction.value, commentId)
                const responseContext = props.context == `comment` ? `api/Video/Comment/${commentId}/vote` : `api/Video/${videoId.value}/vote`;
                const response = await api.delete(responseContext);
            }
            else
            {
                if (localReaction.value === true)
                {
                    localLikesCount.value--;
                } 
                
                localDislikesCount.value++;
                localReaction.value = false
                // console.log("Смена лайка на дизлайк или добавление дизлайка", localReaction.value, commentId)
                const responseContext = props.context == `comment` ? `api/Video/Comment/${commentId}/vote` : `api/Video/${videoId.value}/vote`;
                const response = await api.post( responseContext, 
                    false
                );
            }
        break
    } 
    }
  catch (error) {
    console.error('Reaction error:', error);
    // console.log("В ошибке", localReaction.value)
    // Откатываем изменения при ошибке
    // localReaction.value = props.reactionStatus;
    // localLikesCount.value = props.likesCount;
    // localDislikesCount.value = props.dislikeCount;
  } 
  finally {
    isProcessing.value = false;
  }
};
</script>

<template>
    <div class="control-button">
        <button :disabled="isProcessing" @click="handleReaction(true)" class="reaction-btn like control-button">
            <svg xmlns="http://www.w3.org/2000/svg" width="10" height="30" viewBox="-1 -1 12 32">
            <path  :class="{'active': localReaction === true}"  d="M10 0 L10 30 L5 30 L5 15 L0 15 L10 0 Z"/>
            </svg>
            <span class="grade-count" aria-hidden="true">{{localLikesCount}}</span>
        </button>
        <button :disabled="isProcessing" @click="handleReaction(false)" class="reaction-btn dislike control-button">
            <svg xmlns="http://www.w3.org/2000/svg" width="10" height="30" viewBox="-1 -1 12 32">
            <path  :class="{'active': localReaction === false}"  d="M10 0 L10 30 L5 30 L5 15 L0 15 L10 0 Z" transform="rotate(180 5 14)"/>
            </svg>
            <span class="grade-count" aria-hidden="true">{{localDislikesCount}}</span>
        </button>
        <!-- <button style="width: 40px; height: 40px;" @click="testButton">

        </button> -->
    </div>
</template>

<style scoped>
    .active {
        fill: #F3F0E9;
    }
    .reaction-btn {
        height: 35px;
        padding: 10px;
        background: #2D2D2D;
    }

    .reaction-btn span {
        display: inline-block;
        padding-left: 10px;
        font-size: 0.9rem;
        line-height: 1.3;
    }

    .reaction-btn:hover {
    background: #4A4947;
    }

    .like::after {
        content: "";
        position: absolute;
        right: 0;
        top: 10%;
        height: 80%;
        width: 1px;
        background-color: #f3f0e9;
    }

    .like {
        position: relative;
        border-radius: 4px 0 0 4px;
    }

    .dislike {
        border-radius: 0 4px 4px 0;
    }
</style>