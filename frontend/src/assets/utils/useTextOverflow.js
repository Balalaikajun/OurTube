import { ref, nextTick } from 'vue'

export default function useTextOverflow() {
    const isClamped = ref(false)
    
    const checkTextOverflow = (element, from = "элемент") => {
        console.log(from)
        if (!element) {
            console.warn("Element is null or undefined")
            isClamped.value = false
            return
        }

        nextTick(() => {
            try {
                if (element.scrollHeight > 0 && element.clientHeight > 0) {
                    isClamped.value = element.scrollHeight > element.clientHeight
                } else {
                    isClamped.value = false
                    console.warn("Element has zero or undefined dimensions")
                }
            } catch (error) {
                console.error("Error checking text overflow:", error)
                isClamped.value = false
            }
        })
    }

    return {
        isClamped,
        checkTextOverflow
    }
}