import { ref, nextTick } from 'vue'

export default function useTextOverflow() {
    const isClamped = ref(false)
    
    const checkTextOverflow = (element, from = "элемент") => {
        // console.log(from)
        if (!element) {
            console.warn("Element is null or undefined");
            isClamped.value = false;
            return;
        }

        nextTick(() => {
            try {
                // Временно убираем clamped-стиль для точного измерения
                const wasClamped = element.classList.contains('clamped');
                element.classList.remove('clamped');
                
                // Принудительный reflow
                void element.offsetHeight;
                
                const singleLineHeight = parseFloat(getComputedStyle(element).lineHeight) || 17;
                const isOverflowing = element.scrollHeight > singleLineHeight * 1.5;
                
                // Возвращаем исходное состояние
                if (wasClamped) element.classList.add('clamped');
                
                isClamped.value = isOverflowing;
            } catch (error) {
                console.error("Error checking text overflow:", error);
                isClamped.value = false;
            }
        });
    }

    return {
        isClamped,
        checkTextOverflow
    }
}