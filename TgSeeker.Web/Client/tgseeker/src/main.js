import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import "../node_modules/bootstrap/dist/js/bootstrap.bundle.min.js";
import "../node_modules/bootstrap/dist/css/bootstrap.min.css";
import "./theme.scss"
import "bootstrap-icons/font/bootstrap-icons.css";

import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";

import { createWebHistory , createRouter } from 'vue-router'

import HomeView from './views/HomeView.vue'
import SignInView from './views/SignInView.vue'
import SettingsView from './views/SettingsView.vue'
import MyAccountView from './views/Account/MyAccountView.vue';
import AccountSignInView from './views/AccountSignInView.vue'
import ChangePasswordView from './views/ChangePasswordView.vue'

const routes = [
    { path: '/', component: HomeView, name: "Home" },
    { path: '/signIn', component: SignInView },
    { path: '/settings', component: SettingsView },
    { path: '/account', component: MyAccountView, name: "MyAccount" },
    { path: '/account/signIn', component: AccountSignInView, name: "SignIn" },
    { path: '/account/changePassword', component: ChangePasswordView },
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})


router.afterEach((to, from) => {
    const toDepth = to.path.split('/').length
    const fromDepth = from.path.split('/').length
    to.meta.transition = toDepth < fromDepth ? 'slide-fade' : 'slide-fade'
})

const toastOptions = {
    position: "bottom-center",
    timeout: 2000,
    closeOnClick: true,
    pauseOnFocusLoss: true,
    pauseOnHover: true,
    draggable: true,
    draggablePercent: 0.6,
    showCloseButtonOnHover: false,
    hideProgressBar: true,
    closeButton: "button",
    icon: true,
    rtl: false
};

createApp(App).use(router).use(Toast, toastOptions).mount('#app')
