<script setup>
import { ref } from 'vue'

const props = defineProps(
        {
            id: {
                type: [Number, null],
                required: true,
                default: null
            },
            title: {
                type: String,
                required: true,
                default: "Не удалось загрузить комментарий."
            },
            count: {
                type: [Number, null],
                required: true,
                default: null
            },
            hasVideo: {
                type: [Boolean, null],
                required: true,
                default: false
            }
        }
    )

    const emit = defineEmits(['select']);

    const localContained = ref(props.hasVideo)

    const handleClick = () => {
        localContained.value = !localContained.value;
        emit('select', props.id, localContained.value);
    }


</script>
<template>
    <div class="playlist-stroke" @click="handleClick">
        <div class="check-box">
            <div :class="{ 'contained': localContained}"></div>
        </div>
        <p class="playlist-title">
            {{ props.title }}
        </p>
    </div>
</template>
<style scoped>
    .check-box {
        display: flex;
        justify-content: center;
        align-items: center;
        box-sizing: border-box;
        border-radius: 4px;
        border: 3px solid #F3F0E9;
        width: var(--size); /* Правильное использование переменной */
        height: var(--size); /* Правильное использование переменной */
        flex-shrink: 0; /* Чтобы не сжимался при нехватке места */
    }
    .contained {
        box-sizing: border-box;
        padding: 5px;
        width: calc(var(--size) - 10px);
        height: calc(var(--size) - 10px);
        background-color: #F39E60;
        border-radius: 4px;
    }
    .playlist-stroke {
        min-width: 250px;
        --size: 30px; 
        display: flex;
        flex-direction: row;
        gap: 10px;
        align-items: center; /* Добавил для вертикального выравнивания */
    }
    .playlist-title {
        display: -webkit-box;
        -webkit-line-clamp: 1; /* Количество строк до обрезки */
        -webkit-box-orient: vertical;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: normal;
        color: #F3F0E9;
        font-size: 0.875rem;
        line-height: 1.2;
        margin: 0; /* Убираем стандартные отступы */
    }
</style>