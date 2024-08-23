<template>  
<div class="main-panel">
	<img :src="iconUrl" />
	<div>
		<span v-if="serviceState == 1" class="badge text-bg-success app-status">
			<div class="spinner-grow text-light loading-small" role="status">
				<span class="visually-hidden">Loading...</span>
			</div>
			Running
		</span>
		<span v-else-if="serviceState == 0" class="badge text-bg-warning app-status">Idle</span>
		<span v-else-if="serviceState == -1" class="badge text-bg-danger app-status">Error</span>
		<span v-else class="badge text-bg-primary app-status">
			<div class="spinner-border spinner-border-sm text-light" role="status">
				<span class="visually-hidden">Loading...</span>
			</div>
		</span>
	</div>

	<div v-if="user" class="credentials text-start">
		<div v-if="user.firstName || user.lastName" class="property">
			<div>{{ user.firstName }} {{ user.lastName }}</div>
			<div class="text-secondary">Full name</div>
		</div>
		<div v-if="user.usernames.editableUsername" class="property">
			<div>@{{ user.usernames.editableUsername }}</div>
			<div class="text-secondary">Username</div>
		</div>
		<div class="property">
			<div>{{ user.phoneNumber }}</div>
			<div class="text-secondary">Phone</div>
		</div>
		<div class="property">
			<div>{{ user.id }}</div>
			<div class="text-secondary">User id</div>
		</div>
	</div>
	<div v-else>
		<span class="badge text-bg-warning">Authorization required!</span>
	</div>
	<div class="d-grid gap-2 mt-2">

		<a v-if="serviceState == 0" role="button" 
			:class="isServerStateChangeRequestPending.state ? 'btn btn-primary disabled' : 'btn btn-primary'" 
			@click="startService()" 
			:aria-disabled="isServerStateChangeRequestPending.state">Start service</a>

		<a v-else-if="serviceState == 1" role="button" 
			:class="isServerStateChangeRequestPending.state ? 'btn btn-primary disabled' : 'btn btn-primary'" 
			@click="stopService()" 
			:aria-disabled="isServerStateChangeRequestPending.state">Stop service</a>

		<a v-if="isAuthorized" role="button" class="btn btn-primary btn-small" @click="logOutFromTgAccount()">Sign out from Telegram</a>
		<RouterLink v-else to="/signIn" 
			:class="serviceState == 0 ? 'btn btn-primary disabled' : 'btn btn-primary'" >Sign in to Telegram</RouterLink>

		<RouterLink to="/account" class="btn btn-primary">My account</RouterLink>
		<RouterLink to="/settings" class="btn btn-primary">Settings</RouterLink>
		<a role="button" class="btn btn-primary btn-small" @click="signOut()">Sign out</a>
	</div>
</div>
</template>

<script>
import { store } from '/src/store';
import { withExecutionStateAsync } from '/src/utils';

export default {
	data() {
		return {
			user: null,
			serviceState: null,
			isServerStateChangeRequestPending: { state: false },
			iconUrl: new URL('@/assets/tgs_logo.png', import.meta.url).href,			
		}
	},
	async mounted() {
		await this.updateServiceState();
		setInterval(this.updateServiceState.bind(this), 5000);
		this.user = await store.getCurrentUser();
	},
	methods: {
		async startService() {
			await withExecutionStateAsync(this.isServerStateChangeRequestPending,
				store.startService, 1000);
			await this.updateServiceState();
		},
		async stopService() {
			await withExecutionStateAsync(this.isServerStateChangeRequestPending,
				store.stopService, 1000);
			await this.updateServiceState();
		},
		async updateServiceState() {
			try {
				this.serviceState = await store.getServiceState();
			} catch (e) {
				if (e.response.status === 401) {
					this.$router.push({ name: "SignIn" });
				}
				throw e;
			}
		},
		async logOutFromTgAccount() {
			await store.logOutFromTgAccount();
			this.user = null;
		},
		async signOut() {
			await store.signOut();
			this.$router.push({ name: "SignIn" });
		}
	},
	computed: {
		isAuthorized() {
			return !!this.user;
		}
	}
}
</script>

<style scoped lang="scss">
.main-panel {
	position: relative;
}
.app-status {
}
.loading-small {
	width: 12px;
    height: 12px;
}
.credentials {

	.property {
		line-height: 1;
		margin-top: 7px;
	}
}
</style>