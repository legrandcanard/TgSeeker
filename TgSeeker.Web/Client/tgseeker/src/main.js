import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import "../node_modules/bootstrap/dist/js/bootstrap.bundle.min.js";
import "../node_modules/bootstrap/dist/css/bootstrap.min.css";
import "./theme.scss"
import "bootstrap-icons/font/bootstrap-icons.css";

import { createWebHistory , createRouter } from 'vue-router'

import HomeView from './views/HomeView.vue'
import SignInView from './views/SignInView.vue'
import SettingsView from './views/SettingsView.vue'
import AccountSignInView from './views/AccountSignInView.vue'

const routes = [
    { path: '/', component: HomeView, name: "Home" },
    { path: '/signIn', component: SignInView },
    { path: '/settings', component: SettingsView },
    { path: '/account/signIn', component: AccountSignInView, name: "SignIn" },
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

import { store } from './store.js';
/*
router.beforeEach((to, from) => {
    if (!store.isUserAuthorized() && to.name !== "SignIn") {
        return { name: "SignIn" };
    }
});
*/
router.afterEach((to, from) => {
    const toDepth = to.path.split('/').length
    const fromDepth = from.path.split('/').length
    to.meta.transition = toDepth < fromDepth ? 'slide-fade' : 'slide-fade'
})

createApp(App).use(router).mount('#app')
