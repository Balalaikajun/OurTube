<script setup>
    const props = defineProps({
        message: {
            type: String,
            required: true
        },
        // show: {
        //     type: Boolean,
        //     required: true
        // }
    })
</script>
<template>
    <p>{message}</p>
</template>
<style scoped>

</style>