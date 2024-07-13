<template>
    <div class="main-panel">
        <div class="d-flex flex-row mb-3">
            <RouterLink to="/" class="btn btn-outline"><i class="bi-arrow-left fw-semibold"></i></RouterLink>
            <h1 class="h3">Sign in</h1>
        </div>
        <div>
            <Transition name="slide-fade" mode="out-in">

                <div key="1" class="mb-1" v-if="stage === 'setPhone'">
                    <p class="text-body-secondary">Please enter a phone number associated to your Telegram account.</p>
                    <div class="form-floating mb-3">
                        <input type="tel" v-model="phoneNumber" id="phoneNumber" class="form-control" placeholder="">
                        <label for="phoneNumber">Phone Number</label>
                    </div>
                    <span>{{ errorMessage }}</span>
                    <div class="d-grid gap-2 mt-2">
                        <a href="#" role="button" class="btn btn-primary block" @click="sendAuthCode()">
                            Next
                        </a>
                    </div>
                </div>

                <div key="2" class="mb-1" v-else-if="stage === 'checkCode'">
                    <div class="form-floating mb-3">
                        <input v-model="phoneCode" placeholder="xxxxx" id="phoneCode" class="form-control">
                        <label for="phoneCode">code</label>
                    </div>
                    <span>{{ errorMessage }}</span>
                    <div class="d-grid gap-2 mt-2">
                        <a href="#" role="button" class="btn btn-primary block" @click="checkAuthCode()">
                            Next
                        </a>
                    </div>
                </div>

                <div key="3" class="mb-1" v-else-if="stage === 'completed'">
                    <p class="text-center">You are authorized! 🎉</p>
                </div>

            </Transition>
        </div>
    </div>
</template>

<script>
    import { store } from '../store';
    export default {
        data() {
            return {
                isCodeSent: false,
                stage: null, // 'setPhone', 'checkCode', 'completed'
                statesMap: {
                    0: "setPhone",
                    1: "checkCode",
                    2: "completed"
                },
                phoneNumber: "",
                phoneCode: "",
                errorMessage: null,
            }
        },
        async mounted() {
            const authState = await store.getAuthState();
            this.stage = this.statesMap[authState];
        },
        methods: {
            async sendAuthCode() {
                const response = await store.sendAuthCode(this.phoneNumber);
                if (!response.ok) {
                    //todo: add localization
                    this.errorMessage = response.error;
                    return;
                }
                this.errorMessage = null;
                this.stage = 'checkCode';
            },
            async checkAuthCode() {
                const response = await store.checkAuthCode(this.phoneCode);
                if (!response.ok) {
                    //todo: add localization
                    this.errorMessage = response.error;
                    return;
                }
                this.errorMessage = null;
                this.stage = 'completed';
            }
        },
    }
</script>