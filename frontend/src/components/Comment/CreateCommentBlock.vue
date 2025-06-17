<script setup>
    import { ref, onMounted, onUnmounted, inject} from "vue";
    import axios from 'axios';
    import { useRouter } from 'vue-router';
    import { injectFocusEngine } from '@/assets/utils/focusEngine.js';
    import UserAvatar from "../Solid/UserAvatar.vue";
    import { API_BASE_URL } from "@/assets/config.js";

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

    const userAvatar = ref('/default-avatar.jpg'); // Путь к дефолтному аватару
    const commentText = ref('');
    const textareaRef = ref(null);
    const showButtons = ref(false);
    const errorMessage = ref("");

    const rootParentId = inject('rootParentId', 100000000);

    const api = axios.create({
        baseURL: API_BASE_URL,
        withCredentials: true, // Важно для передачи кук
        headers: {
            'Content-Type': 'application/json'
        }
    });

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
        console.log(rootParentId)
        if (rootParentId != null)
        {
            console.log('Закрытие', rootParentId);
            emit('close');
        }
    };

    const handleComment = async () => {
        errorMessage.value = "";
        
        if (!commentText.value.trim()) {
            errorMessage.value = "Комментарий не может быть пустым";
            return;
        }

        try {
            if(!localStorage.getItem("token"))
            {
                console.log("Токен не действителен"); //правки
                confirm("Переадресация на страницу авторизации.")
                router.push(`/login`);
                return;
            }   
            console.log('Отправка коммента', rootParentId)
            const response = await api.post('/api/Video/Comment', {
                videoId: props.videoId,
                text: commentText.value,
                parentId: rootParentId
            });

            // Успешная отправка
            commentText.value = '';
            showButtons.value = false;
            
            // emit('comment-created'); //правки

            // console.log()
            handleCancel();
            
        } catch (error) {
            if (error.response) {
            // Сервер ответил с ошибкой
            if (error.response.status === 401) {
                errorMessage.value = "Пожалуйста, войдите в систему";
            } else {
                errorMessage.value = error.response.data?.message || "Ошибка сервера";
            }
            } else if (error.request) {
            // Запрос был сделан, но нет ответа
            errorMessage.value = "Нет ответа от сервера";
            } else {
            // Ошибка при настройке запроса
            errorMessage.value = "Ошибка при отправке комментария";
            }
            console.error('Ошибка:', error);
        }
    };

    onMounted(() => {
        adjustHeight();
    });
</script>

<template>
    <div class="comment-create">
        <UserAvatar/>
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
            <button 
                @click="handleCancel" 
                @mousedown.prevent
                class="control-button comment-button"
            >
                Отмена
            </button>
            <button 
                class="control-button comment-button"
                :class="{ 
                'disabled-button': !commentText.trim(), 
                'comment-isFilled': commentText.trim() 
                }"
                :disabled="!commentText.trim()"
                @click="handleComment" 
            >
                Комментировать
            </button>
            </div>
        </div>
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
    .component-input {
        width: 100%;
        min-height: 20px;
        color: #F3F0E9;
        line-height: 20px; 
        font-size: 14px; /* Размер шрифта */
        overflow-wrap: break-word;
        outline: none;
        resize: none;
        box-sizing: border-box;
        background: transparent;
        border: none;
        border-bottom: 1px solid #F3F0E9;
    }
    .component-input:focus::placeholder {
        opacity: 0;
    }   
    .comment-button {
        box-sizing: border-box;
        border-radius: 4px;
        padding: 10px;
        font-size: 0.875rem;
    }
    .disabled-button {
        cursor: default !important;
    }
    .functional-buttons-block button:first-child:hover {
        background-color: #4A4947;
    }
    .comment-isFilled:hover {
        cursor: pointer !important;
        background-color: #F39E60;
        color: #100E0E;
    }
</style>