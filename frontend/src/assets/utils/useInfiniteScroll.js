import { ref, onMounted, onUnmounted, nextTick } from "vue";

export default function useInfiniteScroll(options) {
    const {
        fetchMethod,
        scrollElement = 'window',
        isEnabled = true,
        initialLoad = true,
        threshold = 0.1,
        rootMargin = '0px',
        onLoadMore
    } = options;

    const data = ref([]);
    const observerTarget = ref(null);
    const hasMore = ref(true);
    const nextAfter = ref(null);
    const isLoading = ref(false);
    const error = ref(null);
    const observer = ref(null);
    const container = ref(null);

    const getScrollElement = () => {
        if (scrollElement === 'window') return null;
        if (scrollElement === 'parent') return container.value?.parentElement;
        return document.querySelector(scrollElement);
    };

    const initIntersectionObserver = () => {
        if (!isEnabled || !observerTarget.value) return;

        observer.value?.disconnect();

        const options = {
            root: getScrollElement(),
            rootMargin,
            threshold
        };

        observer.value = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !isLoading.value && hasMore.value) {
                    loadMore();
                }
            });
        }, options);

        observer.value.observe(observerTarget.value);
    };

    const loadMore = async (reset = false) => {
        if (isLoading.value || (!reset && !hasMore.value)) return;
        
        try {
            isLoading.value = true;
            
            if (reset) {
                data.value = [];
                nextAfter.value = 0;
                hasMore.value = true;
            }

            const result = await fetchMethod(nextAfter.value);
            
            if (Array.isArray(result)) {
                data.value = reset ? result : [...data.value, ...result];
                hasMore.value = result.hasMore;
            } else if (result?.videos) {
                data.value = reset ? result.videos : [...data.value, ...result.videos];
                nextAfter.value = result.nextAfter || null;
                hasMore.value = result.hasMore;
            } else {
                console.error('Неподдерживаемый формат данных');
                hasMore.value = false;
            }

            if (onLoadMore) {
                onLoadMore();
            }

            await nextTick();
            initIntersectionObserver();
        } catch (err) {
            error.value = err.message || 'Ошибка загрузки данных';
            console.error('Ошибка при загрузке:', err);
            hasMore.value = false;
        } finally {
            isLoading.value = false;
        }
    };

    onMounted(() => {
        if (initialLoad) {
            loadMore(true);
        }
    });

    onUnmounted(() => {
        observer.value?.disconnect();
    });

    return {
        data,
        observerTarget,
        hasMore,
        isLoading,
        error,
        container,
        loadMore,
        reset: () => loadMore(true)
    };
}