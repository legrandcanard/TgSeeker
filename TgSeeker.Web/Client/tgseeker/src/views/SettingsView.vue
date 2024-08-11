<template>
    <div class="main-panel">
        <div class="d-flex flex-row mb-3">
            <RouterLink to="/" class="btn btn-outline"><i class="bi-arrow-left fw-semibold"></i></RouterLink>
            <h1 class="h3">Settings</h1>
        </div>

        <a href="https://core.telegram.org/api/obtaining_api_id" target="_blank">Guide on obtaining app credentails</a>

        <div class="text-start">
            <div class="mb-2">
                <label for="api_id" class="form-label">api_id</label>
                <input v-model="apiId" id="api_id" class="form-control">
            </div>
            <div class="mb-2">
                <label for="api_hash" class="form-label">api_hash</label>
                <input v-model="apiHash" id="api_hash" class="form-control">
            </div>
        </div>
        <div class="d-grid gap-2">
            <a href="#" role="button" :class="saveBtnClass" @click="save()">Save</a>
        </div>
    </div>
</template>

<script>
import { store } from '../store'
import { withExecutionStateAsync } from '/src/utils';
import { useToast } from "vue-toastification";

export default {
    data() {
        return {
            apiId: null,
            apiHash: null,
            isSaveRequestPending: { state: false }
        }
    },
    async mounted() {
        const settings = await store.getSettings();
        this.apiId = settings.apiId;
        this.apiHash = settings.apiHash;
    },
    methods: {
        async save() {
            await withExecutionStateAsync(this.isSaveRequestPending,
                async () => await store.saveSettings({ apiId: this.apiId, apiHash: this.apiHash }), 1000);
            const toast = useToast();
            toast.success("Settings saved successfully!");
            this.$router.push({ name: "Home" });
        }
    },
    computed: {
        saveBtnClass() {
            return this.isSaveRequestPending.state ? "btn btn-primary block disabled" : "btn btn-primary block";
        }
    }
}
</script>