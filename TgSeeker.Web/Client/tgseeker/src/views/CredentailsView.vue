<template>
    <div class="main-panel">
        <div class="d-flex flex-row mb-3">
            <RouterLink to="/" class="btn btn-outline"><i class="bi-arrow-left fw-semibold"></i></RouterLink>
            <h1 class="h3">Credentials</h1>
        </div>

        <!-- Authorization -->
         <SignInView></SignInView>
    </div>
</template>

<script>
    import SignInView from './SignInView.vue';
    export default {
        data() {
            return {
            }
        },
        components: {
            SignInView
        }
    }
</script>

<style lang="scss" scoped>
</style>
