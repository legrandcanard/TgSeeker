<template>
    <div class="main-panel">
        <img :src="iconUrl" class="mb-5" />
        <h1 class="h3 mb-0">Welcome back! 1</h1>
        <p class="text-secondary mb-3">Please enter login details below</p>
        <div class="d-grid gap-3">
            <div class="form-floating">
                <input v-model="username" placeholder="" class="form-control">
                <label for="username">Username</label>
            </div>
            <div class="form-floating">
                <input v-model="password" type="password" placeholder="" class="form-control">
                <label for="username">Password</label>
            </div>
            <a class="btn btn-primary" @click="signIn()">Sign in</a>
        </div>
    </div>
</template>

<script>
import { store } from '@/store.js';
export default {
    data() {
        return {
            iconUrl: new URL('@/assets/tgs_logo.png', import.meta.url).href,
            username: "",
            password: ""
        }
    },
    methods: {
        async signIn() {
            const result = await store.signIn(this.username, this.password);
            console.log(result);
            if (result) {
                this.$router.push({ name: "Home" });
            }
            else {
                alert("Failed");
            }
        }
    }
}
</script>