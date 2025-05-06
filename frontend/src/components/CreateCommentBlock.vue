<script setup>
    import { ref, onMounted, onUnmounted} from "vue";
    import { injectFocusEngine } from '@/assets/utils/focusEngine.js';

    const { register, unregister } = injectFocusEngine();

    const commentText = ref('');
    const textareaRef = ref(null);
    const showButtons = ref(false);
    function adjustHeight() {
        if (textareaRef.value) {
            textareaRef.value.style.height = 'auto';
            textareaRef.value.style.height = `${textareaRef.value.scrollHeight}px`;
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
            showButtons.value = false;
            }
        }, 100);        
    };

    const handleCancel = () => {
        // 1. Явно снимаем фокус с textarea
        textareaRef.value.blur();
        
        // 2. Очищаем текст
        commentText.value = '';
        
        // 3. Не меняем showButtons здесь - это сделает handleBlur
    };

    onMounted(() => {
        adjustHeight();
    });
    defineExpose({
        showButtons,
        textareaRef
    });
</script>

<template>
    <div class="comment-create">
      <img 
        @error="event => event.target.style.display = 'none'" 
        class="user-avatar" 
        :src="data" 
        alt="User avatar"
      >
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
        min-height: 10px;
        color: #F3F0E9;
        line-height: 10px; 
        font-size: 14px; /* Размер шрифта */
        overflow-wrap: break-word;
        outline: none;
        resize: none;
        background: transparent;
        border: none;
        border-bottom: 1px solid #F3F0E9;
    }
    .component-input:focus {
        min-height: 20px; /* Базовая высота */
        height: 20px;
    }
    .component-input:focus::placeholder {
        opacity: 0;
    }   
    .comment-button {
        box-sizing: border-box;
        border-radius: 4px;
        padding: 10px;
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