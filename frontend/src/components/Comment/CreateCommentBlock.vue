<script setup>
import { computed, inject, onMounted, ref } from 'vue'
import api from '@/assets/utils/api.js'
import { useRouter } from 'vue-router'
import { injectFocusEngine } from '@/assets/utils/focusEngine.js'
import UserAvatar from '../Solid/UserAvatar.vue'

const props = defineProps({
        videoId: {
            type: Number,
            required: true,
            default: 0
        }
    })

    const router = useRouter();

    const emit = defineEmits(['close']);

    const { register, unregister } = injectFocusEngine();

    const userData = computed(() => JSON.parse(localStorage.getItem('userData'))); //правки
    const isAuthenticated = computed(() => !!userData.value);
    const commentText = ref('');
    const textareaRef = ref(null);
    const showButtons = ref(false);
    const errorMessage = ref("");

    const rootParentId = inject('rootParentId', 100000000);

    function adjustHeight() {
        if (textareaRef.value) {
            textareaRef.value.style.height = 'auto';
            textareaRef.value.style.height = `${textareaRef.value.scrollHeight + 1}px`;
        }
    }
    const handleFocus = () => {
        register('commentBlock', () => {
            showButtons.value = true;
        });
    };

    const handleBlur = () => {
        setTimeout(() => {
            if (!document.activeElement?.closest('.comment-create')) {
            unregister('commentBlock');
            }
        }, 100);        
    };

    const handleCancel = () => {
        commentText.value = '';
        textareaRef.value.blur();
        textareaRef.value.style.height = 'auto';
        showButtons.value = false;
        if (rootParentId != null)
        {
            handleBlur();
            emit('close');
        }
    };

    const handleComment = async () => {
        errorMessage.value = "";
        
        if (!commentText.value.trim()) {
            errorMessage.value = "Комментарий не может быть пустым";
            return;
        }

        if (!isAuthenticated.value) {
            router.push('/login');
            return;
        }

        try {
            const response = await api.post('Video/Comment', {
                videoId: props.videoId,
                text: commentText.value,
                parentId: rootParentId
            });

            commentText.value = '';
            showButtons.value = false;
            // emit('comment-created', response.data);
            handleCancel();
            
        } catch (error) {
            handleCommentError(error);
        }
    };

    const handleCommentError = (error) => {
        if (error.response) {
            if (error.response.status === 401) {
                errorMessage.value = "Сессия истекла. Пожалуйста, войдите снова";
                localStorage.removeItem('userData');
                localStorage.removeItem('token');
                router.push('/login');
            } else {
                errorMessage.value = error.response.data?.message || "Ошибка сервера";
            }
        } else {
            errorMessage.value = "Ошибка соединения";
        }
        console.error('Ошибка:', error);
    };

    onMounted(() => {
        adjustHeight();
    });
</script>

<template>
    <div class="comment-create" v-if="isAuthenticated">
        <UserAvatar :user-avatar-path="userData?.userAvatar?.fileDirInStorage"/>
        <div class="comment-container">
            <textarea  
                ref="textareaRef"
                @focus="handleFocus"
                @blur="handleBlur"
                @input="adjustHeight" 
                v-model="commentText" 
                class="component-input" 
                placeholder="Комментарий" 
                rows="1"
            ></textarea>
            <div v-if="showButtons" class="functional-buttons-block">
                <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
                <button 
                    @click="handleCancel" 
                    @mousedown.prevent
                    class="reusable-button fit"
                >
                    Отмена
                </button>
                <button 
                    class="reusable-button fit"
                    :class="{ 
                        'disabled': !commentText.trim(), 
                        'isFilled': commentText.trim() 
                    }"
                    :disabled="!commentText.trim()"
                    @click="handleComment" 
                >
                    Комментировать
                </button>
            </div>
        </div>
    </div>
    <div v-else class="auth-prompt">
        <p>Чтобы оставить комментарий, <router-link to="/login">войдите</router-link> в аккаунт</p>
    </div>
</template>

<style scoped>
    .comment-create {
        display: flex;
        flex-direction: row;
        gap: 10px;
        width: 100%;
    }
    .comment-container {
        width: 100%;
    }
    .functional-buttons-block {
        display: flex;
        gap: 10px;
        flex-direction: row;
        justify-content: end;
    }   

    .functional-buttons-block button:first-child:hover {
        background-color: #4A4947;
    }

    .error-message {
        color: #ff4d4f;
        margin-top: 8px;
        font-size: 0.875rem;
    }

    .auth-prompt {
        padding: 16px;
        text-align: center;
        color: #F3F0E9;
    }

    .auth-prompt a {
        color: #F39E60;
        text-decoration: none;
    }

    .auth-prompt a:hover {
        text-decoration: underline;
    }
</style>