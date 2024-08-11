<template>
    <div class="main-panel non-selectable">
        <div class="d-flex flex-row mb-3">
            <RouterLink to="/account" class="btn btn-outline btn-sm me-1"><i class="bi-arrow-left fw-semibold"></i></RouterLink>
            <h1 class="h4">Change password</h1>
        </div>

        <div class="form-floating mb-3">
            <input type="password" v-model="currentPassword" id="phoneNumber" class="form-control" placeholder="">
            <label for="phoneNumber">Current password</label>
            <FormControlMessage v-if="errors" :messages="errors.CurrentPassword"></FormControlMessage>
        </div>
        <div class="form-floating mb-3">
            <input type="password" v-model="newPassword" id="phoneNumber" class="form-control" placeholder="">
            <label for="phoneNumber">New password</label>
            <FormControlMessage v-if="errors" :messages="errors.NewPassword"></FormControlMessage>
        </div>
        <div class="form-floating mb-3">
            <input type="password" v-model="confirmNewPassword" id="phoneNumber" class="form-control" placeholder="">
            <label for="phoneNumber">Confirm password</label>
            <FormControlMessage v-if="errors" :messages="errors.ConfirmNewPassword"></FormControlMessage>
        </div>
        <div class="d-grid gap-2 mt-2">
            <a href="#" role="button" class="btn btn-primary block" @click="save()">
                Submit
            </a>
        </div>
    </div>
</template>

<script>
import { store } from "@/store.js";
import FormControlMessage from "../components/FormControlMessage.vue"
import { useToast } from "vue-toastification";

export default {
    data() {
        return {
            username: null,
            currentPassword: null,
            newPassword: null,
            confirmNewPassword: null,
            errors: null
        }
    },
    methods: {
        async save() {
            const toast = useToast();
            try {
                await store.changePassword({
                    username: this.username,
                    currentPassword: this.currentPassword,
                    newPassword: this.newPassword,
                    confirmNewPassword: this.confirmNewPassword
                });
                this.errors = null;
                this.$router.push({ name: "MyAccount" });

                toast.success("Password changed successfully!");
            } catch (error) {
                this.errors = error.response.data.errors;
            }
        }
    },
    components: {
        FormControlMessage
    }
}
</script>