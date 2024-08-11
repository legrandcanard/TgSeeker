<template>
    <div class="main-panel non-selectable">
        <div class="d-flex flex-row mb-0">
            <RouterLink to="/" class="btn btn-outline"><i class="bi-arrow-left fw-semibold"></i></RouterLink>
            <h1 class="h3">My account</h1>
        </div>
        <p class="text-start text-secondary">Your application internal account.</p>

		<div class="property">
			<div class="fw-semibold selectable">{{ userName }}</div>
			<div class="property-label text-secondary">Username</div>
		</div>

        <div id="thatsyou" class="mb-3 non-selectable">
            That's you! <picture>
                <source srcset="https://fonts.gstatic.com/s/e/notoemoji/latest/1f389/512.webp" type="image/webp">
                <img src="https://fonts.gstatic.com/s/e/notoemoji/latest/1f389/512.gif" alt="🎉" width="32" height="32">
            </picture>
        </div>

        <div class="d-grid gap-2 mt-3">
            <RouterLink to="/account/changePassword" class="btn btn-primary">Change password</RouterLink>
        </div>
    </div>
</template>

<script>
import { store } from '@/store'
import { sleep } from '@/utils'

export default {
    data() {
        return {
            userName: null
        }
    },
    async beforeCreate() {
        const details = await store.getAccountDetails();
        this.userName = details.userName;
    }
}
</script>

<style scoped>
#thatsyou {
    display: inline-flex;
    align-items: flex-end;
    gap: 5px;
}
</style>