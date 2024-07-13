<template>
    <div v-for="message in messages" :key="message">
        {{ message.text }}
    </div>
</template>

<script>
export default {
    props: {
        messages: {
            reqired: true
        }
    }
}
</script>