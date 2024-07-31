import { reactive } from 'vue'
import axios from 'axios'
import Cookies from 'js-cookie'

const client = axios.create({
    baseURL: import.meta.env.VITE_VUE_APP_BASE_URL + "api/"
});

async function get(path) {
    const response = await client.get(path);
    return response.data;
}

async function post(path, body) {
    const response = await client.post(path, body);
    return response.data;
}

export const store = reactive({
  
    async getCurrentUser() {
        return await get("/auth/currentUser");
    },
    async sendAuthCode(phoneNumber) {
        return await post("/auth/setPhone", { phoneNumber });
    },
    async getAuthState() {
        return await get("/auth/authorizationState");
    },
    async checkAuthCode(phoneCode) {
        const response = await client.post("/auth/checkCode", { phoneCode });
        return response.data;
    },
    async logOutFromTgAccount() {
        const response = await client.get("/auth/logOut");
        return response.data;
    },
    async getSettings() {
        const response = await client.get("/settings");
        return response.data;
    },
    async saveSettings(settings) {
        const response = await client.post("/settings", settings);
        return response.data;
    },
    async getServiceState() {
        const response = await client.get("/application/serviceState");
        return response.data;
    },
    async startService() {
        const response = await client.post("/application/startService");
        return response.data;
    },
    async stopService() {
        return await post("/application/stopService");
    },

    // Authorization
    async signIn(username, password) {
        try
        {
            await post("/account/signIn", {
                username: username,
                password: password
            });
            return true;
        }
        catch (e) {
            return false;
        }
    },
    async signOut() {
        await get("/account/signOut");
    }
})