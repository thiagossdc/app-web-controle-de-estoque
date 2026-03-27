import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'

import App from './App.vue'
import Login from './components/Login.vue'
import Dashboard from './components/Dashboard.vue'
import Users from './components/Users.vue'
import Products from './components/Products.vue'
import Categories from './components/Categories.vue'
import Suppliers from './components/Suppliers.vue'
import StockMovements from './components/StockMovements.vue'

import './assets/main.css'

const routes = [
  { path: '/', component: Dashboard, meta: { requiresAuth: true } },
  { path: '/login', component: Login },
  { path: '/users', component: Users, meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/products', component: Products, meta: { requiresAuth: true } },
  { path: '/categories', component: Categories, meta: { requiresAuth: true } },
  { path: '/suppliers', component: Suppliers, meta: { requiresAuth: true } },
  { path: '/stock-movements', component: StockMovements, meta: { requiresAuth: true } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)
app.mount('#app')