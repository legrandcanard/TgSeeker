<template>
    <ul>
        <li v-for="msg in messages" :key="msg" class="text-start text-danger">{{ msg }}</li>
    </ul>
</template>

<script>
export default {
    props: [ "messages" ]
}
</script>

<style scoped>
ul {
    list-style: none;
    padding-left: 5px;
}
</style>