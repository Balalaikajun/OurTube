<script setup>
    import { ref, onMounted, onUnmounted } from "vue";
    const commentText = ref('');
    const textareaRef = ref(null);
    const showButtons = ref(false);
    function adjustHeight() {
        if (textareaRef.value) {
            textareaRef.value.style.height = 'auto';
            textareaRef.value.style.height = `${textareaRef.value.scrollHeight}px`;
        }
    }
    onMounted(() => {
        adjustHeight();
    });
    defineExpose({
        showButtons
    });
</script>

<template >
  <div class="comment-create">
    <img @error="event => event.target.style.display = 'none'" class="user-avatar" :src="data" alt="User avatar">
    <div class="comment-container">
        <textarea  
            ref="textareaRef"
            @focus="showButtons = true"            
            @input="adjustHeight" 
            v-model="commentText" 
            class="component-input" 
            placeholder="Комментарий" 
            rows="1"></textarea>
        <div v-if="showButtons" class="functional-buttons-block">
            <button @click="showButtons = false; commentText = ''" class="control-button comment-button">
                Отмена</button>
            <button 
                class="control-button comment-button"
                :class="{ 'disabled-button': !commentText.trim(), 'comment-isFilled': commentText.trim() }"
                :disabled="!commentText.trim()">
                Комментировать</button>
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