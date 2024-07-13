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
const routes = [
  { path: '/', component: HomeView },
  { path: '/signIn', component: SignInView },
  { path: '/settings', component: SettingsView },
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

createApp(App).use(router).mount('#app')
